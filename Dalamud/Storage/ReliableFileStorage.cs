using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Dalamud.Logging.Internal;
using Dalamud.Utility;

using Turso;

namespace Dalamud.Storage;

/*
 * TODO: A file that is read frequently, but written very rarely, might not have offline changes by users persisted
 * into the backup database, since it is only written to the backup database when it is written to the filesystem.
 */

/// <summary>
/// A service that provides a reliable file storage.
/// Implements a VFS that writes files to the disk, and additionally keeps files in a SQLite database
/// for journaling/backup purposes.
/// Consumers can choose to receive a backup if they think that the file is corrupt.
/// </summary>
/// <remarks>
/// This is not an early-loaded service, as it is needed before they are initialized.
/// </remarks>
[ServiceManager.ProvidedService]
internal class ReliableFileStorage : IInternalDisposableService
{
    private static readonly ModuleLog Log = ModuleLog.Create<ReliableFileStorage>();

    private readonly Lock syncRoot = new();

    private TursoConnection? db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReliableFileStorage"/> class.
    /// </summary>
    /// <param name="vfsDbPath">Path to the VFS.</param>
    public ReliableFileStorage(string vfsDbPath)
    {
        var databasePath = Path.Combine(vfsDbPath, "dalamudVfs.db");

        Log.Verbose("Initializing VFS database at {Path}", databasePath);

        try
        {
            this.SetupDb(databasePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load VFS database, starting fresh");

            try
            {
                if (File.Exists(databasePath))
                    File.Delete(databasePath);

                this.db?.Dispose();
                this.db = null;

                this.SetupDb(databasePath);
            }
            catch
            {
                // ignored, we can run without one
            }
        }
    }

    /// <summary>
    /// Check if a file exists.
    /// This will return true if the file does not exist on the filesystem, but in the transparent backup.
    /// You must then use this instance to read the file to ensure consistency.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="containerId">The container to check in.</param>
    /// <returns>True if the file exists.</returns>
    public bool Exists(string path, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        return File.Exists(path) || (this.db != null && this.ExistsInDatabase(path, containerId));
    }

    /// <summary>
    /// Write all text to a file.
    /// </summary>
    /// <param name="path">Path to write to.</param>
    /// <param name="contents">The contents of the file.</param>
    /// <param name="containerId">Container to write to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WriteAllTextAsync(string path, string? contents, Guid containerId = default)
        => await this.WriteAllTextAsync(path, contents, Encoding.UTF8, containerId);

    /// <summary>
    /// Write all text to a file.
    /// </summary>
    /// <param name="path">Path to write to.</param>
    /// <param name="contents">The contents of the file.</param>
    /// <param name="encoding">The encoding to write with.</param>
    /// <param name="containerId">Container to write to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WriteAllTextAsync(string path, string? contents, Encoding encoding, Guid containerId = default)
    {
        var bytes = encoding.GetBytes(contents ?? string.Empty);
        await this.WriteAllBytesAsync(path, bytes, containerId);
    }

    /// <summary>
    /// Write all bytes to a file.
    /// </summary>
    /// <param name="path">Path to write to.</param>
    /// <param name="bytes">The contents of the file.</param>
    /// <param name="containerId">Container to write to.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task WriteAllBytesAsync(string path, byte[] bytes, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        this.syncRoot.Enter();

        try
        {
            if (this.db != null)
            {
                await this.WriteOrUpdateToDatabaseAsync(path, bytes, containerId);
            }

            FilesystemUtil.WriteAllBytesSafe(path, bytes);
        }
        finally
        {
            this.syncRoot.Exit();
        }
    }

    /// <summary>
    /// Read all text from a file.
    /// If the file does not exist on the filesystem, a read is attempted from the backup. The backup is not
    /// automatically written back to disk, however.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <param name="forceBackup">Whether the backup of the file should take priority.</param>
    /// <param name="containerId">The container to read from.</param>
    /// <returns>All text stored in this file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the filesystem or in the backup.</exception>
    public Task<string> ReadAllTextAsync(string path, bool forceBackup = false, Guid containerId = default)
        => this.ReadAllTextAsync(path, Encoding.UTF8, forceBackup, containerId);

