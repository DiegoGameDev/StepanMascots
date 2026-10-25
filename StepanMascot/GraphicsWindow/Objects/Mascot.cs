using StbImageSharp;
using Newtonsoft.Json;
using System.Numerics;

using mat4 = OpenTK.Mathematics.Matrix4;
using Color = OpenTK.Mathematics.Color4;
using StepanMascot.Batch;
using StepanMascot.GraphicsWindow;
using StepanMascot.GraphicsWindow.Util;
using StepanMascot.GraphicsWindow.Objects;

namespace StepanMascot
{
    public class Mascot
    {
        [JsonIgnore]
        internal ObjectBatch objectBatch;
        [JsonIgnore]
        internal int objectBatchIndex = -1;
        [JsonIgnore]
        public int instanceIndex = -1;


        #region Properties
        public string mascotName = "";
        public Color color = Color.White;

        public Vector3 position { get { return _position; } set { if (value != _position) { _position = value; isDirty = true; } } }
        Vector3 _position;
        public Vector3 scale { get { return _scale; } set { if (value != _scale) { _scale = value; isDirty = true; } } }
        Vector3 _scale = new Vector3(10);

        public List<Vector4> sprites = new List<Vector4>();
        public Animator animator;


        //Render Properties
        public string shaderName = "Default";
        public string texturePath;
        public int cell;

        #endregion


        #region Load Objects
        [JsonIgnore]
        public bool isRendering = false;

        [JsonIgnore]
        public ImageResult mascotImage;
        [JsonIgnore] 
        public Vector4 currentSprite { get { return _currentSprite; } set { if (value != _currentSprite) { _currentSprite = value; isDirty = true; } } }
        Vector4 _currentSprite;

        #endregion

        #region GLObjects

        [JsonIgnore]
        public Material mascotMaterial;

        [JsonIgnore]
        public static readonly float[] vertices = new float[]
            {
                -1f, -1f, 0.0f, 0.0f, 0.0f, // Bottom-left vertex
                 1f, -1f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
                 1f,  1f, 0.0f, 1.0f, 1.0f, // Top-right vertex
                 -1f,  1f, 0.0f, 0.0f, 1.0f  // Top-left vertex
            };

        [JsonIgnore]
        public static readonly uint[] indices = new uint[]
        {
                0, 1, 2, // First triangle
                2, 3, 0  // Second triangle
        };

        [JsonIgnore]
        public bool isDirty { get; private set; } = true;

        #endregion

        public Mascot(string name = "")
        {
            mascotName = name;
        }
        int currentIndex = 0;
        float timeAccumulator = 0f;
        public static Mascot CreateMascot(Vector4[] sprites, string name)
        {
            Mascot mascot = new()
            {
                mascotName = name,
            };
            mascot.sprites.AddRange(sprites);

            return mascot;
        }


        internal void Drawed()
        {
            isDirty = false;
        }

        public void mascotUpdate(float deltaTime)
        {
            animator.Update(deltaTime, this);
        }

        public void BatchUpdate()
        {
            if (objectBatch != null && isDirty)
            {
                objectBatch.DirtyMascotBatch(this);
            }
        }

        public mat4 model() => mat4.CreateScale(scale.ConvertSystemNumericsVector3ToOpenTKVector3()) * mat4.CreateTranslation(position.X, position.Y, position.Z);
        internal void SetObjectBatch(ObjectBatch batch)
        {
            objectBatch = batch;
        }
    }
}
