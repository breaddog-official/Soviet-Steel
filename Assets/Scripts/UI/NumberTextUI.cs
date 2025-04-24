using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    [ExecuteInEditMode]
    public class NumberTextUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueText;
        [Space]
        [SerializeField] private bool addDigits;
        [ShowIf(nameof(addDigits)), MinValue(0)]
        [SerializeField] private int digits;

        public virtual string FormatString => addDigits ? $"0.{GetZeros()}" : string.Empty;

        // Ётот ужас ввиде копипасты тут потому, что у object'а нету перегрузок ToString с более чем 0 аргументов и € не смогу сделать это через 
        // метод с аргументом object value

        public virtual void UpdateValue(uint value)
        {
            valueText.text = value.ToString(FormatString);
        }

        public virtual void UpdateValue(int value)
        {
            valueText.text = value.ToString(FormatString);
        }

        public virtual void UpdateValue(float value)
        {
            valueText.text = value.ToString(FormatString);
        }

        public virtual void UpdateValue(double value)
        {
            valueText.text = value.ToString(FormatString);
        }




        // Ќе нашЄл более элегантного решени€
        public virtual string GetZeros()
        {
            string zeros = string.Empty;

            for (int i = 0; i < digits; i++)
            {
                zeros += "0";
            }

            return zeros;
        }
    }
}
