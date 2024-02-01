using System.Text;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Goods
{
    public partial struct TestSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            foreach (var q in SystemAPI.Query<JBuildingsPrefabsTag>().WithEntityAccess())
            {
                var a = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(q.Item2);
                var c = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(q.Item2);
                var b = SystemAPI.GetBuffer<BuildingsBuffer>(q.Item2);

                for (int i = 0; i < b.Length; i++)
                {
                    var item = b.ElementAt(i).Name;

                    Debug.LogWarning(item);
                    if (a.Length > 0)
                    {
                        for (int j = 0; j < a.Length; j++)
                        {
                            Debug.LogWarning("req = " + a.ElementAt(j)._requiredItem);
                        }
                    }

                    if (c.Length > 0)
                    {
                        for (int k = 0; k < c.Length; k++)
                        {
                            Debug.LogWarning("man = " + c.ElementAt(k)._manufacturedItem);
                        }
                    }
                }
            }
        }
    }
}