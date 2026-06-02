namespace CharacterManager.Core.Characters;

public sealed record ProgressTrack(string Name, int Progress, int MaxProgress)
{
    public double FilledRatio => MaxProgress <= 0 ? 0 : Math.Clamp((double)Progress / MaxProgress, 0, 1);
}
