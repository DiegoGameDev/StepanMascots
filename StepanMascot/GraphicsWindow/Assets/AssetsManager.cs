using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.GraphicsWindow.Assets
{
    internal static class AssetsManager
    {
        public static Dictionary<string, int> shaderByName = new();

        public static Dictionary<string, TextureAsset> textureByName = new();
        static Dictionary<int, TextureUnit> unitByTexture = new();


        public static int TextureLinkedCount { get; private set; }
        static Dictionary<TextureUnit, bool> hasTexture = new(10)
        {
            
        };

        public static void Init()
        {
            hasTexture.Add(TextureUnit.Texture0, false);
            hasTexture.Add(TextureUnit.Texture1, false);
            hasTexture.Add(TextureUnit.Texture2, false);
            hasTexture.Add(TextureUnit.Texture3, false);
            hasTexture.Add(TextureUnit.Texture4, false);
            hasTexture.Add(TextureUnit.Texture5, false);
            hasTexture.Add(TextureUnit.Texture6, false);
            hasTexture.Add(TextureUnit.Texture7, false);
            hasTexture.Add(TextureUnit.Texture8, false);
            hasTexture.Add(TextureUnit.Texture9, false);
        }

        public static bool TryGetShader(string name, out int shader)
        {
            if (shaderByName.TryGetValue(name, out shader))
            {
                return true;
            }

            shader = -1;
            return false;
        }
        public static bool TryGetTexture(string name, out TextureAsset textureAsset) => textureByName.TryGetValue(name, out textureAsset);

        public static void AddTexture(string name, TextureAsset textureAsset)
        {
            textureByName.TryAdd(name, textureAsset);
        }

        public static void AddShader(string name, int shader)
        {
            shaderByName.TryAdd(name, shader);
        }

        public static int LinkTexture(int texture, int index = -1)
        {
            if (unitByTexture.ContainsKey(texture))
            {
                index = (int)unitByTexture[texture] - 33984;
                return index;
            }

            int index_enum = 0;
            if (index == -1) // se indice não foi definido, implementar no próximo
            {
                // a primeira unidade é 33984

                //o maximo vai ser 16, pois algumas maquinas nao pega 32

                TextureUnit emptyUnit = TextureUnit.Texture0;

                index = 0;
                bool found = false;
                foreach (var i in hasTexture)
                {
                    index++;
                    if (!i.Value)
                    {
                        found = true;
                        emptyUnit = i.Key;
                        GL.ActiveTexture(i.Key);
                        GL.BindTexture(TextureTarget.Texture2D, texture);
                        unitByTexture.Add(texture, emptyUnit);
                        break;
                    }
                }

                return index;
            }

            index_enum = 33984;
            var unit = (TextureUnit)index_enum + index;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            hasTexture[unit] = true;
            return index;
        }
    }

    public sealed record TextureAsset(int id, ImageResult image);
}
