using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using files =  System.IO.File;
using StbImageSharp;
using OpenTK.Windowing.Common;
using StepanMascot.Batch;
using StepanMascot.MascotManager;
using StepanMascot.StepanInterComunications;
using OpenTK.Mathematics;

namespace StepanMascot
{
    public class mascotGraphicManager : GameWindow
    {
        public static MascotsController mascotsController = new();

        public static bool running { get; private set; }

        float[] verts = new float[]
        {
            -1f, -1f, 0.0f, 0.0f, 0.0f, // Bottom-left vertex
             1f, -1f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
             1f,  1f, 0.0f, 1.0f, 1.0f, // Top-right vertex
            -1f,  1f, 0.0f, 0.0f, 1.0f  // Top-left vertex
        };

        uint[] indices = new uint[]
        {
            0, 1, 2, // First triangle
            2, 3, 0  // Second triangle
        };

        public static Dictionary<string, int> textureByPath = new();
        public static Dictionary<string, int> shaderByName = new();

        public static string MascotName { get; private set; } = "Stepan Mascot";

        public mascotGraphicManager(string mascotName, int w, int h, int spriteSize = 32) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Title = mascotName,
            TransparentFramebuffer = true,
        })
        {

            Size = new OpenTK.Mathematics.Vector2i(CurrentMonitor.HorizontalResolution, CurrentMonitor.VerticalResolution);
            AlwaysOnTop = true;
            WindowBorder = WindowBorder.Hidden;
            MousePassthrough = true;
            MascotName = mascotName;

            running = true;
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 0f);

            start();
        }

        void start()
        {
            var screen = Monitors.GetPrimaryMonitor();
            int x = screen.WorkArea.Max.X - Size.X - 20;
            int y = screen.WorkArea.Max.Y - Size.Y - 20;

            Location = (x, y);

            RenderBatch.Initialize();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            Matrix4 VP = Matrix4.CreateOrthographic(Size.X, Size.Y, 0.1f, 10f)* Matrix4.LookAt(Vector3.UnitZ * 2, Vector3.UnitZ * -1, Vector3.UnitY);

            RenderBatch.Render(VP);

            SwapBuffers();
        }

        int currentIndex = 0;
        float timeAccumulator = 0f;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            float time = (float)args.Time;

            mascotsController.UpdateMascot(time);

            while (ActionsQueue.shareActions.TryDequeue(out var action))
            {
                action();
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            running = false;
        }

        public static int createTexture(string texturePath, out ImageResult image)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);

            image = ImageResult.FromStream(files.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);

            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return textureHandle;
        }

        public static int createShader(string vertexPath, string fragmentPath, out int handle)
        {
            string vertexShaderSource = files.ReadAllText(vertexPath);
            string fragmentShaderSource = files.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            GL.CompileShader(vertexShader);
            GL.GetShaderInfoLog(vertexShader, out string vertexInfoLog);
            Console.WriteLine($"Vertex Shader Info Log: {vertexInfoLog}");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(fragmentShader);
            GL.GetShaderInfoLog(fragmentShader, out string fragmentInfoLog);
            Console.WriteLine($"Fragment Shader Info Log: {fragmentInfoLog}");

            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            Console.WriteLine(handle);

            return handle;
        }
    }
}
