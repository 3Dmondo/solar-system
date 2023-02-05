namespace EpemeridesReader
{
  public static class HeaderReader
  {
    private const string Group = "GROUP";
    private const string NCoeffName = "NCOEFF=";

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
      var lineWords = line.SplitInternal();
      var nCoeff = lineWords[Array.IndexOf(lineWords, NCoeffName) + 1];
      return int.Parse(nCoeff);
    }

    private static void AdvanceToGroup(StreamReader streamReader, string groupName)
    {
      var lineWords = streamReader.
        AdvanceToLineContaining(Group).
        SplitInternal();
      while (lineWords[1] != groupName && !streamReader.EndOfStream) {
        lineWords = streamReader.
          AdvanceToLineContaining(Group).
          SplitInternal();
      }
    }
    private static (double, double, double) GetGroup1030(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1030");
      var lineWords = streamReader.
        ReadNonEmptyLine().
        SplitInternal();
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
          SplitInternal());
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
          SplitInternal().
          Select(StringExtensions.ToDouble));
      }
      return result;
    }

    private static IEnumerable<SeriesDescription> GetGroup1050(StreamReader streamReader)
    {
      AdvanceToGroup(streamReader, "1050");

      var startOffset = streamReader.
        ReadNonEmptyLine().
        SplitInternal().
        Select(int.Parse);

      var numberOfCoefficients = streamReader.
        ReadNonEmptyLine().
        SplitInternal().
        Select(int.Parse);

      var numberOfIntervals = streamReader.
        ReadNonEmptyLine().
        SplitInternal().
        Select(int.Parse);

      return startOffset.
        Zip(numberOfCoefficients, numberOfIntervals).
        Select(x => new SeriesDescription {
          StartOffset = x.First,
          NumberOfCoefficients = x.Second,
          NumberOfIntervals = x.Third
        });
    }
  }
}
