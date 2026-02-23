using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary> 현재 선택된 노드SO를 담는 SO</summary>
[CreateAssetMenu]
public class SelectedNode : ScriptableObject
{
    public NodeSO NodeSO;
}
