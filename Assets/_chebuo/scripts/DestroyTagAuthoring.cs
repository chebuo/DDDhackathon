using Unity.Entities;
using UnityEngine;

public struct DestroyTag : IComponentData
{
    
}

public class DestroyTagAuthoring : MonoBehaviour
{
    class Baker : Baker<DestroyTagAuthoring>
    {
        public override void Bake(DestroyTagAuthoring src)
        {
            AddComponent<DestroyTag>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}