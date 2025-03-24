using Mirror;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectMapUI : MonoBehaviour
    {
        [SerializeField] protected MapSO[] mapsSo;
        [SerializeField] protected SelectMapInstanceUI mapPrefab;
        [SerializeField] protected Transform spawnParent;

        protected Dictionary<IMap, SelectMapInstanceUI> spawnedMaps;
        protected List<IMap> maps;


        protected virtual void Awake()
        {
            maps = new(mapsSo);
            spawnedMaps = new();

            UpdateInstances(true);
        }

        protected virtual void OnEnable()
        {
            UpdateInstances();
        }



        public void UpdateInstances(bool skipSpawnCheck = false)
        {
            foreach (var map in maps)
            {
                if (skipSpawnCheck || !spawnedMaps.TryGetValue(map, out SelectMapInstanceUI spawnedMap))
                {
                    spawnedMap = Instantiate(mapPrefab, spawnParent);
                    spawnedMap.Initialize(map, SelectMap);
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


        public virtual void SelectMap(IMap map)
        {
            NetworkManager.singleton.onlineScene = map.Scene;

            UpdateInstances();
        }



        public void AddMap(IMap map) => maps.Add(map);
        public bool RemoveMap(IMap map) => maps.Remove(map);

        public IReadOnlyCollection<IMap> GetMaps() => maps;
    }
}