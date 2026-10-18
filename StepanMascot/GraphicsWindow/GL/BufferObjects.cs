using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StepanMascot.Batch;

namespace StepanMascot.GraphicsWindow.GLObjects
{
    // Classe que encapsula um VAO
    public class VertexArrayObject : IDisposable
    {
        public readonly int Handle;
        private bool disposedValue;

        public VertexArrayObject()
        {
            Handle = GL.GenVertexArray();
        }

        public void Bind() => GL.BindVertexArray(Handle);
        public static void Unbind() => GL.BindVertexArray(0);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteVertexArray(Handle);
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    // Classe que encapsula um VBO (Array Buffer)
    public class VertexBufferObject : IDisposable
    {
        public readonly int Handle;
        public bool disposedValue;

        public VertexBufferObject()
        {
            Handle = GL.GenBuffer();
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        public static void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        // Envia um array de floats para o buffer (usado para vertices)
        public void SetData(float[] data, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, usage);
        }

        // Envia um array de InstanceData (definido em StepanMascot.Batch) para o buffer.
        // Cada InstanceData contém: Vector4 instanceSpriteUv (4 floats), Color4 instanceColor (4 floats), Matrix4 model (16 floats)
        // Total por instância: 24 floats.
        internal void SetData(InstanceData[] instanceDatas, bool instanceData = true, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
        {
            if (instanceDatas == null || instanceDatas.Length == 0)
                return;

            if (instanceData && usage == BufferUsageHint.StaticDraw)
                usage = BufferUsageHint.DynamicDraw;

            Bind();
            int count = instanceDatas.Length;
            const int floatsPerInstance = 24; // 4 + 4 + 16
            float[] data = new float[count * floatsPerInstance];

            for (int i = 0; i < count; i++)
            {
                var inst = instanceDatas[i];
                int b = i * floatsPerInstance;
                // instanceSpriteUv (Vector4)
                data[b + 0] = inst.instanceSpriteUv.X;
                data[b + 1] = inst.instanceSpriteUv.Y;
                data[b + 2] = inst.instanceSpriteUv.Z;
                data[b + 3] = inst.instanceSpriteUv.W;

                // instanceColor (Color4)
                data[b + 4] = inst.instanceColor.R;
                data[b + 5] = inst.instanceColor.G;
                data[b + 6] = inst.instanceColor.B;
                data[b + 7] = inst.instanceColor.A;

                // model matrix (16 floats)
                var m = inst.model;
                data[b + 8] = m.M11;
                data[b + 9] = m.M12;
                data[b + 10] = m.M13;
                data[b + 11] = m.M14;
                data[b + 12] = m.M21;
                data[b + 13] = m.M22;
                data[b + 14] = m.M23;
                data[b + 15] = m.M24;
                data[b + 16] = m.M31;
                data[b + 17] = m.M32;
                data[b + 18] = m.M33;
                data[b + 19] = m.M34;
                data[b + 20] = m.M41;
                data[b + 21] = m.M42;
                data[b + 22] = m.M43;
                data[b + 23] = m.M44;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, usage);
        }

        // Sobrecarga que permite indicar que os dados são de instância.
        // Não altera o comportamento por si só, mas oferece uma assinatura explícita
        // para código que trata dados por-instância (ex.: escolher BufferUsageHint apropriado ao chamar).
        public void SetData(float[] data, bool instanceData, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            // Se for dados de instância e o chamador não especificou uso, recomendamos DynamicDraw porque
            // dados de instância geralmente são atualizados com frequência.
            if (instanceData && usage == BufferUsageHint.StaticDraw)
                usage = BufferUsageHint.DynamicDraw;

            SetData(data, usage);
        }

        // Sobrecarga para um único Matrix4
        public void SetData(Matrix4 matrix, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            SetData(new Matrix4[] { matrix }, usage);
        }

        // Envia um array de Matrix4 (OpenTK.Mathematics) para o buffer.
        // Cada Matrix4 é serializada como 16 floats (M11..M44) na ordem de memória padrão do struct.
        public void SetData(Matrix4[] matrices, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            if (matrices == null || matrices.Length == 0)
                return;

            Bind();
            float[] data = new float[matrices.Length * 16];
            for (int i = 0; i < matrices.Length; i++)
            {
                var m = matrices[i];
                int b = i * 16;
                data[b + 0] = m.M11;
                data[b + 1] = m.M12;
                data[b + 2] = m.M13;
                data[b + 3] = m.M14;
                data[b + 4] = m.M21;
                data[b + 5] = m.M22;
                data[b + 6] = m.M23;
                data[b + 7] = m.M24;
                data[b + 8] = m.M31;
                data[b + 9] = m.M32;
                data[b + 10] = m.M33;
                data[b + 11] = m.M34;
                data[b + 12] = m.M41;
                data[b + 13] = m.M42;
                data[b + 14] = m.M43;
                data[b + 15] = m.M44;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, usage);
        }

        // Versão que aceita indicar que os dados são por-instância.
        public void SetData(Matrix4[] matrices, bool instanceData, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            if (instanceData && usage == BufferUsageHint.StaticDraw)
                usage = BufferUsageHint.DynamicDraw;

            SetData(matrices, usage);
        }

        // Atualiza parte dos dados do buffer
        public void UpdateData(float[] data, int offsetBytes)
        {
            Bind();
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetBytes, data.Length * sizeof(float), data);
        }

