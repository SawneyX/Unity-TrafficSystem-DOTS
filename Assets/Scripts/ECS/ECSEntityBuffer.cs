using Unity.Entities;
using Unity.Mathematics;


[InternalBufferCapacity(20)]
public struct ECSBufferEntity : IBufferElementData
{

    public Entity entity;


}

