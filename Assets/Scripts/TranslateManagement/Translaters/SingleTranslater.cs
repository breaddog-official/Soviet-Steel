using System.Reflection;
using Unity.Burst;
using UnityEngine;

namespace Scripts.TranslateManagement
{
    [BurstCompile]
    public abstract class SingleTranslater : Translater
    {
        [field: SerializeField]
        public string Name { get; private set; }
        public string TranslationString => TranslateManager.GetTranslationString(Name);


        protected override void Awake()
        {
            if (string.IsNullOrEmpty(Name))
                return;

            base.Awake();
        }

        public virtual void SetName(string name, bool withInvoke = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            Name = name;

            if (withInvoke)
                ChangeElement();
        }
    }
}
