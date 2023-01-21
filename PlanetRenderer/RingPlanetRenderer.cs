namespace PlanetRenderer
{
  internal class RingPlanetRenderer : PlanetRenderer
  {
    private Texture RingTexture;
    protected virtual string RingTextureName { get; } = "PlanetRenderer.Textures.SaturnRing.png";
    override protected  string FragmentShaderName { get; } = "PlanetRenderer.Shaders.RingPlanetShader.frag";
    override protected string SurfaceTextureName { get; } = "PlanetRenderer.Textures.Saturn.jpg";

    protected float InnerRadius = 50f;
    protected float OuterRadius = 100f;
    public RingPlanetRenderer() : base()
    {
      RingTexture = Texture.LoadFromResource(
        RingTextureName,
        OpenTK.Graphics.OpenGL4.TextureUnit.Texture1);
    }
    protected override void UseTextures()
    {
      base.UseTextures();
      RingTexture.Use();
    }
  }
}
