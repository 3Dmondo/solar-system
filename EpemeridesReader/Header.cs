namespace EpemeridesReader
{
  public class Header
  {
    public int NCoeff { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public double DaysPerBlock { get; set; }
    public Dictionary<string,double> Constants { get; set; }
    public SeriesDescription[] SeriesDescriptions { get; set; } = null;
  }
}