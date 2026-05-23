using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

partial struct DestroySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb=new EntityCommandBuffer(Allocator.Temp);
        foreach (var (transform,entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<DestroyTag>().WithEntityAccess())
        {
            float posX=transform.ValueRO.Position.x;
            if (posX < -126f || posX > 400f)
            {
                ecb.DestroyEntity(entity);
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
