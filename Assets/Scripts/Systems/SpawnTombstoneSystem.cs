using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem : ISystem
{
    [BurstCompile]
    public void OnCreate (ref SystemState state)
    {
        state.RequireForUpdate<GraveyardProperties>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
        var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        //var spawnPoints = state.EntityManager.GetBuffer<ZombieSpawnPoint>(graveyardEntity);
        var spawnPoints = graveyard.ZombieSpawnPoints;
        var tombstoneOffset = new float3(0f , -4f , 1f);
        for (int i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
        {
            var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);
            var newTombstoneTransform = graveyard.GetRandomTombstoneTransform();
            ecb.SetComponent(newTombstone , newTombstoneTransform);
            var newZombieSpawnPoint = newTombstoneTransform.Position + tombstoneOffset;
            spawnPoints.Add(new ZombieSpawnPoint {
                Value = newZombieSpawnPoint
            });
        }
        //graveyard.ZombieSpawnPoints = spawnPoints.ToArray(Allocator.Persistent);
        ecb.Playback(state.EntityManager);
    }
}
