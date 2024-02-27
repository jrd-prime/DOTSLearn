using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Sources.Scripts.Utility
{
    public static class Utils
    {
        public static bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static Vector3 GetScreenCenterPoint()
        {
            return new Vector2(UnityEngine.Screen.width / 2f, UnityEngine.Screen.height / 2f);
        }


        public static bool IsScreenSizeChanged(float2 screenSize, out float2 newSize)
        {
            var currentWidth = Screen.width;
            var currentHeight = Screen.height;

            newSize = new float2(currentWidth, currentHeight);

            // Returns true for the recalculation
            if (screenSize is { x: < 0, y: < 0 }) return true;

            return Math.Abs(screenSize.x - currentWidth) < float.Epsilon &&
                   Math.Abs(screenSize.y - currentHeight) < float.Epsilon;
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }


        public static T LoadFromResources<T>(string itemPath, object who) where T : notnull, Object
        {
            var item = Resources.Load<T>(itemPath);

            if (item != null) return item;

            throw new FileLoadException($"Unable to load [{itemPath}] Who: " + who);
        }
    }
}

// public static void OffSystem()
// {
//     var world = World.DefaultGameObjectInjectionWorld;
//     var a =World.DefaultGameObjectInjectionWorld.GetExistingSystem<EditModePanelSystem>();
//     ref SystemState state = ref world.Unmanaged.ResolveSystemStateRef(a);
//     state.Enabled = false;
// }