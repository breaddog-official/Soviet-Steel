using System.Collections.Generic;
using System.Linq;
using ArcadeVP;
using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineHelicopter : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private CinemachineTargetGroup group;

    private readonly Dictionary<Transform, ArcadeVehicleNetwork> networks = new();

    public static CinemachineHelicopter Instance { get; private set; }


    private void Awake()
    {
        Instance = this;

        cinemachineCamera = GetComponent<CinemachineCamera>();
        group = GetComponent<CinemachineTargetGroup>();

        SetState(false);
    }

    private void FixedUpdate()
    {
        UpdateTargets();
    }

    private void UpdateTargets()
    {
        group.Targets.RemoveAll(t => t == null || t.Object == null);

        if (group.Targets.Count < GameManager.Instance.RoadManager.GetPlayers().Count)
        {
            var transforms = group.Targets.Select(t => t.Object);

            foreach (var player in GameManager.Instance.RoadManager.GetPlayers())
            {
                if (player.Key.TryFindNetworkByID(out var network) && !transforms.Contains(network.transform))
                {
                    var target = new CinemachineTargetGroup.Target
                    {
                        Object = network.transform,
                        Radius = 3f,
                    };

                    group.Targets.Add(target);
                    networks.Add(network.transform, network);
                }
            }
        }

        foreach (var target in group.Targets)
        {
            float first = networks[target.Object].AI ? 0 : 0.5f;
            float second = networks[target.Object].isLocalPlayer ? 0.5f : 0f;

            target.Weight = first + second;// Mathf.Lerp(0, 1, GameManager.Instance.RoadManager.GetPlace(networks[target.Object]) / ((float)networks.Count - 1));
        }

        //cinemachineCamera.enabled = group.Targets.Count > 0 && GameManager.Instance.RoadManager.GetPlayers().Count > 0 && NetworkClient.active;
    }

    public static void SetState(bool state)
    {
        Instance.gameObject.SetActive(state);

        Instance.cinemachineCamera.enabled = state;
        Instance.group.enabled = state;

        var target = state ? Instance.transform : null;
        Instance.cinemachineCamera.Target = new CameraTarget()
        {
            TrackingTarget = target,
            LookAtTarget = target,
        };
    }
}
