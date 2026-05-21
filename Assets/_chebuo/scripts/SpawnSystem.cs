using Unity.Burst;
using Unity.Entities;

partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
       
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var spawn in SystemAPI.Query<RefRW<Spawn>>())
        {
            spawn.ValueRW.timer+=SystemAPI.Time.DeltaTime;
            if(spawn.ValueRO.timer>spawn.ValueRO.spawnInterval)
            {
                spawn.ValueRW.timer=0;
                state.EntityManager.Instantiate(spawn.ValueRO.dish);
            }
            
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
