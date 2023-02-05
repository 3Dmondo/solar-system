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
  }
}
