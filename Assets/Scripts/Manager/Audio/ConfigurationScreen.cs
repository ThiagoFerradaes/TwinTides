using System;
using UnityEngine;

public class ConfigurationScreen : MonoBehaviour
{
    public event Action OnEnabled;
    private void OnEnable() {
        OnEnabled?.Invoke();
    }
}
