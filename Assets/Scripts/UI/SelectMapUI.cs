using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectMapUI : MonoBehaviour
    {
        [SerializeField] protected MapSO[] mapsSo;
        [SerializeField] protected SelectMapInstanceUI mapPrefab;
        [SerializeField] protected Transform spawnParent;

        protected Dictionary<IMap, SelectMapInstanceUI> spawnedMaps;
        protected HashSet<IMap> maps;


        protected virtual void Awake()
        {
            maps = new(mapsSo);
            spawnedMaps = new();
        }



        public void UpdateInstances(bool skipSpawnCheck = false)
        {
            foreach (var map in maps)
            {
                if (!skipSpawnCheck && spawnedMaps.ContainsKey(map))
                    continue;

                var spawnedMap = Instantiate(mapPrefab, spawnParent);
                spawnedMap.Initialize(map, SelectMap);
                spawnedMaps.Add(map, spawnedMap);
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


        public virtual void SelectMap(IMap map)
        {
            NetworkManager.singleton.onlineScene = map.Scene;
        }



        public bool AddMap(IMap map) => maps.Add(map);
        public bool RemoveMap(IMap map) => maps.Remove(map);

        public IReadOnlyCollection<IMap> GetMaps() => maps;
    }
}