using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PropSpawn.Struct
{
    public class MapSaveData
    {
        public string MapName { get; set; }
        public List<PropSaveData> Props { get; set; } = new();
    }

    public class PropSaveData
    {
        public string Name { get; set; }
        public string Room { get; set; }
        public SerializableVector3 Offset { get; set; }
        public SerializableVector3 Rotation { get; set; }
    }

    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToVector3() => new Vector3(x, y, z);

        public static SerializableVector3 From(Vector3 v) => new()
        {
            x = v.x,
            y = v.y,
            z = v.z
        };
    }

}