    /// <summary>
    /// Read all text from a file.
    /// If the file does not exist on the filesystem, a read is attempted from the backup. The backup is not
    /// automatically written back to disk, however.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <param name="encoding">The encoding to read with.</param>
    /// <param name="forceBackup">Whether the backup of the file should take priority.</param>
    /// <param name="containerId">The container to read from.</param>
    /// <returns>All text stored in this file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the filesystem or in the backup.</exception>
    public async Task<string> ReadAllTextAsync(string path, Encoding encoding, bool forceBackup = false, Guid containerId = default)
    {
        var bytes = await this.ReadAllBytesAsync(path, forceBackup, containerId);
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Read all text from a file, and automatically try again with the backup if the file does not exist or
    /// the <paramref name="reader"/> function throws an exception. If the backup read also throws an exception,
    /// or the file does not exist in the backup, a <see cref="FileReadException"/> is thrown.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <param name="reader">Lambda that reads the file. Throw here to automatically attempt a read from the backup.</param>
    /// <param name="containerId">The container to read from.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the filesystem or in the backup.</exception>
    /// <exception cref="FileReadException">Thrown here if the file and the backup fail their read.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ReadAllTextAsync(string path, Action<string> reader, Guid containerId = default)
        => await this.ReadAllTextAsync(path, Encoding.UTF8, reader, containerId);

    /// <summary>
    /// Read all text from a file, and automatically try again with the backup if the file does not exist or
    /// the <paramref name="reader"/> function throws an exception. If the backup read also throws an exception,
    /// or the file does not exist in the backup, a <see cref="FileReadException"/> is thrown.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <param name="encoding">The encoding to read with.</param>
    /// <param name="reader">Lambda that reads the file. Throw here to automatically attempt a read from the backup.</param>
    /// <param name="containerId">The container to read from.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the filesystem or in the backup.</exception>
    /// <exception cref="FileReadException">Thrown here if the file and the backup fail their read.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ReadAllTextAsync(string path, Encoding encoding, Action<string> reader, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        // TODO: We are technically reading one time too many here, if the file does not exist on the FS, ReadAllText
        // fails over to the backup, and then the backup fails to read in the lambda. We should do something about that,
        // but it's not a big deal. Would be nice if ReadAllText could indicate if it did fail over.

        // 1.) Try without using the backup
        try
        {
            var text = await this.ReadAllTextAsync(path, encoding, false, containerId);
            reader(text);
            return;
        }
        catch (FileNotFoundException)
        {
            // We can't do anything about this.
            throw;
        }
        catch (Exception ex)
        {
            Log.Verbose(ex, "First chance read from {Path} failed, trying backup", path);
        }

        // 2.) Try using the backup
        try
        {
            var text = await this.ReadAllTextAsync(path, encoding, true, containerId);
            reader(text);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Second chance read from {Path} failed, giving up", path);
            throw new FileReadException(ex);
        }
    }

    /// <summary>
    /// Read all bytes from a file.
    /// If the file does not exist on the filesystem, a read is attempted from the backup and the backup
    /// automatically written back to disk.
    /// </summary>
    /// <param name="path">The path to read from.</param>
    /// <param name="forceBackup">Whether the backup of the file should take priority.</param>
    /// <param name="containerId">The container to read from.</param>
    /// <returns>All bytes stored in this file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist on the filesystem or in the backup.</exception>
    public async Task<byte[]> ReadAllBytesAsync(string path, bool forceBackup = false, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        if (forceBackup)
        {
            var bytes = await this.ReadFromDatabaseAsync(path, containerId);

            // If the file doesn't exist, write it
            if (!File.Exists(path))
            {
                try
                {
                    FilesystemUtil.WriteAllBytesSafe(path, bytes);
                }
                catch (Exception e)
                {
                    Log.Warning(e, $"File \"{path}\" does not exist on the filesystem and could not be written");
                }
            }

            return bytes;
        }

        // If the file doesn't exist, immediately check the backup db
        if (!File.Exists(path))
        {
            var bytes = await this.ReadFromDatabaseAsync(path, containerId);

            try
            {
                FilesystemUtil.WriteAllBytesSafe(path, bytes);
            }
            catch (Exception e)
            {
                Log.Warning(e, $"File \"{path}\" does not exist on the filesystem and could not be written");
            }

            return bytes;
        }

        try
        {
            return File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to read file from disk, falling back to database");
            return await this.ReadFromDatabaseAsync(path, containerId);
        }
    }

