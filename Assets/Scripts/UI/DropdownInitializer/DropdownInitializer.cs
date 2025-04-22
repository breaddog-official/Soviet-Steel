using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class DropdownInitializer : MonoBehaviour
{
    protected TMP_Dropdown dropdown;

    private void Awake()
    {
        InitializeDropdown();
    }

    public virtual void InitializeDropdown()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
}
