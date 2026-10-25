using Newtonsoft.Json.Linq;
using StbImageSharp;
using StepanMascot.Batch;
using StepanMascot.GraphicsWindow.Assets;
using StepanMascot.GraphicsWindow.Util;
using StepanMascot.ShadersServices;
using System.Numerics;

namespace StepanMascot.MascotManager
{
    public class MascotsController
    {
        public List<Mascot> mascots { get; private set; } = new List<Mascot>();

        public void CreateMascot(JObject json)
        {
            //deserialization
            string name = json.Property("mascotName").Value.ToString();
            string shader = json.Property("shaderName").Value.ToString();
            string texturePath = json.Property("texturePath").Value.ToString();
            int cell = json["spriteSize"].Value<int>();

            Vector3 position, scale;

            position.X = json["position"]["x"].Value<float>();
            position.Y = json["position"]["y"].Value<float>();
            position.Z = json["position"]["z"].Value<float>();

            scale.X = json["scale"]["x"].Value<float>();
            scale.Y = json["scale"]["y"].Value<float>();
            scale.Z = json["scale"]["z"].Value<float>();

            //deserialization

            var mascotsSameName = mascots.Where(x => x.mascotName == name);
            if (mascotsSameName != null)
            {
                string number = $"[{mascotsSameName.Count()}]";
                name = string.Concat(name, number);
            }

            shader = string.IsNullOrEmpty(shader) ? "Default" : shader;

            Mascot mascot = new(name)
            {
                shaderName = shader,
                texturePath = texturePath,
                cell = cell,
                position = position,
                scale = scale
            };

            int mascotShader, mascotTexture;

            if (!AssetsManager.TryGetShader(shader, out mascotShader))
                mascotShader = ShaderFilesService.ShaderFromFile(mascot.shaderName);

            if (!AssetsManager.TryGetTexture(texturePath, out TextureAsset textureAsset))
            {
                mascotTexture = mascotGraphicManager.createTexture(mascot.texturePath, out ImageResult image);
                textureAsset = new TextureAsset(mascotTexture, image);
                mascot.mascotImage = textureAsset.image;
            }
            else
                mascot.mascotImage = textureAsset.image;

            var sprites = SpriteSheetManager.getSprites(mascot.mascotImage, cell);
            mascot.sprites = sprites.Select(x => x.ConvertOpenTKVector4ToSystemNumericsVector4()).ToList();

            AssetsManager.AddShader(mascot.shaderName, mascotShader);
            AssetsManager.AddTexture(mascot.texturePath, textureAsset);

            mascot.mascotMaterial = new(mascotShader, textureAsset.id);

            mascots.Add(mascot);

            RenderBatch.AddBatch(mascot);

            mascot.animator = new();
        }

        public void AddMascot(Mascot mascot)
        {
            var mascotsSameName = mascots.Where(x => x.mascotName == mascot.mascotName);
            if (mascotsSameName != null)
            {
                string number = $"[{mascotsSameName.Count()}]";
                mascot.mascotName = string.Concat(mascot.mascotName, number);
            }
            // o mascote apenas guarda o nome da textura e do shader
            // o metodo deve realizar uma procura em um pasta determinada, buscando o shader com o nome equivalente
            // o mesmo para texturas, caso não tiver texturas, usar o padrão do programa


            int mascotShader, mascotTexture;

            if (!AssetsManager.TryGetShader(mascot.shaderName, out mascotShader))
                mascotShader = ShaderFilesService.ShaderFromFile(mascot.shaderName);

            if (!AssetsManager.TryGetTexture(mascot.texturePath, out TextureAsset textureAsset))
            {
                mascotTexture = mascotGraphicManager.createTexture(mascot.texturePath, out ImageResult image);
                textureAsset = new TextureAsset(mascotTexture, image);
                mascot.mascotImage = textureAsset.image;
            }

            AssetsManager.AddShader(mascot.shaderName, mascotShader);
            AssetsManager.AddTexture(mascot.texturePath, textureAsset);

            mascot.mascotMaterial = new(mascotShader, textureAsset.id);

            mascots.Add(mascot);

            RenderBatch.AddBatch(mascot);
        }

        public void RemoveMascot(Mascot m)
        {
            mascots.Remove(m);
            m.objectBatch.RemoveMascot(m);
        }

        public void RenderExistentMascot(int mascotIdentifier)
        {
            Mascot? mascot = mascots[mascotIdentifier];

            if (mascot == null)
                return;

            RenderBatch.AddBatch(mascot);
        }

        public void UnRenderMascot(int id)
        {
            Mascot? mascot = mascots[id];

            if (mascot == null)
                return;

            mascot.objectBatch.RemoveMascot(mascot);
        }

        public void UpdateMascot(float dt)
        {
            for (int i = 0; i < mascots.Count; i++)
            {
                mascots[i]?.mascotUpdate(dt);
            }
        }
    }
}