        // Atualiza parte dos dados do buffer com um array de Matrix4 (OpenTK.Mathematics).
        // offsetBytes indica o deslocamento em bytes dentro do buffer onde a atualização deve começar.
        public void UpdateData(Matrix4[] matrices, int offsetBytes)
        {
            if (matrices == null || matrices.Length == 0)
                return;

            Bind();
            float[] data = new float[matrices.Length * 16];
            for (int i = 0; i < matrices.Length; i++)
            {
                var m = matrices[i];
                int b = i * 16;
                data[b + 0] = m.M11;
                data[b + 1] = m.M12;
                data[b + 2] = m.M13;
                data[b + 3] = m.M14;
                data[b + 4] = m.M21;
                data[b + 5] = m.M22;
                data[b + 6] = m.M23;
                data[b + 7] = m.M24;
                data[b + 8] = m.M31;
                data[b + 9] = m.M32;
                data[b + 10] = m.M33;
                data[b + 11] = m.M34;
                data[b + 12] = m.M41;
                data[b + 13] = m.M42;
                data[b + 14] = m.M43;
                data[b + 15] = m.M44;
            }

            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offsetBytes, data.Length * sizeof(float), data);
        }

        // Atualiza parte dos dados do buffer com uma única Matrix4
        public void UpdateData(Matrix4 matrix, int offsetBytes)
        {
            UpdateData(new Matrix4[] { matrix }, offsetBytes);
        }

        // Atualiza parte dos dados do buffer com um array de InstanceData.
        // offsetBytes indica o deslocamento em bytes dentro do buffer onde a atualização deve começar.
        internal void UpdateData(InstanceData[] instanceDatas, IntPtr offsetBytes)
        {
            if (instanceDatas == null || instanceDatas.Length == 0)
                return;

            Bind();
            int count = instanceDatas.Length;
            const int floatsPerInstance = 24; // 4 (spriteUv) + 4 (color) + 16 (model)
            float[] data = new float[count * floatsPerInstance];

            for (int i = 0; i < count; i++)
            {
                var inst = instanceDatas[i];
                int b = i * floatsPerInstance;
                // instanceSpriteUv (Vector4)
                data[b + 0] = inst.instanceSpriteUv.X;
                data[b + 1] = inst.instanceSpriteUv.Y;
                data[b + 2] = inst.instanceSpriteUv.Z;
                data[b + 3] = inst.instanceSpriteUv.W;

                // instanceColor (Color4)
                data[b + 4] = inst.instanceColor.R;
                data[b + 5] = inst.instanceColor.G;
                data[b + 6] = inst.instanceColor.B;
                data[b + 7] = inst.instanceColor.A;

                // model matrix (16 floats)
                var m = inst.model;
                data[b + 8] = m.M11;
                data[b + 9] = m.M12;
                data[b + 10] = m.M13;
                data[b + 11] = m.M14;
                data[b + 12] = m.M21;
                data[b + 13] = m.M22;
                data[b + 14] = m.M23;
                data[b + 15] = m.M24;
                data[b + 16] = m.M31;
                data[b + 17] = m.M32;
                data[b + 18] = m.M33;
                data[b + 19] = m.M34;
                data[b + 20] = m.M41;
                data[b + 21] = m.M42;
                data[b + 22] = m.M43;
                data[b + 23] = m.M44;
            }

            GL.BufferSubData(BufferTarget.ArrayBuffer, offsetBytes, data.Length * sizeof(float), data);
        }

        // Atualiza parte dos dados do buffer com uma única InstanceData
        internal void UpdateData(InstanceData instanceData, int offsetBytes)
        {
            UpdateData(new InstanceData[] { instanceData }, offsetBytes);
        }

        // Configura o ponteiro de atributo (como VertexAttribPointer)
        public void VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
        }

        // Configura um atributo e define seu divisor para instancing.
        // divisor = 0 -> atributo por vértice (não instanciado)
        // divisor = 1 -> atributo por instância
        public void VertexAttribPointerInstanced(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset, int divisor)
        {
            VertexAttribPointer(index, size, type, normalized, stride, offset);
            GL.VertexAttribDivisor(index, divisor);
        }

        // Define apenas o divisor de instância para um atributo já configurado.
        public void SetAttribDivisor(int index, int divisor)
        {
            GL.VertexAttribDivisor(index, divisor);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteBuffer(Handle);
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    // Classe que encapsula um EBO (Element / Index Buffer)
    public class ElementBufferObject : IDisposable
    {
        public readonly int Handle;
        private bool disposedValue;

        public ElementBufferObject()
        {
            Handle = GL.GenBuffer();
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
        public static void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        // Envia indices (uint[]) para o elemento de índice
        public void SetData(uint[] indices, BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            Bind();
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, usage);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteBuffer(Handle);
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
