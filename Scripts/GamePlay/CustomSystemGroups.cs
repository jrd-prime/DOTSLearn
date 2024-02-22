using Unity.Entities;
using UnityEngine;

namespace GamePlay
{
    public partial class MyInitSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("My Init System Group");
        }
    }

    [UpdateAfter(typeof(MyInitSystemGroup))]
    public partial class MyDefaultSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("My Default System Group");
        }
    }
}