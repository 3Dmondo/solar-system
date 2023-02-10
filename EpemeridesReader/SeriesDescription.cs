namespace EpemeridesReader
{
  public record struct SeriesDescription
  {
    public int StartOffset { get; init; }
    public int NumberOfCoefficients { get; init; }
    public int NumberOfIntervals { get; init; }
    public int Length { get; init; }
    public int NumberOfProperties { get; init; }
  }
}
