using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class SelectMapUI : MonoBehaviour
    {
        [SerializeField] protected MapSO[] mapsSo;
        [Space]
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text descriptionText;
        [SerializeField] protected Material screenMaterial;
        [Space]
        [SerializeField] protected Texture defaultScreenTexture;

        protected List<IMap> maps;

        protected int currentMapIndex;




        protected virtual void Awake()
        {
            maps = new(mapsSo);
        }




        private void OnEnable()
        {
            UpdateCurrentMap();
        }

        private void OnDisable()
        {
            screenMaterial.mainTexture = defaultScreenTexture;
        }




        public void UpdateCurrentMap()
        {
            var map = maps[currentMapIndex];

            nameText.text = map.Name;
            descriptionText.text = map.Description;
            screenMaterial.mainTexture = map.Icon;
        }


        public virtual void SelectMap(int index)
        {
            currentMapIndex = index;
            
            NetworkManager.singleton.onlineScene = maps[currentMapIndex].Scene;
            GameManager.GameMode.map = maps[currentMapIndex];

            UpdateCurrentMap();
        }



        public void NextMap()
        {
            SelectMap(currentMapIndex.IncreaseInBoundsReturn(maps.Count));
        }

        public void PreviousMap()
        {
            SelectMap(currentMapIndex.DecreaseInBoundsReturn(maps.Count));
        }



        public void AddMap(IMap map) => maps.Add(map);
        public bool RemoveMap(IMap map) => maps.Remove(map);

        public IReadOnlyCollection<IMap> GetMaps() => maps;
    }
}