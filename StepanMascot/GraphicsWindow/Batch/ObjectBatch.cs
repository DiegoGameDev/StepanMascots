using OpenTK.Mathematics;
using StepanMascot.GraphicsWindow;
using StepanMascot.GraphicsWindow.GLObjects;
using StepanMascot.GraphicsWindow.Util;

namespace StepanMascot.Batch
{
    internal class ObjectBatch
    {
        internal Dictionary<objectKeyBatch, InstanceState> batches = new Dictionary<objectKeyBatch, InstanceState>();
        internal Dictionary<InstanceState, List<Mascot>> mascots = new Dictionary<InstanceState, List<Mascot>>();

        internal VertexBufferObject mascotsVBO;

        internal const int capacity = 64; // Initial capacity for instance data

        public void DirtyMascotBatch(Mascot mascot) 
        {
            var key = new objectKeyBatch(mascot.mascotMaterial);
            if (batches.ContainsKey(key))
            {
                var instanceInfo = batches[key];
                instanceInfo.instanceDatas[mascot.instanceIndex] = new InstanceData
                {
                    instanceSpriteUv = mascot.currentSprite.ConvertSystemNumericsVector4ToOpenTKVector4(),
                    instanceColor = mascot.color,
                    model = mascot.model()
                };
                instanceInfo.isDirty = true;
            }

            mascot.Drawed();
        }

        public void ChangeBatchInstance(Mascot mascot, out string message) // aqui o mascot ja está com o batch alterado
        {
            var key = new objectKeyBatch(mascot.mascotMaterial);

            foreach (var i in batches)
            {
                var state = i.Value;
                if (state.identifier == mascot.objectBatchIndex)
                {
                    state.isDirty = true;
                    //adicionar em outro batch
                    var otherBatch = batches.GetValueOrDefault(key);
                    if (otherBatch != default && otherBatch.instanceCount < capacity - 1)
                    {
                        otherBatch?.instanceCount += 1;
                        otherBatch?.instanceDatas[otherBatch.instanceCount] = new InstanceData()
                        {
                            instanceSpriteUv = mascot.currentSprite.ConvertSystemNumericsVector4ToOpenTKVector4(),
                            instanceColor = mascot.color,
                            model = mascot.model()
                        };

                        otherBatch.isDirty = true;

                        message = "mascot successfully moved to existing batch.";
                    }
                    else if (otherBatch == default)
                    {
                        //cria um novo batch
                        var newBatch = new InstanceState(capacity)
                        {
                            identifier = batches.Count,
                            instanceCount = 1,
                        };
                        newBatch.instanceDatas[0] = new InstanceData()
                        {
                            instanceSpriteUv = mascot.currentSprite.ConvertSystemNumericsVector4ToOpenTKVector4(),
                            instanceColor = mascot.color,
                            model = mascot.model()
                        };
                        batches.Add(key, newBatch);
                        message = "mascot successfully moved to a newly created batch.";

                        newBatch.isDirty = true;
                    }
                    else // o batch está lotado
                    {
                        message = "mascot could not be moved to the batch because it is full.";
                    }

                    //swap and pop
                    state.instanceDatas[mascot.instanceIndex] = state.instanceDatas.Last(); //substitui pelo ultimo da lista
                    state.instanceDatas[state.instanceCount] = new InstanceData(); //anula o ultimo da lista

                    return;
                }
            }
            message = "mascot could not be moved to any batch.";
        }

        public bool AddMascot(Mascot mascot, objectKeyBatch key)
        {
            InstanceData instanceData = new InstanceData
            {
                instanceSpriteUv = mascot.currentSprite.ConvertSystemNumericsVector4ToOpenTKVector4(),
                instanceColor = mascot.color,
                model = mascot.model()
            };

            if (batches.ContainsKey(key)) // adiciona um objeto no batch correspondido
            {
                var instanceInfo = batches[key];

                if (instanceInfo.instanceCount < capacity)
                {
                    mascot.instanceIndex = instanceInfo.instanceCount;
                    mascot.objectBatchIndex = instanceInfo.identifier;
                    instanceInfo.instanceDatas[instanceInfo.instanceCount] = instanceData;
                    instanceInfo.instanceCount++;
                    instanceInfo.isDirty = true;
                    mascots[instanceInfo].Add(mascot);
                    mascot.objectBatch = this;
                    return true;
                }
                else
                {
                    //if capacity was range bound override last element
                    mascot.objectBatchIndex = instanceInfo.identifier;
                    mascot.instanceIndex = instanceInfo.instanceCount;
                    instanceInfo.instanceDatas[capacity - 1] = instanceData;
                    instanceInfo.isDirty = true;
                    mascots[instanceInfo].Add(mascot);
                    mascot.objectBatch = this;
                    return true;
                }
            }
            else // cria um objectBatch
            {
                try
                {
                    var info = new InstanceState(capacity)
                    {
                        identifier = batches.Count,
                        instanceCount = 0,
                    };

                    mascot.objectBatchIndex = info.identifier;
                    mascot.instanceIndex = info.instanceCount;
                    info.instanceDatas[info.instanceCount] = instanceData;
                    info.instanceCount++;
                    info.isDirty = true;
                    mascots.Add(info, new List<Mascot>());
                    mascots[info].Add(mascot);
                    mascot.objectBatch = this;

                    batches.Add(key, info);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    
        public void RemoveMascot(Mascot mascot)
        {
            var key = new objectKeyBatch(mascot.mascotMaterial);
            if (batches.ContainsKey(key))
            {
                var instanceInfo = batches[key];
                if (instanceInfo.instanceCount > 0)
                {
                    // Swap and pop
                    instanceInfo.instanceDatas[mascot.instanceIndex] = instanceInfo.instanceDatas[instanceInfo.instanceCount - 1];
                    instanceInfo.instanceDatas[instanceInfo.instanceCount - 1] = new InstanceData(); // Clear the last element
                    instanceInfo.instanceCount--;
                    instanceInfo.isDirty = true;
                    mascots.Remove(instanceInfo);
                }
            }
        }
    }
    internal class InstanceState
    {
        public int identifier;
        public int instanceCount;
        public InstanceData[] instanceDatas;

        public bool isDirty;

        public InstanceState(int capacity)
        {
            instanceDatas = new InstanceData[capacity];
        }
    }

    internal struct InstanceData
    {
        public Vector4 instanceSpriteUv;
        public Color4 instanceColor;
        public Matrix4 model;
        
        public byte SizeInBytes()
        {
            return (byte)(sizeof(float) * 4f + sizeof(float) * 4f + sizeof(float) * 16f); // 4 floats for instanceSpriteUv, 4 floats for instanceColor, and 16 floats for model matrix
        }
    }

    internal struct objectKeyBatch : IEquatable<objectKeyBatch>
    {
        public Material materialBatch;

        public objectKeyBatch(Material material)
        {
            materialBatch = material;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(materialBatch.shaderProgram, materialBatch.texture);
        }

        public bool Equals(objectKeyBatch other)
        {
            return other.GetHashCode() == GetHashCode();
        }
    }
}
