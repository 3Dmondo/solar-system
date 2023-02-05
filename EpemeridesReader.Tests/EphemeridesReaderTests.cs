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
    }
  }
}
