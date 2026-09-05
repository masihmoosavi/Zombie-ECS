using Unity.Entities;

[InternalBufferCapacity(8)]
public struct BrainDamageBufferElement : IBufferElementData
{
    public float Value;
}
