using Mirror;
using Scripts.Gameplay;
using Scripts.Gameplay.Experience;
using Scripts.TranslateManagement;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectMapUI : SelectUI<Map>
    {
        [Space]
        [SerializeField] protected TextTranslater nameText;
        [SerializeField] protected TextTranslater descriptionText;
        [Space]
        [SerializeField] protected Material screenMaterial;
        [SerializeField] protected Texture defaultScreenTexture;


        protected virtual void Awake()
        {
            values.AddRange(NetworkManagerExt.instance.registeredMaps.Select(m => m.map));
            
            Select(values.Where(v => v.Scene == GameManager.GameMode.Map?.Scene).FirstOrDefault());
        }

        private void OnDisable()
        {
            screenMaterial.mainTexture = defaultScreenTexture;
        }




        public override void ApplyCurrentValue()
        {
            var map = CurrentValue;

            NetworkManager.singleton.onlineScene = map.Scene;
            GameManager.GameMode.mapHash = map.MapHash;

            nameText.SetName(map.TranslateName);
            descriptionText.SetName(map.TranslateDescription);
            screenMaterial.mainTexture = map.Icon;
        }

        public override bool IsAvailable(int index)
        {
            return values[index].Level <= ExperienceManager.GetCurrentLevel();
        }
    }
}