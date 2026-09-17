using StbImageSharp;
using OpenTK.Mathematics;
using StepanMascot.GraphicsWindow.Util;
using Newtonsoft.Json;

namespace StepanMascot
{
    public static class SpriteSheetManager
    {
        static string BasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan", "StepanMascots");
        static JsonSerializerSettings JsonSerializerSettings = new()
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

        };

        public static List<Vector4> getSprites(ImageResult imageResult, int spriteSize)
        {
            var sucess = existsMascot(mascotGraphicManager.MascotName);

            if (sucess.Item1)
            {
                return sucess.Item2;
            }


            var sprites = generateSprites(imageResult, spriteSize);
            spritesManagement(ref sprites, imageResult);
            saveSprites(sprites);

            return sprites;
        }

        private static List<Vector4> generateSprites(ImageResult image, int spriteSize)
        {
            List<Vector4> spritesRectangleMinAndMax = new();

            int index;

            for (index = 0; index < (image.Width / spriteSize) * (image.Height / spriteSize); index++)
            {
                int columns = image.Width / spriteSize;

                int column = index % columns;
                int row = index / columns;

                float left = column * spriteSize;
                float right = (column + 1) * spriteSize;

                float bottom = row * spriteSize;
                float top = (row + 1) * spriteSize;

                Vector2 minUV = (left / image.Width, (image.Height - top) / image.Height);
                Vector2 maxUV = (right / image.Width, (image.Height - bottom) / image.Height);
                spritesRectangleMinAndMax.Add(new(minUV.X, minUV.Y, maxUV.X, maxUV.Y));
            }

            return spritesRectangleMinAndMax;
        }
           
        private static void spritesManagement(ref List<Vector4> sprites, ImageResult image)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                int x = (int)(sprites[i].X * image.Width);
                int y = (int)((sprites[i].Y) * image.Height);

                int width = (int)((sprites[i].Z - sprites[i].X) * image.Width);
                int height = (int)((sprites[i].W - sprites[i].Y) * image.Height);

                if (spriteHasVisiblePixel(image, width, height, sprites[i]))
                {
                    continue;
                }
                else
                {
                    // The sprite is fully transparent, remove it from the list
                    sprites.RemoveAt(i);
                    i--; // Adjust the index since we removed an item
                }
            }
        }

        static bool spriteHasVisiblePixel(ImageResult image, int width, int height, Vector4 uv)
        {
            int x0 = (int)MathF.Floor(uv.X * image.Width);
            int y0 = (int)MathF.Floor(uv.Y * image.Height);
            int x1 = (int)MathF.Ceiling(uv.Z * image.Width);
            int y1 = (int)MathF.Ceiling(uv.W * image.Height);

            x0 = Math.Clamp(x0, 0, image.Width);
            y0 = Math.Clamp(y0, 0, image.Height);
            x1 = Math.Clamp(x1, 0, image.Width);
            y1 = Math.Clamp(y1, 0, image.Height);

            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    int index = (y * image.Width + x) * 4;
                    byte alpha = image.Data[index + 3];

                    if (alpha > 0)
                        return true;
                }
            }
            return false;
        }

        private static void saveSprites(List<Vector4> sprites)
        {
            string path = Path.Combine(BasePath, mascotGraphicManager.MascotName + ".mascot");

            System.Numerics.Vector4[] spriteArray = new System.Numerics.Vector4[sprites.Count];

            for (int i = 0; i < sprites.Count; i++)
            {
                spriteArray[i] = sprites[i].ConvertOpenTKVector4ToSystemNumericsVector4();
            }

            mascot currentMascot = mascot.CreateMascot(spriteArray, mascotGraphicManager.MascotName);

            var json = JsonConvert.SerializeObject(currentMascot, JsonSerializerSettings);
            File.WriteAllText(path, json);
        }

        private static (bool, List<Vector4> sprites) existsMascot(string mascotName)
        {
            string path = Path.Combine(BasePath, mascotName + ".mascot");
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var mascot = JsonConvert.DeserializeObject<mascot>(json, JsonSerializerSettings);

                List<Vector4> sprites = new List<Vector4>();


                if (mascot != null && mascot.sprites.ContainsKey(""))
                {
                    for (int i = 0; i < mascot.sprites.Count; i++)
                    {
                        sprites.Add(mascot.sprites[mascotName][i].ConvertSystemNumericsVector4ToOpenTKVector4());
                    }


                    return (true, sprites);
                }
            }
            return (false, new List<Vector4>());
        }
    }
}
