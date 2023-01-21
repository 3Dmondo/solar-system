using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PlanetRenderer
{
  internal class PlanetRenderer
  {
    private const float TwoPi = 2f * (float)Math.PI;
    protected Shader PlanetShader;
    private readonly float[] Vertices;
    private int VertexBufferObject;
    private int VertexArrayObject;
    private Texture Surface;
    private float RotationAngle = 0;
    protected virtual string VertexShaderName { get; } = "PlanetRenderer.Shaders.PlanetShader.vert";
    protected virtual string FragmentShaderName { get; } = "PlanetRenderer.Shaders.FlatPlanetShader.frag";
    protected virtual string SurfaceTextureName { get; } = "PlanetRenderer.Textures.MarsSurface.png";
    protected virtual float RotationSpeed { get; } = 1f / 3600f;

    public PlanetRenderer()
    {
      PlanetShader = new Shader(VertexShaderName, FragmentShaderName);
      Vertices = new float[] { 0f, 0f, 0f };
      VertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(
        BufferTarget.ArrayBuffer,
        VertexBufferObject);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        Vertices.Length * sizeof(float),
        Vertices,
        BufferUsageHint.StreamDraw); VertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(VertexArrayObject);
      GL.VertexAttribPointer(
        0,
        3,
        VertexAttribPointerType.Float,
        false,
        3 * sizeof(float),
        0);
      GL.EnableVertexAttribArray(0);
      Surface = Texture.LoadFromResource(SurfaceTextureName);
      Surface.Use(TextureUnit.Texture0);
      PlanetShader.SetInt("SurfaceTexture", 0);
    }

    public virtual void UpdateFrame(bool pause)
    {
      if (!pause)
        RotationAngle += RotationSpeed;
      if (RotationAngle > TwoPi)
        RotationAngle -= TwoPi;
    }

    public virtual void RenderFrame(
      Camera camera,
      Vector3 LightPosition)
    {
      GL.Enable(EnableCap.PointSprite);
      GL.Enable(EnableCap.VertexProgramPointSize);
      UseTextures();
      PlanetShader.Use();
      SetShaderUniforms(camera, LightPosition);
      BindVertexArrays();
    }

    private void BindVertexArrays()
    {
      GL.BindVertexArray(VertexArrayObject);
      GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length / 3);
      GL.Disable(EnableCap.PointSprite);
      GL.Disable(EnableCap.VertexProgramPointSize);
    }

    protected virtual void SetShaderUniforms(
      Camera camera,
      Vector3 LightPosition)
    {
      PlanetShader.SetMatrix4(
        "model_view_projection",
        Matrix4.Identity *
        camera.GetViewMatrix() *
        camera.GetProjectionMatrix());
      PlanetShader.SetVector3("camera_pos", camera.Position);
      PlanetShader.SetVector3("lightPos", LightPosition);
      PlanetShader.SetVector3("rotation", new Vector3(RotationAngle, 0, 0));
    }

    protected virtual void UseTextures()
    {
      Surface.Use(TextureUnit.Texture0);
    }
  }
}
