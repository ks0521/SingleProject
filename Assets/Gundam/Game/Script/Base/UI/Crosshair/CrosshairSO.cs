using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CrosshairSO : ScriptableObject
{
    [SerializeField] private Sprite targetCrosshair;
    [SerializeField] private Sprite untargetCrosshair;

    public float untargetScale;
    public float targetScale;

    public float scaleLerfSpeed;
}