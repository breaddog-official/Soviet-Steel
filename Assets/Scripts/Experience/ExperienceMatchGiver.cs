using ArcadeVP;
using UnityEngine;

namespace Scripts.Gameplay.Experience 
{
    public class ExperienceMatchGiver : MonoBehaviour
    {
        [SerializeField] private uint minExperience = 250u;
        [SerializeField] private uint maxExperience = 1000u;
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
                float t = GameManager.Instance.WinManager.GetPlace(ArcadeVehicleNetwork.LocalPlayerNetwork.netId) / (float)GameManager.Instance.WinManager.GetPlaces().Count;
                uint experience = (uint)Mathf.Lerp(maxExperience, minExperience, t);
                ExperienceManager.EncreaseExperience(experience);
            }
        }
    }
}