using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ZombieRiseAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRW<LocalTransform> _transformAspect;
    private readonly RefRO<ZombieRiseRate> _zombieRiseRate;
    public bool IsAboveGround => _transformAspect.ValueRO.Position.y >= 0;

    public void Rise(float deltaTime)
    {
        _transformAspect.ValueRW.Position += math.up() * _zombieRiseRate.ValueRO.Value * deltaTime;
    }

    public void SetAtGroundLevel ()
    {
        var position = _transformAspect.ValueRO.Position;
        position.y = 0;
        _transformAspect.ValueRW.Position = position;
    }
}