    /// <inheritdoc/>
    void IInternalDisposableService.DisposeService()
    {
        this.db?.Dispose();
        this.db = null;
    }

    /// <summary>
    /// Replace possible non-portable parts of a path with portable versions.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    private static string NormalizePath(string path)
    {
        // Replace users folder
        var usersFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return path.Replace(usersFolder, "%USERPROFILE%");
    }

    /// <summary>
    /// Check if a file exists in the database.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="containerId">The container to check in.</param>
    /// <returns>True if the row exists.</returns>
    private bool ExistsInDatabase(string path, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        if (this.db == null)
            throw new FileNotFoundException("Backup database is not available", path);

        using var query = new TursoCommand(this.db);
        query.CommandText = "SELECT EXISTS(SELECT 1 FROM DbFile WHERE Path = $path AND ContainerId = $containerId LIMIT 1);";
        query.Parameters.AddWithValue("$path", NormalizePath(path));
        query.Parameters.AddWithValue("$containerId", containerId.ToString());
        var result = query.ExecuteScalar() as int?;
        return result == 1;
    }

    /// <summary>
    /// Check if a file exists in the database.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="containerId">The container to check in.</param>
    /// <returns>True if the row exists.</returns>
    private async Task<byte[]> ReadFromDatabaseAsync(string path, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        if (this.db == null)
            throw new FileNotFoundException("Backup database is not available", path);

        var normalizedPath = NormalizePath(path);
        await using var query = new TursoCommand(this.db);
        query.CommandText = "SELECT Data FROM DbFile WHERE Path = $path AND ContainerId = $containerId LIMIT 1;";
        query.Parameters.AddWithValue("$path", normalizedPath);
        query.Parameters.AddWithValue("$containerId", containerId);
        var result = await query.ExecuteScalarAsync();
        return result as byte[] ?? throw new FileNotFoundException("File not found", path);
    }

    /// <summary>
    /// Check if a file exists in the database.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="bytes">The contents of the file.</param>
    /// <param name="containerId">The container to check in.</param>
    /// <returns>True if the row exists.</returns>
    private async Task WriteOrUpdateToDatabaseAsync(string path, byte[] bytes, Guid containerId = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        if (this.db == null)
            throw new FileNotFoundException("Backup database is not available", path);

        await using var upsert = new TursoCommand(this.db);
        upsert.CommandText = @"
                    INSERT INTO DbFile(ContainerId, Path, Data)
                        VALUES ($containerId, $path, $data)
                    ON CONFLICT(ContainerId, Path)
                        DO UPDATE SET Data = excluded.Data;";
        upsert.Parameters.AddWithValue("$containerId", containerId.ToString());
        upsert.Parameters.AddWithValue("$path", NormalizePath(path));
        upsert.Parameters.AddWithValue("$data", bytes);
        await upsert.ExecuteNonQueryAsync();
    }

    private void SetupDb(string path)
    {
        this.db = new TursoConnection("Data Source=" + path);
        this.db.Open();

        this.db.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS DbFile (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ContainerId VARCHAR(32) NOT NULL,
                Path TEXT NOT NULL,
                Data BLOB NOT NULL,
                UNIQUE (ContainerId, Path)
            );
        ");

        this.db.ExecuteNonQuery(@"
            CREATE UNIQUE INDEX IF NOT EXISTS idx_DbFile_ContainerId_Path 
                ON DbFile (ContainerId, Path);
        ");
    }
}
