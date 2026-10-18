using System;
using System.Collections.Generic;
using System.Text;

namespace StepanMascot.ShadersServices
{
    internal static class ShaderFilesService
    {
        static readonly string shaderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stepan/StepanMascots", "Shaders");
        static readonly string shaderPathDebug = @"C:\Users\DiegoMoggingBytes\Documents\GitHub\STP Softwares\StepanMascots\StepanMascot\GraphicsWindow\Shaders";

        internal static int ShaderFromFile(string name)
        {
#if DEBUG
            string path = shaderPathDebug;
#else
            path = shaderPath;
#endif


            string fileVert = Path.Combine(path, $"{name}.vert");
            string fileFrag = Path.Combine(path, $"{name}.frag");

            if (!File.Exists(fileVert))
                return -1;
            if (!File.Exists(fileFrag))
                return -1;

            mascotGraphicManager.createShader(fileVert, fileFrag, out int handle);

            return handle;
        }
    }

    internal enum ShaderType { vertex, frag}
}
