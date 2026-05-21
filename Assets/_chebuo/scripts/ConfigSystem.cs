using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

partial struct ConfigSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
        => state.RequireForUpdate<Config>();

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var config in SystemAPI.Query<RefRO<Config>>())
        {
            var con = config.ValueRO;
            var distance = math.abs(con.endPos.x - con.startPos.x);
            if (con.interval <= 0f) continue;
            var count = (int)math.floor(distance / con.interval) + 1;
            var instances = state.EntityManager.Instantiate(con.prefab, count, Allocator.Temp);

            for (int i = 0; i < instances.Length; i++)
            {
                var entity = instances[i];
                var transform = state.EntityManager.GetComponentData<LocalTransform>(entity);
                float x = con.startPos.x + i * con.interval;
                transform.Position = new float3(x, con.startPos.y, con.startPos.z);
                state.EntityManager.SetComponentData(entity, transform);
            }
        }
        state.Enabled = false;
    }
}