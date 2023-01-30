using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PlanetRenderer
{
  internal class PlanetRenderer2
  {
    private const float TwoPi = 2f * (float)Math.PI;
    private readonly float[] Vertices;
    private int VertexBufferObject;
    private int VertexArrayObject;
    private Texture Surface;
    private float RotationAngle = 0;

    protected Shader PlanetShader;
    protected virtual string VertexShaderName { get; } = "PlanetRenderer.Shaders.ImpostorSphere.vert";
    protected virtual string FragmentShaderName { get; } = "PlanetRenderer.Shaders.ImpostorSphere.frag";
    protected virtual string SurfaceTextureName { get; } = "PlanetRenderer.Textures.EarthDay.jpg";
    protected virtual float RotationSpeed { get; } = 1f / 3600f;

    public PlanetRenderer2()
    {
      PlanetShader = new Shader(VertexShaderName, FragmentShaderName);
      Vertices = new float[] {
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f
      };
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
      Surface = Texture.LoadFromResource(
        SurfaceTextureName,
        TextureUnit.Texture0);
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
      GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
      GL.Disable(EnableCap.PointSprite);
      GL.Disable(EnableCap.VertexProgramPointSize);
    }

    private void BindVertexArrays()
    {
      GL.BindVertexArray(VertexArrayObject);
    }

    protected virtual void SetShaderUniforms(
      Camera camera,
      Vector3 LightPosition)
    {
      //GL.Enable(EnableCap.DepthTest);
      //GL.DepthFunc(DepthFunction.Less);

      var planetAxis = new Vector3(1.0f, 0.1f, 0.0f);
      planetAxis.Normalize();
      var norm = Vector3.Cross(planetAxis, new Vector3(0, 0, -1));
      var binorm = Vector3.Cross(norm, planetAxis);
      var orientation = new Matrix3(planetAxis, norm, binorm);


      PlanetShader.SetMatrix3(
        "orientation",
        orientation);

      PlanetShader.SetMatrix4(
        "view",
        camera.GetViewMatrix());

      PlanetShader.SetMatrix4(
        "cameraToClipMatrix",
        camera.GetProjectionMatrix());

      PlanetShader.SetVector3("cameraSpherePos", (new Vector4(0.0f, 0, 0, 1) * camera.GetViewMatrix()).Xyz);
      //PlanetShader.SetVector3("lightPos", (new Vector4(LightPosition, 1) * camera.GetViewMatrix()).Xyz);
      PlanetShader.SetVector3("lightPos", LightPosition);

      PlanetShader.SetFloat("rotation", RotationAngle);
    }
    protected virtual void UseTextures()
    {
      Surface.Use();
    }
  }
}
