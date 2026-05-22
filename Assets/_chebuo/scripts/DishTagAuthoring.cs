using Unity.Entities;
using UnityEngine;

public struct DishTag : IComponentData
{
    
}

public class DishTagAuthoring : MonoBehaviour
{
    class Baker : Baker<DishTagAuthoring>
    {
        public override void Bake(DishTagAuthoring src)
        {
            AddComponent<DishTag>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}