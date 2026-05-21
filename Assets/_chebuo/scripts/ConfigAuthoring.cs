using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Config : IComponentData
{
    public Entity prefab;
    public float3 startPos;
    public float3 endPos;
}

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject _prefab;
    public Vector3 _startPos;
    public Vector3 _endPos;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var data=new Config()
            {
                prefab=GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),
                startPos=(float3)authoring._startPos,
                endPos=(float3)authoring._endPos
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic),data);
        }
    }
}
