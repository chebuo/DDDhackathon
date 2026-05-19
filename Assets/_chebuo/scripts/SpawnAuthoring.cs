using Unity.Entities;
using UnityEngine;

public struct Spawn : IComponentData
{
    public float spawnInterval;
    public Entity dish;
    public float timer;
}

public class SpawnAuthoring : MonoBehaviour
{
    public float _spawnInterval;
    public GameObject _dish;
    class Baker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring src)
        {
            var data=new Spawn()
            {
                spawnInterval=src._spawnInterval,
                dish=GetEntity(src._dish, TransformUsageFlags.Dynamic),
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic),data);
        }
    }
}
