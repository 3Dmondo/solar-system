namespace EpemeridesReader
{
  public static class StringExtensions
  {
    public static double ToDouble(this string s) =>
      double.Parse(s.Replace('D', 'E'));

    public static string[] SplitInternal(this string s) =>
      s.Split(Constants.SplitChar, Constants.splitOptions);
  }
}
