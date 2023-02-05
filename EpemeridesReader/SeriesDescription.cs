namespace EpemeridesReader
{
  public record struct SeriesDescription
  {
    public int StartOffset { get; init; }
    public int NumberOfCoefficients { get; init; }
    public int NumberOfIntervals { get; init; }
  }
}
