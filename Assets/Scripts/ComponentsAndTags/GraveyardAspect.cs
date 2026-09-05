using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public readonly partial struct GraveyardAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRO<LocalTransform> _transformAspect;
    private readonly RefRO<GraveyardProperties> _graveyardProperties;
    private readonly RefRW<GraveyardRandom> _graveyardRandom;
    private readonly DynamicBuffer<ZombieSpawnPoint> _zombieSpawnPoints;
    private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;
    private const float BRAIN_SAFETY_RADIUS_SQ = 400;
    public int NumberTombstonesToSpawn => _graveyardProperties.ValueRO.NumberTombstonesToSpawn;
    public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;
    public DynamicBuffer<ZombieSpawnPoint> ZombieSpawnPoints => _zombieSpawnPoints;
    public float ZombieSpawnTimer
    {
        get => _zombieSpawnTimer.ValueRO.Value;
        set => _zombieSpawnTimer.ValueRW.Value = value;
    }
    public bool TimeToSpawnZombie => (ZombieSpawnTimer <= 0f);
    public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;
    public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;
    public float3 Position => _transformAspect.ValueRO.Position;


    public LocalTransform GetRandomTombstoneTransform()
    {
        return LocalTransform.FromPositionRotationScale(GetRandomPosition(),
                                                        GetRandomRotation(),
                                                        GetRandomScale(0.5f));
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(_transformAspect.ValueRO.Position , randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);
        return randomPosition;
    }

    private float3 MinCorner => _transformAspect.ValueRO.Position - HalfDimensions;
    private float3 MaxCorner => _transformAspect.ValueRO.Position + HalfDimensions;
    private float3 HalfDimensions => new()
    {
        x = _graveyardProperties.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = _graveyardProperties.ValueRO.FieldDimensions.y * 0.5f,
    };

    private quaternion GetRandomRotation() => quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
    private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min , 1f);

    private float3 GetRandomZombieSpawnPoint ()
    {
        return ZombieSpawnPoints[_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPoints.Length)].Value;
    }

    public LocalTransform GetZombieSpawnPoint ()
    {
        var position = GetRandomZombieSpawnPoint();
        return LocalTransform.FromPositionRotationScale(position,
                                                        quaternion.RotateY(MathHelpers.GetHeading(position , _transformAspect.ValueRO.Position)),
                                                        1f);
    }

}
