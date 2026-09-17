using System.Numerics;

namespace StepanMascot
{
    public class mascot
    {
        public string mascotName = "";
        public Dictionary<string, Vector4[]> sprites = new Dictionary<string, Vector4[]>(1);

        public static mascot CreateMascot(Vector4[] sprites, string name)
        {
            mascot mascot = new()
            {
                mascotName = name,
            };
            mascot.sprites.Add(mascot.mascotName, sprites);

            return mascot;
        }

    }
}
