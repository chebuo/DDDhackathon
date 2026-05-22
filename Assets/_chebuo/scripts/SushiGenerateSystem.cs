using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

partial struct SushiGenerateSystem : ISystem
{
    private Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random=new Random(123u);
    }

    [BurstCompile] 
    public void OnUpdate(ref SystemState state)
    {
        var ecb=new EntityCommandBuffer(Allocator.Temp);
        foreach (var (buffer,dish) in SystemAPI.Query<DynamicBuffer<SushiGenerate>>().WithAll<DishTag>().WithEntityAccess())
        {
            var index = random.NextInt(buffer.Length);      
            UnityEngine.Debug.Log($"index:{index}");
            var prefab = buffer[index].sushi;
            var instance = ecb.Instantiate(prefab);
            ecb.AddComponent(instance, new Parent()
            {
                Value = dish
            });
            ecb.AddComponent(instance,LocalTransform.FromPosition(new float3(0, 0f, 0)));
        
            ecb.RemoveComponent<DishTag>(dish);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
