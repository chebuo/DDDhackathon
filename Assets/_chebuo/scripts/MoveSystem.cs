using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (move,transform) in SystemAPI.Query<RefRO<Move>, RefRW<LocalTransform>>())
        {
            transform.ValueRW.Position += new float3(1, 0, 0) * move.ValueRO.speed * deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
