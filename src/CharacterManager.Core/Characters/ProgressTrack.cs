namespace CharacterManager.Core.Characters;

public sealed record ProgressTrack(
    string Id,
    string Name,
    string Type,
    string Rank,
    int Ticks,
    int MaxTicks,
    string Status,
    string SharedScope,
    string Notes)
{
    public const int DefaultMaxTicks = 40;

    public int ProgressScore => Math.Clamp(Ticks, 0, MaxTicks) / 4;

    public double FilledRatio => MaxTicks <= 0 ? 0 : Math.Clamp((double)Ticks / MaxTicks, 0, 1);
}
