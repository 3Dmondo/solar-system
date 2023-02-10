namespace EpemeridesReader
{
  public static class PropertyExtensions
  {
    public static (double x, double dx) GetValue(this Property property, double time)
    {
      var length = property.Coefficients.Length;
      var t = new double[length];
      var dt = new double[length];
      t[0] = 1;
      t[1] = time;
      for (var n = 2; n < length; n++)
        t[n] = 2 * time * t[n - 1] + t[n - 2];


      dt[0] = 0;
      dt[1] = 1;
      dt[2] = 4 * time;
      for (var n = 3; n < length; n++)
        dt[n] = 2 * time * dt[n - 1] + 2 * t[n - 1] - dt[n - 2];

      double x = 0;
      double dx = 0;

      for (var i = 0; i < length; i++) {
        x += t[i] * property.Coefficients[i];
        dx += dt[i] * property.Coefficients[i];
      }

      dx *= 2.0 / property.TimeSpan;

      return (x, dx);
    }
  }
}
