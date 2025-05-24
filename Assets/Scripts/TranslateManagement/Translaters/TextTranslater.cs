using TMPro;
using Unity.Burst;
using UnityEngine;

namespace Scripts.TranslateManagement
{
    [BurstCompile]
    [AddComponentMenu("Translaters/TextMeshPro Translater")]
    public class TextTranslater : SingleTranslater
    {
        public TMP_Text Text { get; private set; }

        protected override void Awake()
        {
            Text = GetComponent<TMP_Text>();

            base.Awake();
        }
        public override void ChangeElement()
        {
            if (Text != null)
                Text.SetText(TranslationString);
        }
    }
}
