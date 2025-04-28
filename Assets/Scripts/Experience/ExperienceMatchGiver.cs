using ArcadeVP;
using UnityEngine;

namespace Scripts.Gameplay.Experience 
{
    public class ExperienceMatchGiver : MonoBehaviour
    {
        [SerializeField] private uint minExperience = 250u;
        [SerializeField] private uint maxExperience = 1000u;
        [Space]
        [SerializeField] private float minRoadMultiplier = 0.5f;
        [SerializeField] private float maxRoadMultiplier = 1.5f;
        [SerializeField] private float minRoadLength = 1000f;
        [SerializeField] private float maxRoadLength = 10000f;
        [Space]
        [SerializeField] private bool checkOnEnable = true;


        private void OnEnable()
        {
            if (GameManager.Instance.WinManager != null)
                GameManager.Instance.WinManager.OnWin += OnWin;

            if (checkOnEnable)
                OnWin(ArcadeVehicleNetwork.LocalPlayerNetwork.netId);
        }

        private void OnDisable()
        {
            if (GameManager.Instance.WinManager != null)
                GameManager.Instance.WinManager.OnWin -= OnWin;
        }


        private void OnWin(uint netId)
        {
            if (ArcadeVehicleNetwork.LocalPlayerNetwork != null && ArcadeVehicleNetwork.LocalPlayerNetwork.netId == netId)
            {
                var experience = GetExperience() * GetMultiplier();
                ExperienceManager.EncreaseExperience((uint)experience);
            }
        }

        private uint GetExperience()
        {
            float t = GameManager.Instance.WinManager.GetPlace(ArcadeVehicleNetwork.LocalPlayerNetwork.netId) / (float)GameManager.Instance.WinManager.GetPlaces().Count;
            return (uint)Mathf.Lerp(maxExperience, minExperience, t);
        }

        private float GetMultiplier()
        {
            float t = Mathf.Max(GameManager.Instance.RoadManager.Distance, minRoadLength) / maxRoadLength;
            return Mathf.Lerp(minRoadMultiplier, maxRoadMultiplier, t);
        }
    }
}