using UnityEngine;
using Scripts.Extensions;
using System.Collections.Generic;

namespace Scripts.UI
{
    public abstract class SelectUI<T> : MonoBehaviour
    {
        [SerializeField] protected bool allowRoundSwitching = true;

        protected readonly List<T> values = new();

        protected int currentValueIndex;

        protected T CurrentValue => values[currentValueIndex];



        protected virtual void OnEnable()
        {
            ApplyCurrentValue();
        }




        public abstract void ApplyCurrentValue();


        public virtual void Select(int index)
        {
            currentValueIndex = index;

            ApplyCurrentValue();
        }

        public virtual void Select(T value)
        {
            if (value == null)
                return;

            currentValueIndex = values.IndexOf(value);

            ApplyCurrentValue();
        }



        public void Next()
        {
            if (!allowRoundSwitching && currentValueIndex == values.Count - 1)
                return;

            int index = currentValueIndex;
            do
            {
                index.IncreaseInBounds(values.Count);
                if (index == currentValueIndex)
                    throw new System.Exception("All values not available!");
            }
            while (!IsAvailable(index));


            Select(index);
        }

        public void Previous()
        {
            if (!allowRoundSwitching && currentValueIndex == 0)
                return;

            int index = currentValueIndex;
            do
            {
                index.DecreaseInBounds(values.Count);
                if (index == currentValueIndex)
                    throw new System.Exception("All values not available!");
            }
            while (!IsAvailable(index));


            Select(index);
        }


        public virtual bool IsAvailable(int index) => true;

        public void AddValue(T value) => values.Add(value);
        public bool RemoveValue(T value) => values.Remove(value);

        public IReadOnlyCollection<T> GetValues() => values;
    }
}