using Exiled.API.Features;
using GameCore;
using Mirror;
using PropSpawn.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace PropSpawn.EventHandler
{
    public class EventHandlers
    {
        public void RegisterEvent()
        {
            Exiled.Events.Handlers.Server.RoundStarted += WhenRoundStart;
        }
        public void UnregisterEvent()
        {
            Exiled.Events.Handlers.Server.RoundStarted += WhenRoundStart;
        }

        private void WhenRoundStart()
        {
            string filename = Main.Instance.Config.MapToLoad;
            var loadedProps = Loader.LoadMap(filename, out string error);

            if (!string.IsNullOrEmpty(error))
            {
                Exiled.API.Features.Log.Error($"Error loading props: {error}");
                return;
            }

            int loadedCount = 0;

            foreach (var p in loadedProps)
            {
                GameObject prefab = GetPrefabByPartialName(p.name);
                if (prefab == null)
                {
                    Exiled.API.Features.Log.Warn($"Prefab '{p.name}' not found.");
                    continue;
                }

                GameObject instance = UnityEngine.Object.Instantiate(prefab, p.position, Quaternion.Euler(p.rotation));
                NetworkServer.Spawn(instance);
                loadedCount++;
            }

            Exiled.API.Features.Log.Info($"Loaded {loadedCount} props from '{filename}.txt'.");
        }

        private GameObject GetPrefabByPartialName(string partialName)
        {
            partialName = partialName.ToLower().Replace(" ", "");

            foreach (var prefab in Mirror.NetworkClient.prefabs.Values)
            {
                if (prefab == null) continue;

                string prefabName = prefab.name.ToLower().Replace(" ", "");

                if (prefabName.Contains(partialName))
                    return prefab;
            }

            return null;
        }
    }
}
