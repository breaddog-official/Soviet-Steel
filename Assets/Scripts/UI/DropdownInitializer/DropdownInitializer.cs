using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DropdownInitializer : MonoBehaviour
{
    [SerializeField] protected bool initializeOnAwake;
    protected TMP_Dropdown dropdown;

    protected virtual void Awake()
    {
        if (initializeOnAwake)
            InitializeDropdown();
    }

    public virtual void InitializeDropdown()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
}
