using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ZombieMono : MonoBehaviour
{
    public float RiseRate;
    public float WalkSpeed;
    public float WalkAmplitude;
    public float WalkFrequency;
    public float EatDamage;
    public float EatAmplitude;
    public float EatFrequency;
}


public class ZombieBaker : Baker<ZombieMono>
{
    public override void Bake(ZombieMono authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity , new ZombieRiseRate { Value = authoring.RiseRate });
        AddComponent(entity, new ZombieWalkProperties
        {
            WalkSpeed = authoring.WalkSpeed,
            WalkAmplitude = authoring.WalkAmplitude,
            WalkFrequency = authoring.WalkFrequency,
        });
        AddComponent(entity, new ZombieEatProperties
        {
            EatDamagePerSecond = authoring.EatDamage,
            EatAmplitude = authoring.EatAmplitude,
            EatFrequency = authoring.EatFrequency
        });
        AddComponent<ZombieTimer>(entity);
        AddComponent<ZombieHeading>(entity);
        AddComponent<NewZombieTag>(entity);
    }
}
