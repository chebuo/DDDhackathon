using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        foreach (var spawn in SystemAPI.Query<RefRW<Spawn>>())
        {
            if(spawn.ValueRO.isLeft)spawn.ValueRW.spawnPos=new float3(-35,-6,-9.8f);
            else spawn.ValueRW.spawnPos=new float3(35,6,-9.8f);
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var spawn in SystemAPI.Query<RefRW<Spawn>>())
        {
            var entity=state.EntityManager.Instantiate(spawn.ValueRW.dish);
                state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(spawn.ValueRW.spawnPos));
                
            spawn.ValueRW.timer+=SystemAPI.Time.DeltaTime;
            if(spawn.ValueRO.timer>spawn.ValueRO.spawnInterval)
            {
                spawn.ValueRW.timer=0;
                //var entity=state.EntityManager.Instantiate(spawn.ValueRO.dish);
                state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(spawn.ValueRO.spawnPos));
            }
            
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
