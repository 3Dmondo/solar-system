using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EpemeridesReader.Tests
{
  [TestClass]
  public class EphemeridesReaderTests
  {
    [TestMethod]
    public void AppendOrCreate()
    {
      using var HeaderStream = new MemoryStream(Encoding.UTF8.GetBytes(Data.De440Header));
      using var EphemeridesStream = new MemoryStream(Encoding.UTF8.GetBytes(Data.Ascp01550Cut));
      var header = HeaderReader.Read(HeaderStream);
      var ephemerides = EphemeridesReader.AppendOrCreate(new Ephemerides(), header, EphemeridesStream);
      Assert.IsNotNull(ephemerides);
      Assert.AreEqual(
        -0.1242590853666514E-10, 
        ephemerides.
          Blocks[0].
          Series[12].
          Intervals[3].
          Properties[2].
          Coefficients[9]);
      Assert.AreEqual(
        -0.1348070957759274E+08,
        ephemerides.
          Blocks[1].
          Series[0].
          Intervals[0].
          Properties[0].
          Coefficients[0]);
    }
  }
}
