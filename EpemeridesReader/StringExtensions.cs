namespace EpemeridesReader
{
  internal static class StringExtensions
  {
    public static double ToDouble(this string s) =>
      double.Parse(s.Replace('D', 'E'));
  }
}
