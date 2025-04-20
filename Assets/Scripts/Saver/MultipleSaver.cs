using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "MultipleSaver", menuName = "Scripts/Savers/MultipleSaver")]
    public class MultipleSaver : Saver
    {
        public Saver[] savers;

        private Saver Saver => savers.Where(s => s.IsAvailable()).FirstOrDefault();


        public override void Save(string path, string value) => Saver.Save(path, value);
        public override string Load(string path) => Saver.Load(path);


        public override bool Exists(string path) => Saver.Exists(path);
        public override bool IsAvailable() => Saver != null;


        public async override UniTask SaveAsync(string path, string value, CancellationToken token = default) => await Saver.SaveAsync(path, value, token);
        public async override UniTask<string> LoadAsync(string path, CancellationToken token = default) => await Saver.LoadAsync(path, token);
    }
}
