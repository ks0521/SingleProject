using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [field:SerializeField]
    public int Stage { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public void SetStage(int stage)
    {
        Stage = stage;
    }
    public void StageProgress() => Stage++;
}
