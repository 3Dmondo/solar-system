using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
namespace PlanetRenderer
{
  internal class EarthRenderer : PlanetRenderer
  {
    private Texture EarthNight;
    private Texture EarthNormal;
    private Texture EarthSpec;
    private Texture EarthCloud;
    override protected string VertexShaderName { get; } = "PlanetRenderer.Shaders.PlanetShader.vert";
    override protected string FragmentShaderName { get; } = "PlanetRenderer.Shaders.EarthShader.frag";
    override protected string SurfaceTextureName { get; } = "PlanetRenderer.Textures.EarthDay.jpg";

    public EarthRenderer() : base()
    {
      EarthNight = Texture.LoadFromResource("PlanetRenderer.Textures.EarthNight.jpg");
      EarthNight.Use(TextureUnit.Texture1);
      EarthNormal = Texture.LoadFromResource("PlanetRenderer.Textures.EarthNormal.png");
      EarthNormal.Use(TextureUnit.Texture2);
      EarthSpec = Texture.LoadFromResource("PlanetRenderer.Textures.EarthSpec.png");
      EarthSpec.Use(TextureUnit.Texture3);
      EarthCloud = Texture.LoadFromResource("PlanetRenderer.Textures.EarthClouds.jpg");
      EarthCloud.Use(TextureUnit.Texture4);
      PlanetShader.SetInt("texture1", 1);
      PlanetShader.SetInt("texture2", 2);
      PlanetShader.SetInt("texture3", 3);
      PlanetShader.SetInt("texture4", 4);
    }

    protected override void UseTextures()
    {
      base.UseTextures();
      EarthNight.Use(TextureUnit.Texture1);
      EarthNormal.Use(TextureUnit.Texture2);
      EarthSpec.Use(TextureUnit.Texture3);
      EarthCloud.Use(TextureUnit.Texture4);
    }
  }
}
