using System;
using System.Collections.Generic;
using ArcadeVP;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.SplitScreen
{
    public class SplitScreenSpawner : MonoBehaviour
    {
        [SerializeField] private Camera defaultCamera;

        private readonly List<Camera> cameras = new();



        private void Start()
        {
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            for (int i = 0; i < SplitScreenManager.GetPlayers().Count; i++)
            {
                var splitScreenPlayer = SplitScreenManager.GetPlayers()[i];
                var player = NetworkManagerExt.instance.SpawnPlayer(new NetworkManagerExt.ConnectMessage
                {
                    name = splitScreenPlayer.name,
                    carHash = splitScreenPlayer.carHash
                }, (player) => ConfigurePlayer(player, splitScreenPlayer, i));

                var camera = i == 0 ? defaultCamera : Instantiate(defaultCamera, defaultCamera.transform.parent);

                ConfigureCamera(camera, splitScreenPlayer, i);
                cameras.Add(camera);
            }
        }

        protected virtual void ConfigurePlayer(GameObject player, SplitScreenPlayer splitScreenPlayer, int index)
        {
            if (player.TryGetComponent(out ArcadeVehicleNetwork network))
            {
                network.SetSplitScreenPlayer(splitScreenPlayer, index);
            }
        }

        protected virtual void ConfigureCamera(Camera camera, SplitScreenPlayer player, int index)
        {
            if (camera.TryGetComponent(out CinemachineBrain brain))
            {
                brain.ChannelMask = GetChannel(index);
            }
        }

        public static OutputChannels GetChannel(int index)
        {
            return (OutputChannels)(int)Math.Pow(2, index + 1);
        }
    }
}