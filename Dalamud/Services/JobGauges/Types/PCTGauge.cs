using Dalamud.Services.JobGauges.Enums;

namespace Dalamud.Services.JobGauges.Types;

/// <summary>
/// In-Memory PCT job gauge.
/// </summary>
public unsafe class PCTGauge : JobGaugeBase<FFXIVClientStructs.FFXIV.Client.Game.Gauge.PictomancerGauge>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PCTGauge"/> class.
    /// </summary>
    /// <param name="address">Address of the job gauge.</param>
    internal PCTGauge(IntPtr address)
        : base(address)
    {
    }

    /// <summary>
    /// Gets the use of subjective pallete.
    /// </summary>
    public byte PalleteGauge => this.Struct->PalleteGauge;

    /// <summary>
    /// Gets the amount of paint the player has.
    /// </summary>
    public byte Paint => this.Struct->Paint;

    /// <summary>
    /// Gets a value indicating whether a creature motif is drawn.
    /// </summary>
    public bool CreatureMotifDrawn => this.Struct->CreatureMotifDrawn;

    /// <summary>
    /// Gets a value indicating whether a weapon motif is drawn.
    /// </summary>
    public bool WeaponMotifDrawn => this.Struct->WeaponMotifDrawn;

    /// <summary>
    /// Gets a value indicating whether a landscape motif is drawn.
    /// </summary>
    public bool LandscapeMotifDrawn => this.Struct->LandscapeMotifDrawn;

    /// <summary>
    /// Gets a value indicating whether a moogle portrait is ready.
    /// </summary>
    public bool MooglePortraitReady => this.Struct->MooglePortraitReady;

    /// <summary>
    /// Gets a value indicating whether a madeen portrait is ready.
    /// </summary>
    public bool MadeenPortraitReady => this.Struct->MadeenPortraitReady;

    /// <summary>
    /// Gets which creature flags are present.
    /// </summary>
    public CreatureFlags CreatureFlags => (CreatureFlags)this.Struct->CreatureFlags;

    /// <summary>
    /// Gets which canvas flags are present.
    /// </summary>
    public CanvasFlags CanvasFlags => (CanvasFlags)this.Struct->CanvasFlags;
}
