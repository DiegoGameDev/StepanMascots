using OpenTK.Graphics.OpenGL4;
using StepanMascot.GraphicsWindow.Assets;

namespace StepanMascot.GraphicsWindow
{
    public class Material
    {
        public int shaderProgram { get; private set; }
        public int texture { get; private set; }
        public Material(int shaderProgram, int textureUnit)
        {
            this.shaderProgram = shaderProgram;
            this.texture = textureUnit;
        }

        public void Bind()  
        {
            GL.UseProgram(shaderProgram);

            int unit = AssetsManager.LinkTexture(texture);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "u_texture"), unit);
        }
    }
}
