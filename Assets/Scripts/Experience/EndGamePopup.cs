using Scripts.Gameplay.Experience;
using Scripts.SaveManagement;
using UnityEngine;

namespace Scripts.UI
{
    public class EndGamePopup : MonoBehaviour
    {
        [SerializeField] private int endGameLevel = 12;
        [SerializeField] private GameObject popup;
        [Space]
        [SerializeField] private Saver saver;

        private const string key = "end_game_showed";


        private void Start()
        {
            if (ExperienceManager.GetCurrentLevel() < endGameLevel)
                return;

            if (saver.Exists(key) && bool.Parse(saver.Load(key)) == true)
                return;

            popup.SetActive(true);
            saver.Save(key, true.ToString());
        }
    }
}