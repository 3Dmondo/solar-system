using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpemeridesReader;
using System.ComponentModel.DataAnnotations;

namespace EpemeridesReader.Tests
{
  [TestClass]
  public class TestPo
  {
    const string TestpoName = "testpo";
    const string HeaderName = "header";
    const string EphName = "asc";

    const string DE440 = "D:\\de440";
    const string ext440 = "440";

    [DataTestMethod]
    [DataRow(DE440, ext440)]
    public void Test(string directory, string extension)
    {
      var headerFileName = Path.Combine(directory, $"{HeaderName}.{extension}");
      using var headerFileStream = new FileStream(headerFileName, FileMode.Open, FileAccess.Read);
      var header = HeaderReader.Read(headerFileStream);

      var files = Directory.
        GetFiles(directory).
        Where(x =>
          x.StartsWith(Path.Combine(directory, EphName)) &&
          x.EndsWith(extension)).
        Order();

      var ephemerides = new Ephemerides();

      foreach (var file in files) {
        using var ephStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        ephemerides.AppendOrCreate(header, ephStream);
      }


      var testpoSileName = Path.Combine(directory, $"{TestpoName}.{extension}");
      using var testpoStream = new FileStream(testpoSileName, FileMode.Open, FileAccess.Read);
      using var testpoReader = new StreamReader(testpoStream);
      var line = testpoReader.ReadLine();
      while (!line.StartsWith("EOT")) line = testpoReader.ReadLine();
      while (!testpoReader.EndOfStream) {
        line = testpoReader.ReadLine();
        if (string.IsNullOrEmpty(line)) continue;
        var testData = line.SplitInternal();
        var test = new Test {
          Date = DateOnly.Parse(testData[1]),
          Jed = double.Parse(testData[2]),
          Target = int.Parse(testData[3]),
          Center = int.Parse(testData[4]),
          Coordinate = int.Parse(testData[5]),
          ExpectedValue = double.Parse(testData[6])
        };

        if (header.SeriesDescriptions[test.Target - 1].NumberOfIntervals == 0) continue;
        if (test.Center > 0 && header.SeriesDescriptions[test.Center - 1].NumberOfIntervals == 0) continue;

        var daysFromStartTime = test.Jed - header.StartTime;
        var blockIndex = Math.Floor(daysFromStartTime / header.DaysPerBlock);
        var blockStartTime = blockIndex * header.DaysPerBlock;

        var daysFromBlockStartTime = daysFromStartTime - blockStartTime;
        var intervalLength = header.DaysPerBlock / header.SeriesDescriptions[test.Target - 1].NumberOfIntervals;
        var intervalIdex = Math.Floor(daysFromBlockStartTime / intervalLength);
        var intervalStartTime = intervalIdex * intervalLength;
        var daysFromIntervalStartTime = daysFromBlockStartTime - intervalStartTime;
        var timeInInterval = daysFromIntervalStartTime / intervalLength * 2.0 - 1.0;
        Assert.IsTrue(timeInInterval >= -1.0 && timeInInterval <= 1.0);

        var numberOfProperties = header.SeriesDescriptions[test.Target - 1].NumberOfProperties;
        var targetValues = ephemerides.
          Blocks[(int)blockIndex].
          Series[test.Target - 1].
          Intervals[(int)intervalIdex].
          Properties[(test.Coordinate - 1) % numberOfProperties].
          GetValue(timeInInterval);
        var targetValue = targetValues.x;
        if (test.Coordinate > numberOfProperties)
          targetValue = targetValues.dx;

        var centerValue = 0.0;
        if (test.Center > 0) {
          numberOfProperties = header.SeriesDescriptions[test.Center - 1].NumberOfProperties;
          intervalLength = header.DaysPerBlock / header.SeriesDescriptions[test.Center - 1].NumberOfIntervals;
          intervalIdex = Math.Floor(daysFromBlockStartTime / intervalLength);
          intervalStartTime = intervalIdex * intervalLength;
          daysFromIntervalStartTime = daysFromBlockStartTime - intervalStartTime;
          timeInInterval = daysFromIntervalStartTime / intervalLength * 2.0 - 1.0;
          Assert.IsTrue(timeInInterval >= -1.0 && timeInInterval <= 1.0);
          var centerValues = ephemerides.
            Blocks[(int)blockIndex].
            Series[test.Center - 1].
            Intervals[(int)intervalIdex].
            Properties[(test.Coordinate - 1) % numberOfProperties].
            GetValue(timeInInterval);
          centerValue = centerValues.x;
          if (test.Coordinate > numberOfProperties)
            centerValue = centerValues.dx;
        }

        var testValue = (targetValue - centerValue)/header.Constants["AU"];
        //Assert.AreEqual(test.ExpectedValue, testValue, 1e-15);
      }
    }
  }

  public class Test
  {
    public DateOnly Date { get; set; }
    public double Jed { get; set; }
    public int Target { get; set; }
    public int Center { get; set; }
    public int Coordinate { get; set; }
    public double ExpectedValue { get; set; }
  }
}
