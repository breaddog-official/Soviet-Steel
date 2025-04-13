using Mirror;
using Scripts.Gameplay;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectMapSpawnerUI : SelectUI<Map>
    {
        [SerializeField] protected MapSO[] mapsSo;
        [SerializeField] protected SelectMapInstanceUI mapPrefab;
        [SerializeField] protected Transform spawnParent;

        protected Dictionary<Map, SelectMapInstanceUI> spawnedMaps;


        protected virtual void Awake()
        {
            values.AddRange(mapsSo.Select(m => m.map));
            spawnedMaps = new();

            UpdateInstances(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateInstances();
        }



        public void UpdateInstances(bool skipSpawnCheck = false)
        {
            foreach (var map in values)
            {
                if (skipSpawnCheck || !spawnedMaps.TryGetValue(map, out SelectMapInstanceUI spawnedMap))
                {
                    spawnedMap = Instantiate(mapPrefab, spawnParent);
                    spawnedMap.Initialize(map, Select);
                    spawnedMaps.Add(map, spawnedMap);
                }
                
                // Mirror stores scenes in paths
                spawnedMap.SetSelectState(map.Scene == Path.GetFileNameWithoutExtension(NetworkManager.singleton.onlineScene));
            }
        }

        public void RespawnInstances()
        {
            RemoveInstances();
            UpdateInstances(true);
        }

        public void RemoveInstances()
        {
            foreach (var map in spawnedMaps.Values)
            {
                Destroy(map.gameObject);
            }

            spawnedMaps.Clear();
        }


        public override void ApplyCurrentValue()
        {
            NetworkManager.singleton.onlineScene = CurrentValue.Scene;
            GameManager.GameMode.mapHash = CurrentValue.MapHash;

            UpdateInstances();
        }
    }
}