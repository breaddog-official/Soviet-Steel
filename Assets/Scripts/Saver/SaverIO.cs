using Cysharp.Threading.Tasks;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SaverIO", menuName = "Scripts/Savers/SaverIO")]
    public class SaverIO : Saver
    {
        public override void Save(string path, string value)
        {
            File.WriteAllText(path, value);
        }

        public override string Load(string path)
        {
            return File.ReadAllText(path);
        }


        public override bool Exists(string path) => File.Exists(path);
#if YandexGamesPlatform_yg
        public override bool IsAvailable() => false;
#else
        public override bool IsAvailable() => SaveManager.SupportIO;
#endif



        public async override UniTask SaveAsync(string path, string value, CancellationToken token = default)
        {
            await File.WriteAllTextAsync(path, value, token);
        }

        public async override UniTask<string> LoadAsync(string path, CancellationToken token = default)
        {
            return await File.ReadAllTextAsync(path, token);
        }
    }
}
