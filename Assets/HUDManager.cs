using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    public event Action OnPlayerActived;
    private void Awake()
    {
        Instance = this;
    }

    public void PlayerActivated()
    {
        OnPlayerActived?.Invoke();
    }
}
