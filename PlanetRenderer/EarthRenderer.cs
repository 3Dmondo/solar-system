using OpenTK.Graphics.OpenGL4;
namespace PlanetRenderer
{
  internal class EarthRenderer : RockPlanetRenderer
  {
    private Texture EarthNight;
    private Texture EarthSpec;
    private Texture EarthCloud;
    override protected string VertexShaderName { get; } = "PlanetRenderer.Shaders.PlanetShader.vert";
    override protected string FragmentShaderName { get; } = "PlanetRenderer.Shaders.EarthShader.frag";
    override protected string SurfaceTextureName { get; } = "PlanetRenderer.Textures.EarthDay.jpg";
    protected override string NormalTextureName { get; } = "PlanetRenderer.Textures.EarthNormal.png";

    public EarthRenderer() : base()
    {
      EarthNight = Texture.LoadFromResource(
        "PlanetRenderer.Textures.EarthNight.jpg",
        TextureUnit.Texture2);
      PlanetShader.SetInt("NightTexture", 2);

      EarthSpec = Texture.LoadFromResource(
        "PlanetRenderer.Textures.EarthSpec.png",
        TextureUnit.Texture3);
      PlanetShader.SetInt("SpecularTexture", 3);

      EarthCloud = Texture.LoadFromResource(
        "PlanetRenderer.Textures.EarthClouds.jpg",
        TextureUnit.Texture4);
      PlanetShader.SetInt("CloudsTexture", 4);
    }

    protected override void UseTextures()
    {
      base.UseTextures();
      EarthNight.Use();
      EarthSpec.Use();
      EarthCloud.Use();
    }
  }
}
