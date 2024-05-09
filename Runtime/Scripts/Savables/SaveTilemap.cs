using HHG.Common.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HHG.SaveSystem.Runtime
{
    [RequireComponent(typeof(Saver), typeof(TilemapExporter))]
    public class SaveTilemap : MonoBehaviour, ISavable
    {
        [System.Serializable]
        public class Data : SavableData
        {
            public List<string> Tiles = new List<string>();
            public List<SerializableTilemap> Tilemaps = new List<SerializableTilemap>();
        }

        public string Id => id;

        [SerializeField] private string id = System.Guid.NewGuid().ToString();

        private Lazy<TilemapExporter> _exporter = new Lazy<TilemapExporter>();
        private TilemapExporter exporter => _exporter.FromComponent(this);

        public void Load(SavableData saveData)
        {
            Data data = saveData as Data;
            TilemapAsset tilemap = ScriptableObject.CreateInstance<TilemapAsset>();
            tilemap.Initiialize(data.Tiles.Select(t => AssetRegistry.GetAsset<TileBase>(t)), data.Tilemaps);
            exporter.Load(tilemap);
        }

        public SavableData Save()
        {
            TilemapAsset tilemap = ScriptableObject.CreateInstance<TilemapAsset>();
            exporter.Save(tilemap);
            Data data = new Data
            {
                Id = id,
                Tiles = tilemap.Tiles.Select(t => AssetRegistry.GetGuid(t)).ToList(),
                Tilemaps = tilemap.Tilemaps.ToList()
            };
            Destroy(tilemap);
            return data;
        }
    }
}