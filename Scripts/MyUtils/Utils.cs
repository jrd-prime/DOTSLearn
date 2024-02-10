﻿using System;
using System.Collections.Generic;
using System.IO;
using Jrd.Gameplay.Products;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Jrd.MyUtils
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


        public static bool IsScreenSizeChanged(float width, float height)
        {
            return Math.Abs(width - UnityEngine.Screen.width) < float.Epsilon &&
                   Math.Abs(height - UnityEngine.Screen.height) < float.Epsilon;
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

        public static NativeParallelHashMap<int, int> NativeListToHashMap(NativeList<ProductData> nativeList)
        {
            NativeParallelHashMap<int, int> nativeParallelHashMap = new(nativeList.Length, Allocator.Persistent);

            foreach (var product in nativeList)
            {
                nativeParallelHashMap.Add((int)product.Name, 0);
            }

            return nativeParallelHashMap;
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


// public partial struct MapRespawnSystem : ISystem
// {
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         foreach (var (mapBuffer, entity) in SystemAPI.Query<DynamicBuffer<MapElement>>().WithAll<MapEntities>().WithEntityAccess())
//         {
//             for (int i = 0; i < mapBuffer.Length; i++)
//             {
//                 state.EntityManager.Instantiate(mapBuffer[i].MapEntity);
//             }
//         }
//         state.Enabled = false;
//     }
// }