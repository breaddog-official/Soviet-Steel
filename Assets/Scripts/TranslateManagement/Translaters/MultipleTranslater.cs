using System.Collections.Generic;
using System.Reflection;
using Unity.Burst;
using UnityEngine;

namespace Scripts.TranslateManagement
{
    [BurstCompile]
    public abstract class MultipleTranslater : Translater
    {
        [field: SerializeField] 
        public string[] Names { get; private set; }

        public string GetTranslationString(int index)
            => TranslateManager.GetTranslationString(Names[index]);
    }
}
