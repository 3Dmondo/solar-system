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
        ephemerides.ChebCoefficientsBlocks.Add(ReadBlock(header, streamReader));
      }
      return ephemerides;
    }

    private static ChebCoefficientsBlock ReadBlock(Header header, StreamReader streamReader)
    {
      var block = new ChebCoefficientsBlock();
      block.ChebCoefficientSeries =
        new ChebCoefficientsSeries[header.SeriesDescriptions.Length];
      var blockHeader = streamReader.
        ReadNonEmptyLine().
        SplitInternal().
        Select(int.Parse).
        ToArray();
      var count = blockHeader[1];

      var values = streamReader.ReadDoubles(count).ToArray();
      var valuesSpan = new Span<double>(values);
      var currentIndex = 2;
      for (var currentSeries = 0;
        currentSeries < header.SeriesDescriptions.Length;
        currentSeries++) {
        var seriesDescription = header.SeriesDescriptions[currentSeries];
        var currentSeriesLength =
          (currentSeries < header.SeriesDescriptions.Length - 1 ?
          header.SeriesDescriptions[currentSeries + 1].StartOffset :
          count) - seriesDescription.StartOffset;
        var chebCoefficientIntervals = new ChebCoefficientsInterval[seriesDescription.NumberOfIntervals];
        var numberOfProperties =
          seriesDescription.NumberOfCoefficients > 0 && 
          seriesDescription.NumberOfIntervals > 0 ?
            currentSeriesLength /
            seriesDescription.NumberOfCoefficients /
            seriesDescription.NumberOfIntervals:
            0;
        for (var interval = 0; interval < seriesDescription.NumberOfIntervals; interval++) {
          var chebCoefficientInterval = new ChebCoefficientsInterval();
          chebCoefficientInterval.ChebCoefficients = new ChebCoefficients[numberOfProperties];
          for (var property = 0; property < numberOfProperties; property++) {
            var chebCoefficients = new ChebCoefficients {
              Coefficients = valuesSpan.Slice(currentIndex, seriesDescription.NumberOfCoefficients).ToArray()
            };
            chebCoefficientInterval.ChebCoefficients[property] = chebCoefficients;
            currentIndex += seriesDescription.NumberOfCoefficients;
          }
          chebCoefficientIntervals[interval] = chebCoefficientInterval;
        }
        block.ChebCoefficientSeries[currentSeries] = new ChebCoefficientsSeries {
          ChebCoefficientIntervals = chebCoefficientIntervals
        };
      }
      return block;
    }
  }
}
