using Mirror;
using Scripts.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.UI
{
    public class ServerDiscoveryUI : MonoBehaviour
    {
        [SerializeField] private ServerDiscoveryInstanceUI instancePrefab;
        [SerializeField] private Transform spawnParent;

        protected readonly Dictionary<long, ServerDiscoveryInstanceUI> spawnedInstances = new();



        public void AddServer(ServerDiscovery.Response response)
        {
            ServerDiscoveryInstanceUI spawned;

            if (spawnedInstances.ContainsKey(response.serverId))
            {
                spawned = spawnedInstances[response.serverId];
            }
            else
            {
                spawned = Instantiate(instancePrefab, spawnParent);
                spawnedInstances.Add(response.serverId, spawned);
            }
            
            spawned.SetValues(response.name, response.playersCount, response.maxPlayers, response.serverId, this, response.uri);
        }

        public void RemoveServer(long key)
        {
            spawnedInstances.Remove(key);
        }


        public void ConnectServer(Uri uri)
        {
            NetworkManager.singleton.StartClient(uri);
        }
    }
}