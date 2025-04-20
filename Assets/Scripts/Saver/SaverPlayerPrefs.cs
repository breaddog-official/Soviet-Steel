using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SaverPlayerPrefs", menuName = "Scripts/Savers/SaverPlayerPrefs")]
    public class SaverPlayerPrefs : Saver
    {
        public override void Save(string path, string value)
        {
            PlayerPrefs.SetString(ProcessPath(path), value);
            PlayerPrefs.Save();
        }

        public override string Load(string path)
        {
            return PlayerPrefs.GetString(ProcessPath(path));
        }

        public override bool Exists(string path) => PlayerPrefs.HasKey(ProcessPath(path));
        private string ProcessPath(string path) => path.GetHashCode().ToString();
    }
}
