using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EpemeridesReader.Tests
{
  [TestClass]
  public class HeaderReaderTests
  {
    private static SeriesDescription[] ExpectedSeriesDescriptions = new[] {
      new SeriesDescription{
        StartOffset = 3,
        NumberOfCoefficients= 14,
        NumberOfIntervals = 4
      },
      new SeriesDescription{
        StartOffset = 171,
        NumberOfCoefficients= 10,
        NumberOfIntervals = 2
      },
      new SeriesDescription{
        StartOffset = 231,
        NumberOfCoefficients= 13,
        NumberOfIntervals = 2
      },
      new SeriesDescription{
        StartOffset = 309,
        NumberOfCoefficients= 11,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 342,
        NumberOfCoefficients= 8,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 366,
        NumberOfCoefficients= 7,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 387,
        NumberOfCoefficients= 6,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 405,
        NumberOfCoefficients= 6,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 423,
        NumberOfCoefficients= 6,
        NumberOfIntervals = 1
      },
      new SeriesDescription{
        StartOffset = 441,
        NumberOfCoefficients= 13,
        NumberOfIntervals = 8
      },
      new SeriesDescription{
        StartOffset = 753,
        NumberOfCoefficients= 11,
        NumberOfIntervals = 2
      },
      new SeriesDescription{
        StartOffset = 819,
        NumberOfCoefficients= 10,
        NumberOfIntervals = 4
      },
      new SeriesDescription{
        StartOffset = 899,
        NumberOfCoefficients= 10,
        NumberOfIntervals = 4
      },
      new SeriesDescription{
        StartOffset = 1019,
        NumberOfCoefficients= 0,
        NumberOfIntervals = 0
      },
      new SeriesDescription{
        StartOffset = 1019,
        NumberOfCoefficients= 0,
        NumberOfIntervals = 0
      },
    };

    [TestMethod]
    public void Read()
    {
      using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Data.De440Header));
      var header = HeaderReader.Read(stream);
      Assert.IsNotNull(header);
      Assert.AreEqual(1018, header.NCoeff);
      Assert.AreEqual(2287184.50, header.StartTime);
      Assert.AreEqual(2688976.50, header.EndTime);
      Assert.AreEqual(32.0, header.DaysPerBlock);
      Assert.IsNotNull(header.Constants);
      Assert.AreEqual(645, header.Constants.Count);
      Assert.AreEqual(0.149597870699999988E09, header.Constants["AU"]);
      Assert.AreEqual(0.813005682214972154E02, header.Constants["EMRAT"]);
      Assert.IsNotNull(header.SeriesDescriptions);
      CollectionAssert.AreEqual(ExpectedSeriesDescriptions, header.SeriesDescriptions);
    }
  }
}