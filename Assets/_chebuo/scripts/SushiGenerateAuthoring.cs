using Unity.Entities;
using UnityEngine;

public struct SushiGenerate : IBufferElementData
{
    public Entity sushi;
}

public class SushiGenerateAuthoring : MonoBehaviour
{
    public GameObject[] _sushi;
    class Baker:Baker<SushiGenerateAuthoring>
    {
        public override void Bake(SushiGenerateAuthoring src)
        {
            var buffer=AddBuffer<SushiGenerate>(GetEntity(TransformUsageFlags.Dynamic));
            foreach (var sushi in src._sushi)
            {
                buffer.Add(new SushiGenerate()
                {
                    sushi=GetEntity(sushi, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
