using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GraveyardMono : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberTombstonesToSpawn;
    public GameObject TombstonePrefab;
    public uint RandomSeed;
    public GameObject ZombiePrefab;
    public float ZombieSpawnRate;
}

public class GraveyardBaker : Baker<GraveyardMono>
{
    public override void Bake(GraveyardMono authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GraveyardProperties
        {
            FieldDimensions = authoring.FieldDimensions,
            NumberTombstonesToSpawn = authoring.NumberTombstonesToSpawn,
            TombstonePrefab = GetEntity(authoring.TombstonePrefab, TransformUsageFlags.Dynamic),
            ZombiePrefab = GetEntity(authoring.ZombiePrefab , TransformUsageFlags.Dynamic),
            ZombieSpawnRate = authoring.ZombieSpawnRate
        });
        AddComponent(entity , new GraveyardRandom
        {
            Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
        });
        AddBuffer<ZombieSpawnPoint>(entity);
        //AddComponent(entity, new ZombieSpawnPoints());
        AddComponent<ZombieSpawnTimer>(entity);

    }
}
