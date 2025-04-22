using UnityEngine;
using Unity.Burst;
using Cysharp.Threading.Tasks;

namespace Scripts.TranslateManagement
{
    [BurstCompile]
    public abstract class AutoTranslater : ScriptableObject
    {
        /// <summary>
        /// Performs translation of one string
        /// </summary>
        public abstract string Translate(string from, ApplicationLanguage to);

        /// <summary>
        /// Performs translation of one string asynchronously. You must add the <seealso href="async"/> keyword in the implementation
        /// </summary>
        public abstract UniTask<string> TranslateAsync(string from, ApplicationLanguage to);




        /// <summary>
        /// Performs translation
        /// </summary>
        public abstract Translation TranslateAll(Translation from, ApplicationLanguage to);

        /// <summary>
        /// Performs translation asynchronously. You must add the <seealso href="async"/> keyword in the implementation
        /// </summary>
        public abstract UniTask<Translation> TranslateAllAsync(Translation from, ApplicationLanguage to);



        /// <summary>
        /// Returns false if the autotranslator cannot translate this language
        /// </summary>
        public virtual bool ValidForTranslate(ApplicationLanguage language) => true;
    }
}
