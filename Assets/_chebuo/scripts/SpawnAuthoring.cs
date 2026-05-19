using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Spawn : IComponentData
{
    public float spawnInterval;
    public Entity dish;
    public bool isLeft;
    public float3 spawnPos;
    public float timer;
}

public class SpawnAuthoring : MonoBehaviour
{
    public float _spawnInterval;
    public GameObject _dish;
    public bool _isLeft;
    public Vector3 _spawnPos;
    class Baker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring src)
        {
            var data=new Spawn()
            {
                spawnInterval=src._spawnInterval,
                dish=GetEntity(src._dish, TransformUsageFlags.Dynamic),
                isLeft=src._isLeft,
                spawnPos=(float3)src._spawnPos
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic),data);
        }
    }
}
