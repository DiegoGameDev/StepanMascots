using OpenTK.Mathematics;

namespace StepanMascot.GraphicsWindow.Util
{
    public static class otkVectorsToSystemNumerics
    {
        public static System.Numerics.Vector2 ConvertOpenTKVector2ToSystemNumericsVector2(this Vector2 otkVector)
        {
            return new System.Numerics.Vector2(otkVector.X, otkVector.Y);
        }

        public static System.Numerics.Vector3 ConvertOpenTKVector3ToSystemNumericsVector3(this Vector3 otkVector)
        {
            return new System.Numerics.Vector3(otkVector.X, otkVector.Y, otkVector.Z);
        }

        public static System.Numerics.Vector4 ConvertOpenTKVector4ToSystemNumericsVector4(this Vector4 otkVector)
        {
            return new System.Numerics.Vector4(otkVector.X, otkVector.Y, otkVector.Z, otkVector.W);
        }

        public static Vector2 ConvertSystemNumericsVector2ToOpenTKVector2(this System.Numerics.Vector2 sysVector)
        {
            return new Vector2(sysVector.X, sysVector.Y);
        }

        public static Vector3 ConvertSystemNumericsVector3ToOpenTKVector3(this System.Numerics.Vector3 sysVector)
        {
            return new Vector3(sysVector.X, sysVector.Y, sysVector.Z);
        }

        public static Vector4 ConvertSystemNumericsVector4ToOpenTKVector4(this System.Numerics.Vector4 sysVector)
        {
            return new Vector4(sysVector.X, sysVector.Y, sysVector.Z, sysVector.W);
        }
    }
}