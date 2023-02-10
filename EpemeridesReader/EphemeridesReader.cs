namespace EpemeridesReader
{
  public static class EphemeridesReader
  {
    public static Ephemerides AppendOrCreate(this Ephemerides ephemerides, Header header, Stream stream)
    {
      if (ephemerides == null)
        ephemerides = new Ephemerides();
      var streamReader = new StreamReader(stream);
      while (!streamReader.EndOfStream) {
        ephemerides.Blocks.Add(ReadBlock(header, streamReader));
      }
      return ephemerides;
    }

    private static Block ReadBlock(Header header, StreamReader streamReader)
    {
      var block = new Block();

      block.Series =
        new Series[header.SeriesDescriptions.Length];

      var blockHeader = streamReader.
        ReadNonEmptyLine().
        SplitInternal().
        Select(int.Parse).
        ToArray();
      var count = blockHeader[1];

      var values = streamReader.ReadDoubles(count).ToArray();
      var valuesSpan = new Span<double>(values);

      for (var currentSeries = 0;
        currentSeries < header.SeriesDescriptions.Length;
        currentSeries++) {

        var seriesDescription = header.SeriesDescriptions[currentSeries];

        var intervals = ReadIntervals(
          valuesSpan,
          header.DaysPerBlock,
          seriesDescription.StartOffset - 1,
          seriesDescription.NumberOfIntervals,
          seriesDescription.NumberOfProperties,
          seriesDescription.NumberOfCoefficients);

        block.Series[currentSeries] = new Series {
          Intervals = intervals
        };
      }
      return block;
    }

    private static Interval[] ReadIntervals(
      Span<double> valuesSpan,
      double daysPerBlock,
      int startOffset,
      int numberOfIntervals,
      int numberOfProperties,
      int numberOfCoefficients)
    {

      var length = numberOfCoefficients * numberOfProperties;
      var chebCoefficientIntervals = new Interval[numberOfIntervals];

      for (var interval = 0; interval < numberOfIntervals; interval++)
        chebCoefficientIntervals[interval] = 
          ReadInterval(
            valuesSpan.Slice(
              startOffset + interval * length, 
              length),
            daysPerBlock / numberOfIntervals,
            numberOfProperties,
            numberOfCoefficients);

      return chebCoefficientIntervals;
    }

    private static Interval ReadInterval(
      Span<double> valuesSpan,
      double daysPerInterval,
      int numberOfProperties,
      int numberOfCoefficients)
    {
      var interval = new Interval {
        Properties = new Property[numberOfProperties]
      };

      for (var property = 0; property < numberOfProperties; property++)
        interval.Properties[property] =
          ReadProperty(
            valuesSpan.Slice(
              property * numberOfCoefficients, 
              numberOfCoefficients),
              daysPerInterval);

      return interval;
    }

    private static Property ReadProperty(
      Span<double> valuesSpan,
      double daysPerInterval) => 
      new Property {
        TimeSpan = daysPerInterval,
        Coefficients = valuesSpan.ToArray()
      };
  }

}
