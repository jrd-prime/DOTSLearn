using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Sources.Scripts.Screen
{
    /// <summary>
    /// Contains screen [height and length] and [screen center] data
    /// </summary>
    public struct ScreenData : IComponentData
    {
        public float2 WidthAndHeight { get; private set; }
        public float2 ScreenCenter { get; private set; }

        public void SetWidthAndHeight(float2 screenSize)
        {
            if (screenSize is not { x: > 0, y: > 0 }) return;

            WidthAndHeight = screenSize;
            SetScreenCenter(screenSize);
        }

        private void SetScreenCenter(float2 screenSize)
        {
            ScreenCenter = new float2(WidthAndHeight.x / 2f, WidthAndHeight.y / 2f);
        }
    }
}