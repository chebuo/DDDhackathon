using UnityEngine;
using Unity.Entities;

public struct Move : IComponentData
{
    public float speed;
}

public class MoveAuthoring : MonoBehaviour
{
    public float _speed;

    class Baker : Baker<MoveAuthoring>
    {
        public override void Bake(MoveAuthoring src)
        {
            var data = new Move() { speed = src._speed };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}