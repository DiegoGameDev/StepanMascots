using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using files =  System.IO.File;
using StbImageSharp;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

namespace StepanMascot
{
    public class mascotGraphicManager : GameWindow
    {
        int vao = 0;
        int vbo = 0;
        int ebo = 0;

        int Handle = 0;
        int Texture1 = 0;

        List<Vector4> sprites = new();

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

        int mascotSpriteSize = 32;

        ImageResult image;

        public static string MascotName { get; private set; } = "Stepan Mascot";

        public mascotGraphicManager(string mascotName, int w, int h, int spriteSize = 32) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = new OpenTK.Mathematics.Vector2i(w, h),
            Title = mascotName,
            TransparentFramebuffer = true,
        })
        {
            AlwaysOnTop = true;
            WindowBorder = WindowBorder.Hidden;
            MousePassthrough = true;
            mascotSpriteSize = spriteSize;
            MascotName = mascotName;
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 0f);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            Handle = createShader(@"C:\Users\DiegoMogger\Desktop\Programas\StepanSoftware\StepanMascot\StepanMascot\StepanMascot\GraphicsWindow\Shaders\default.vert",
                @"C:\Users\DiegoMogger\Desktop\Programas\StepanSoftware\StepanMascot\StepanMascot\StepanMascot\GraphicsWindow\Shaders\default.frag");

#if DEBUG
            Texture1 = createTexture(@"C:\Users\DiegoMogger\Desktop\Programas\StepanSoftware\StepanMascot\StepanMascot\StepanMascot\GraphicsWindow\MascotTexture\Stepan-Sheet.png");
#endif

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture1);

            start();
        }

        void start()
        {
            var screen = Monitors.GetPrimaryMonitor();
            int x = screen.WorkArea.Max.X - Size.X - 20;
            int y = screen.WorkArea.Max.Y - Size.Y - 20;

            Location = (x, y);

            generateSprites();
        }

        void generateSprites()
        {
            sprites = SpriteSheetManager.getSprites(image, mascotSpriteSize);
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

            GL.UseProgram(Handle);
            GL.BindVertexArray(vao);

            Matrix4 model = Matrix4.Identity;

            GL.UniformMatrix4(GL.GetUniformLocation(Handle, "model"), false, ref model);
            GL.Uniform1(GL.GetUniformLocation(Handle, "u_texture"), 0);

            GL.Uniform4(GL.GetUniformLocation(Handle, "instanceUV"), sprites[currentIndex]);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        int currentIndex = 0;
        float timeAccumulator = 0f;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            float time = (float)args.Time;
            timeAccumulator += time;

            float mileSecondsPerFrame = 1000f / 12f; // 12 FPS

            if (timeAccumulator >= mileSecondsPerFrame / 1000f)
            {
                timeAccumulator -= mileSecondsPerFrame / 1000f;
                currentIndex = (currentIndex + 1) % sprites.Count;
                if (currentIndex >= sprites.Count)
                {
                    currentIndex = 0;
                }
            }
        }

        int createTexture(string texturePath)
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

            return textureHandle;
        }

        int createShader(string vertexPath, string fragmentPath)
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

            int handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return handle;
        }
    }
}
