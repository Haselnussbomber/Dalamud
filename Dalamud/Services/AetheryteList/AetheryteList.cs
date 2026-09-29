using System.Collections;
using System.Collections.Generic;

using Dalamud.IoC;
using Dalamud.IoC.Internal;

using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Dalamud.Services.AetheryteList;

/// <summary>
/// This collection represents the list of available Aetherytes in the Teleport window.
/// </summary>
[PluginInterface]
[ServiceManager.EarlyLoadedService]
#pragma warning disable SA1015
[ResolveVia<IAetheryteList>]
#pragma warning restore SA1015
internal sealed unsafe class AetheryteList : IServiceType, IAetheryteList
{
    [ServiceManager.ServiceDependency]
    private readonly ObjectTable.ObjectTable objectTable = Service<ObjectTable.ObjectTable>.Get();

    private readonly Telepo* telepoInstance = Telepo.Instance();

    [ServiceManager.ServiceConstructor]
    private AetheryteList()
    {
    }

    /// <inheritdoc/>
    public int Count => this.Length;

    /// <inheritdoc/>
    public int Length
    {
        get
        {
            if (this.objectTable.LocalPlayer == null)
                return 0;

            this.Update();

            if (this.telepoInstance->TeleportList.First == this.telepoInstance->TeleportList.Last)
                return 0;

            return this.telepoInstance->TeleportList.Count;
        }
    }

    /// <inheritdoc/>
    public IAetheryteEntry? this[int index]
    {
        get
        {
            if (index < 0 || index >= this.Length)
            {
                return null;
            }

            if (this.objectTable.LocalPlayer == null)
                return null;

            return new AetheryteEntry(this.telepoInstance->TeleportList[index]);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<IAetheryteEntry> GetEnumerator()
    {
        return new Enumerator(this);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private void Update()
    {
        // this is very very important as otherwise it crashes
        if (this.objectTable.LocalPlayer == null)
            return;

        this.telepoInstance->UpdateAetheryteList();
    }

    private struct Enumerator(AetheryteList aetheryteList) : IEnumerator<IAetheryteEntry>
    {
        private int index = -1;

        public IAetheryteEntry Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            if (++this.index < aetheryteList.Length)
            {
                this.Current = aetheryteList[this.index];
                return true;
            }

            this.Current = default;
            return false;
        }

        public void Reset()
        {
            this.index = -1;
        }

        public void Dispose()
        {
        }
    }
}
