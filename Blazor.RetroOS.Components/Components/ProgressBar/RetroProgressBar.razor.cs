using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public partial class RetroProgressBar : RetroComponentBase
{
    /// <summary>
    /// Current value, between 0 and <see cref="Max"/> (clamped).
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Maximum value (100 by default).
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    internal double ClampedValue => Math.Clamp(Value, 0, Max <= 0 ? 0 : Max);

    /// <summary>
    /// Filled part, in percent (0 to 100).
    /// </summary>
    public double Percent => Max <= 0 ? 0 : ClampedValue / Max * 100;
}
