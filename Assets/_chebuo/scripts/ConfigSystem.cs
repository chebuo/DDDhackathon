using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct ConfigSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var job = new SpawnJob
        {
            ecb = ecb
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct SpawnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Config config)
    {
        var distance = math.abs(config.endPos.x - config.startPos.x);
        if (config.interval <= 0f) return;

        var count = (int)math.floor(distance / config.interval) + 1;

        for (int i = 0; i < count; i++)
        {
            var instance = ecb.Instantiate(chunkIndex, config.prefab);

            float x = config.startPos.x + i * config.interval;

            var transform = LocalTransform.FromPosition(
                new float3(x, config.startPos.y, config.startPos.z)
            );

            ecb.SetComponent(chunkIndex, instance, transform);
        }

        // Config削除（1回だけ）
        ecb.RemoveComponent<Config>(chunkIndex, entity);
    }
}