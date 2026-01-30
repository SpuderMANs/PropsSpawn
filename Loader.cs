using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Enums;
using Exiled.API.Features;
using Newtonsoft.Json;
using PropSpawn.Struct;
using UnityEngine;

namespace PropSpawn
{
    public static class Loader
    {
        private static readonly string BaseFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EXILED", "Configs", "Plugins", "PropSpawn"
        );

        private static readonly string SavedMapFolderPath =
            Path.Combine(BaseFolderPath, "SavedMap");

        public static void Initialize()
        {
            try
            {
                Directory.CreateDirectory(BaseFolderPath);
                Directory.CreateDirectory(SavedMapFolderPath);
                Log.Info("Save folder ready.");
            }
            catch (Exception e)
            {
                Log.Error($"Init error: {e}");
            }
        }


        public static bool SaveMap(string mapName, List<PropSaveData> props, out string error)
        {
            error = string.Empty;

            try
            {
                var data = new MapSaveData
                {
                    MapName = mapName
                };

                foreach (var p in props)
                {
                    data.Props.Add(p); 
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string path = Path.Combine(SavedMapFolderPath, $"{mapName}.json");

                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }



        public static List<(string name, Vector3 position, Vector3 rotation)> LoadMap(string mapName, out string error)
        {
            error = string.Empty;
            var result = new List<(string, Vector3, Vector3)>();

            try
            {
                string path = Path.Combine(SavedMapFolderPath, $"{mapName}.json");

                if (!File.Exists(path))
                {
                    error = "Map file not found.";
                    return result;
                }

                var data = JsonConvert.DeserializeObject<MapSaveData>(
                    File.ReadAllText(path));

                foreach (var prop in data.Props)
                {
                    Vector3 worldPos;
                    Vector3 rot = prop.Rotation.ToVector3();

                    if (!Enum.TryParse(prop.Room, out RoomType roomType))
                    {
                        Log.Warn($"Invalid room '{prop.Room}' for prop '{prop.Name}'.");
                        worldPos = prop.Offset.ToVector3();
                    }
                    else
                    {
                        Room room = Room.Get(roomType);
                        if (room == null)
                        {
                            Log.Warn($"Room '{prop.Room}' not found.");
                            worldPos = prop.Offset.ToVector3();
                        }
                        else
                        {
                            worldPos = room.Transform
                                .TransformPoint(prop.Offset.ToVector3());
                        }
                    }

                    result.Add((prop.Name, worldPos, rot));
                }

                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return result;
            }
        }
    }
}
