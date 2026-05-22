using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public struct SushiGenerate : IBufferElementData
{
    public Entity sushi;
}

public class SushiGenerateAuthoring : MonoBehaviour
{
    [SerializeField] SushiData sushiData;

    class Baker:Baker<SushiGenerateAuthoring>
    {
        public override void Bake(SushiGenerateAuthoring src)
        {
            var buffer=AddBuffer<SushiGenerate>(GetEntity(TransformUsageFlags.Dynamic|TransformUsageFlags.Renderable));
            foreach (var sushi in src.sushiData.sushiList)
            {
                buffer.Add(new SushiGenerate()
                {
                    sushi=GetEntity(sushi, TransformUsageFlags.Dynamic|TransformUsageFlags.Renderable)
                });
            }
        }
    }
}
