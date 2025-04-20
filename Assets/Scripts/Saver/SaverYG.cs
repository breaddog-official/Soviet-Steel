using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SaverYG", menuName = "Scripts/Savers/SaverYG")]
    public class SaverYG : Saver
    {
        public override void Save(string path, string value)
        {
            RedefineYG.PlayerPrefs.SetString(ProcessPath(path), value);
            RedefineYG.PlayerPrefs.Save();
        }

        public override string Load(string path)
        {
            return RedefineYG.PlayerPrefs.GetString(ProcessPath(path));
        }

        public override bool Exists(string path) => PlayerPrefs.HasKey(ProcessPath(path));
        private string ProcessPath(string path) => path.GetHashCode().ToString();

        public override bool IsAvailable() => Application.platform == RuntimePlatform.WebGLPlayer;
    }
}
