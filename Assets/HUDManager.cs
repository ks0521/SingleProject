using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    public UIPresenter presenter;
    public event Action OnPlayerActived;
    private void Awake()
    {
        Instance = this;
        presenter = GetComponent<UIPresenter>();
    }

    public void PlayerActivated()
    {
        if (presenter is null) presenter = GetComponent<UIPresenter>();
        Debug.Log("hud event Invoke");
        //presenter.Init();
        OnPlayerActived?.Invoke();
    }
}
