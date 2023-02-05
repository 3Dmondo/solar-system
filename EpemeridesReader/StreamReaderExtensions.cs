namespace EpemeridesReader
{
  internal static class StreamReaderExtensions
  {
    public static string AdvanceToLineContaining(this StreamReader streamReader, string value)
    {
      var line = streamReader.ReadNonEmptyLine();
      while ((string.IsNullOrWhiteSpace(line) || !line.Contains(value)) && 
        !streamReader.EndOfStream)
        line = streamReader.ReadLine();
      if (line is null) throw new ArgumentException();
      return line;
    }
    public static string ReadNonEmptyLine(this StreamReader streamReader)
    {
      var line = streamReader.ReadLine();
      while (string.IsNullOrWhiteSpace(line) && !streamReader.EndOfStream)
        line= streamReader.ReadLine();
      if (line is null) throw new ArgumentException();
      return line;
    }

    public static IEnumerable<double> ReadDoubles(this StreamReader streamReader, int count)
    {
      int returned = 0;
      while (returned < count) {
        foreach (var item in streamReader.
            ReadNonEmptyLine().
            SplitInternal().
            Select(s => s.ToDouble())) {
          if (returned++ < count)
            yield return item;
        } 
      }
    }
  }
}
