namespace EpemeridesReader
{
  public class Header
  {
    private const StringSplitOptions splitOptions =
      StringSplitOptions.TrimEntries |
      StringSplitOptions.RemoveEmptyEntries;
    private const char SplitChar = ' ';
    private const string Group = "GROUP";
    private const string NCoeffName = "NCOEFF=";

    public int NCoeff { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public double DaysPerBlock { get; set; }
    public Dictionary<string,double>? Constants { get; set; }
    public SeriesDescription[] SeriesDescriptions { get; set; }

    public static Header Read(Stream stream)
    {
      var result = new Header();
      using var streamReader = new StreamReader(stream);
      result.NCoeff = GetNCoeff(streamReader);
      (result.StartTime,
        result.EndTime,
        result.DaysPerBlock) = GetGroup1030(streamReader);
      var constantKeys = GetGroup1040(streamReader);
      var constantValues = GetGroup1041(streamReader);
      result.Constants = constantKeys.
        Zip(constantValues).
        ToDictionary(kv => kv.First, kv => kv.Second);
      result.SeriesDescriptions = GetGroup1050(streamReader).ToArray();
      return result;
    }

    private static int GetNCoeff(StreamReader streamReader)
    {
      var line = streamReader.AdvanceToLineContaining(NCoeffName);
      var lineWords = line.Split(SplitChar, splitOptions);
      var nCoeff = lineWords[Array.IndexOf(lineWords, NCoeffName) + 1];
      return int.Parse(nCoeff);
    }

    private static void AdvanceToGroup(StreamReader streamReader, string groupName)
    {
      var lineWords = streamReader.
        AdvanceToLineContaining(Group).
        Split(SplitChar, splitOptions);
      while (lineWords[1] != groupName && !streamReader.EndOfStream) {
        lineWords = streamReader.
          AdvanceToLineContaining(Group).
          Split(SplitChar, splitOptions);
      }
    }
    private static (double, double, double) GetGroup1030(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1030");
      var lineWords = streamReader.
        ReadNonEmptyLine().
        Split(SplitChar, splitOptions);
      return (
        double.Parse(lineWords[0]),
        double.Parse(lineWords[1]),
        double.Parse(lineWords[2]));
    }

    private static IEnumerable<String> GetGroup1040(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1040");
      var count = int.Parse(streamReader.ReadNonEmptyLine());
      var result = new List<String>();
      while (result.Count < count) {
        result.AddRange(streamReader.
          ReadNonEmptyLine().
          Split(SplitChar, splitOptions));
      }
      return result;
    }

    private static IEnumerable<double> GetGroup1041(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1041");
      var count = int.Parse(streamReader.ReadNonEmptyLine());
      var result = new List<double>();
      while (result.Count < count) {
        result.AddRange(streamReader.
          ReadNonEmptyLine().
          Split(SplitChar, splitOptions).
          Select(StringExtensions.ToDouble));
      }
      return result;
    }

    private static IEnumerable<SeriesDescription> GetGroup1050(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1050");

      var startOffset = streamReader.
        ReadNonEmptyLine().
        Split(SplitChar, splitOptions).
        Select(int.Parse);

      var numberOfCoefficients = streamReader.
        ReadNonEmptyLine().
        Split(SplitChar, splitOptions).
        Select(int.Parse);

      var numberOfIntervals = streamReader.
        ReadNonEmptyLine().
        Split(SplitChar, splitOptions).
        Select(int.Parse);

      return startOffset.
        Zip(numberOfCoefficients, numberOfIntervals).
        Select(x=> new SeriesDescription {
          StartOffset = x.First,
          NumberOfCoefficients = x.Second,
          NumberOfIntervals = x.Third
        });
    }
  }
}