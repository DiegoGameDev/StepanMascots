using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StepanMascot.GraphicsWindow.GLObjects;
using StepanMascot.GraphicsWindow.Util;
using System.Runtime.InteropServices;

namespace StepanMascot.Batch
{
    internal static class RenderBatch
    {
        private static readonly ObjectBatch objectBatch = new ObjectBatch();

        private static readonly VertexArrayObject vao = new VertexArrayObject();
        private static readonly VertexBufferObject vbo = new VertexBufferObject();
        private static readonly ElementBufferObject ebo = new ElementBufferObject();


        internal static void Initialize()
        {
            vao.Bind();
            
            vbo.Bind();
            vbo.SetData(Mascot.vertices);
            vbo.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            vbo.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            ebo.Bind();
            ebo.SetData(Mascot.indices);
        }

        public static void AddBatch(Mascot mascot)
        {
            var objectKey = new objectKeyBatch(mascot.mascotMaterial);

            vao.Bind();

            objectBatch.AddMascot(mascot, objectKey);
        }

        private static void UpdateBatch(InstanceState batch)
        {
            if (!batch.isDirty) //tem atualização em alguma instancia (no momento atualizar todos)
                return;


                bool needReload = objectBatch.mascotsVBO == null || objectBatch.mascotsVBO.disposedValue;

            if (needReload)
            {
                vao.Bind();
                 
                objectBatch.mascotsVBO?.Dispose();
                objectBatch.mascotsVBO = new();
                objectBatch.mascotsVBO.Bind();
                objectBatch.mascotsVBO.SetData(batch.instanceDatas, true, usage: BufferUsageHint.DynamicDraw);

                int stride = Marshal.SizeOf<InstanceData>();

                objectBatch.mascotsVBO.VertexAttribPointerInstanced(2, 4, VertexAttribPointerType.Float, false, stride, 0, 1);
                objectBatch.mascotsVBO.VertexAttribPointerInstanced(3, 4, VertexAttribPointerType.Float, false, stride, 4 * sizeof(float), 1);

                int modelAttribIndex = 4;
                for (int row = 0; row < 4; row++)
                {
                    int offset = (8 * sizeof(float)) + (row * 4 * sizeof(float));
                    objectBatch.mascotsVBO.VertexAttribPointerInstanced((modelAttribIndex + row), 4, VertexAttribPointerType.Float, false, stride, offset, 1);
                }
            }
            else
            {
                objectBatch.mascotsVBO?.UpdateData(batch.instanceDatas, IntPtr.Zero);
            }
                batch.isDirty = false;
        }

        private static void ClearBatches()
        {
            objectBatch.batches.Clear();
        }

        public static void Render(Matrix4 VP)
        {
            foreach (var batch in objectBatch.batches)
            {
                UpdateBatch(batch.Value);
                //Console.WriteLine($"Rendering batch with shader program {batch.Key.shaderProgram} and instance count {batch.Value.instanceCount}");

                GL.UseProgram(batch.Key.materialBatch.shaderProgram);
                batch.Key.materialBatch.Bind();
                vao.Bind();

                GL.UniformMatrix4(GL.GetUniformLocation(batch.Key.materialBatch.shaderProgram, "VP"), true, ref VP);

                GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero, batch.Value.instanceCount);
            }
        }
    }
}
