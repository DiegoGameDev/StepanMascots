using System.Numerics;

namespace StepanMascot.GraphicsWindow.Objects
{
    public class AnimationClip
    {
        public string name;
        public Vector4[] sprites;
        public float framesPerSeconds = 12;

        float timeAcumulated = 0;
        int currentIndex;

        public AnimationClip()
        {
            
        }

        public AnimationClip(Vector4[] spriteArray)
        {
            sprites = spriteArray;
        }

        public void Update(float deltaTime)
        {
            if (sprites == null || sprites.Length == 0)
                return;

            timeAcumulated += deltaTime;

            float ms = 1000f / framesPerSeconds;

            if (timeAcumulated >= ms)
            {
                //avanço 1 sprite
                currentIndex = Math.Clamp(currentIndex + 1, 0, sprites.Length - 1);
                if (currentIndex >= sprites.Length)
                    currentIndex = 0;

                timeAcumulated = 0;
            }
        }

        public void End()
        {
            timeAcumulated = 0;
        }
    }
}
