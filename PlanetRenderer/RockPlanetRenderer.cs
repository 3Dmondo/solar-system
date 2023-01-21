namespace PlanetRenderer
{
  internal class RockPlanetRenderer : PlanetRenderer
  {
    private Texture NormalTexture;
    override protected string FragmentShaderName { get; } = "PlanetRenderer.Shaders.RockPlanetShader.frag";
    protected virtual string NormalTextureName { get; } = "PlanetRenderer.Textures.MarsNormal.png";
    public RockPlanetRenderer() : base() 
    {
      NormalTexture = Texture.LoadFromResource(
        NormalTextureName,
        OpenTK.Graphics.OpenGL4.TextureUnit.Texture1);
      PlanetShader.SetInt("NormalTexture", 1);
    }
    protected override void UseTextures()
    {
      base.UseTextures();
      NormalTexture.Use();
    }
  }
}
