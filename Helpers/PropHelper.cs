using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using MEC;
using Mirror;
using Newtonsoft.Json.Linq;
using PropSpawn.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropSpawn.Helpers
{
    public static class PropHelper
    {
        private static readonly Dictionary<Player, List<GameObject>> SpawnedProps = new();

        private static readonly Dictionary<Player, string> SelectedProp = new();
        private static readonly Dictionary<Player, GameObject> MovingProp = new();
        private static readonly Dictionary<GameObject, GameObject> PrefabLookup = new();




        public static void Set(Player player, string propName)
        {
            if (player == null) return;
            SelectedProp[player] = propName;
        }

        public static string Get(Player player)
        {
            if (player == null) return null;
            return SelectedProp.TryGetValue(player, out var prop)
                ? prop
                : null;
        }

        public static void Clear(Player player)
        {
            if (player == null) return;
            SelectedProp.Remove(player);
        }

        public static bool TrySpawn(Player player, string propName, Vector3 position)
        {
            GameObject prefab = GetPrefabByPartialName(propName);

            if (prefab == null)
            {
                Log.Debug($"Prefab matching '{propName}' not found");
                return false;
            }
            GameObject spawned = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
            NetworkServer.Spawn(spawned);
            PrefabLookup[spawned] = prefab;
            Log.Debug($"Spawned prefab '{prefab}' for player '{player.Nickname}' at {position}.");

            if (SpawnedProps.ContainsKey(player))
                SpawnedProps[player].Add(spawned);
            else
                SpawnedProps[player] = new List<GameObject> { spawned };

            return true;
        }

        public static bool TryDelete(Player player)
        {
            var camera = player.CameraTransform;
            Vector3 origin = camera.position;
            Vector3 direction = camera.forward;

            LayerMask mask = ~LayerMask.GetMask("Player", "Hitbox");

            if (!Physics.Raycast(origin, direction, out RaycastHit hit, 100f, mask))
                return false;


            Vector3 toHit = hit.point - origin;
            if (Vector3.Dot(direction, toHit.normalized) < 0.5f)
                return false;

            GameObject hitObject = hit.collider.gameObject;
            GameObject root = hitObject.transform.root.gameObject;

            if (!SpawnedProps.TryGetValue(player, out var props))
                return false;

            for (int i = props.Count - 1; i >= 0; i--)
            {
                if (props[i] == null)
                {
                    props.RemoveAt(i);
                    continue;
                }

                if (props[i] == root)
                {
                    UnityEngine.Object.Destroy(root);
                    props.RemoveAt(i);

                    Log.Debug($"Deleted prop '{root.name}' for {player.Nickname}");
                    return true;
                }
            }

            return false;
        }

        public static void TryMove(Player player)
        {
            var cam = player.CameraTransform;
            const float maxDistance = 100f;
            const float offset = 0.1f;
            LayerMask mask = ~LayerMask.GetMask("Player", "Hitbox");


            if (MovingProp.TryGetValue(player, out var moving) && moving != null)
            {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, mask))
                {
                    Vector3 normal = hit.normal;
                    if (Vector3.Dot(normal, cam.forward) > 0f)
                        normal = -normal;

                    if (!PrefabLookup.TryGetValue(moving, out var prefab))
                    {
                        Log.Debug("Prefab null!");
                        return;
                    }

                    NetworkServer.Destroy(moving);
                    MovingProp.Remove(player);

                    GameObject newProp = UnityEngine.Object.Instantiate(prefab, hit.point + normal * offset, Quaternion.identity);
                    NetworkServer.Spawn(newProp);
                    PrefabLookup.Add(newProp, prefab);

                    HudHelper.SetAction(player, $"Moved {newProp.name} to {newProp.transform.position.x:F2}, {newProp.transform.position.y:F2}, {newProp.transform.position.z:F2}");
                    Log.Debug($"Released and respawned prop for {player.Nickname}");

                    if (!SpawnedProps.TryGetValue(player, out var list))
                        SpawnedProps[player] = new List<GameObject> { newProp };
                    else
                        list.Add(newProp);

                    Timing.CallDelayed(4f, () => HudHelper.SetAction(player, $""));
                }

                return;
            }




            if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit newHit, maxDistance, mask) ||
                Vector3.Dot(cam.forward, (newHit.point - cam.position).normalized) < 0.5f)
            {
                Log.Debug("Invalid raycast or bad angle");
                return;
            }

            GameObject root = newHit.collider.transform.root.gameObject;

            if (!SpawnedProps.TryGetValue(player, out var props) || !props.Contains(root))
            {
                Log.Debug("Hit object is not a valid spawned prop");
                return;
            }

            MovingProp[player] = root;
            HudHelper.SetAction(player, $"Attached {root.name}");
            Log.Debug($"Attached '{root.name}'");
        }


        public static void TryRotate(Player player, Vector3 angle)
        {
            var cam = player.CameraTransform;
            const float maxDistance = 100f;
            LayerMask mask = ~LayerMask.GetMask("Player", "Hitbox");

            if (MovingProp.TryGetValue(player, out var moving) && moving != null)
            {
                if (!PrefabLookup.TryGetValue(moving, out var prefab))
                    return;

                Vector3 spawnPosition = moving.transform.position;
                Quaternion newRotation = moving.transform.rotation * Quaternion.Euler(angle);

                NetworkServer.Destroy(moving);
                MovingProp.Remove(player);

                GameObject newProp = UnityEngine.Object.Instantiate(prefab, spawnPosition, newRotation);
                NetworkServer.Spawn(newProp);
                PrefabLookup.Add(newProp, prefab);

                if (!SpawnedProps.TryGetValue(player, out var list))
                    SpawnedProps[player] = new List<GameObject> { newProp };
                else
                {
                    list.RemoveAll(p => p == moving);
                    list.Add(newProp);
                }

                MovingProp[player] = newProp;
                HudHelper.SetAction(player, $"Rotated {newProp.name} to {newProp.transform.eulerAngles.x:F1}, {newProp.transform.eulerAngles.y:F1}, {newProp.transform.eulerAngles.z:F1}");
                Timing.CallDelayed(4f, () => HudHelper.SetAction(player, $""));
                return;
            }

            if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, mask) ||
                Vector3.Dot(cam.forward, (hit.point - cam.position).normalized) < 0.5f)
                return;

            GameObject root = hit.collider.transform.root.gameObject;

            if (!SpawnedProps.TryGetValue(player, out var props) || !props.Contains(root))
                return;

            MovingProp[player] = root;
        }




        public static bool TryList(out string response)
        {
            var prefabNames = NetworkClient.prefabs.Values
                .Where(p => p != null)
                .Select(p => p.name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            response = "Available props: " + string.Join(", ", prefabNames);
            return true;
        }

        public static bool TrySave(Player player, string filename, out string response)
        {
            if (!SpawnedProps.TryGetValue(player, out var props) || props.Count == 0)
            {
                response = "No spawned props to save.";
                return false;
            }

            var allProps = new List<PropSaveData>();

            foreach (var go in props)
            {
                if (go == null) continue;

                Room room = Room.Get(go.transform.position);
                if (room == null) continue;

                allProps.Add(new PropSaveData
                {
                    Name = go.name.Replace("(Clone)", "").Trim(),
                    Room = room.Type.ToString(),
                    Offset = SerializableVector3.From(room.Transform.InverseTransformPoint(go.transform.position)),
                    Rotation = SerializableVector3.From(go.transform.eulerAngles)
                });
            }

            if (allProps.Count == 0)
            {
                response = "No valid props to save.";
                return false;
            }

            bool success = Loader.SaveMap(filename, allProps, out string error);
            response = success
                ? $"Props saved to '{filename}.json'."
                : $"Error saving props: {error}";
            return success;
        }




        public static bool TryLoad(Player player, string filename, out string response)
        {
            var loadedProps = Loader.LoadMap(filename, out string error);

            if (!string.IsNullOrEmpty(error))
            {
                response = $"Error loading props: {error}";
                return false;
            }

            int loadedCount = 0;

            foreach (var p in loadedProps)
            {
                GameObject prefab = GetPrefabByPartialName(p.name);
                if (prefab == null)
                {
                    Log.Warn($"Prefab '{p}' not found.");
                    continue;
                }
                Vector3 pos = p.position ;
                Quaternion rot = Quaternion.Euler(p.rotation);

                GameObject instance = UnityEngine.Object.Instantiate(prefab, pos, rot);
                NetworkServer.Spawn(instance);
                SpawnedProps.Add(player, new List<GameObject> { instance });
                Log.Debug($"Loaded {p} ({p.position.x},{p.position.y},{p.position.z})  from '{filename}.json'.");
                loadedCount++;
            }

            response = $"Loaded {loadedCount} props from '{filename}.json'.";
            Log.Debug($"Loaded {loadedCount} props from '{filename}.json'.");
            return true;
        }


        public static GameObject GetPrefabByPartialName(string partialName)
        {
            partialName = partialName.ToLower().Replace(" ", "");

            foreach (var prefab in NetworkClient.prefabs.Values)
            {
                if (prefab == null) continue;
                if (prefab.name.ToLower().Replace(" ", "").Contains(partialName))
                    return prefab;
            }

            return null;
        }

    }
}
