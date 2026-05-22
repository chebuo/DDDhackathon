using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (move, transform, entity) in 
        SystemAPI.Query<RefRO<Move>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            transform.ValueRW.Position += new float3(1, 0, 0) * move.ValueRO.speed * deltaTime;

            if (transform.ValueRO.Position.x > 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
    }
}
