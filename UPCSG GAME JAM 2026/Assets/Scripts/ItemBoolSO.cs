using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/Item State")]
public class BoolItemSO : ScriptableObject
{
    public bool value;

    public event Action<bool> OnValueChanged;

    void OnEnable()
    {
        value = false;
    }

    public void Set(bool newValue)
    {
        if (value == newValue) return;

        value = newValue;
        OnValueChanged?.Invoke(value);
    }
}
