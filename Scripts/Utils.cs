using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Jrd
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
            return new Vector2(Screen.width / 2f, Screen.height / 2f);
        }


        public static bool IsScreenSizeChanged(float width, float height)
        {
            return Math.Abs(width - Screen.width) < float.Epsilon && Math.Abs(height - Screen.height) < float.Epsilon;
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