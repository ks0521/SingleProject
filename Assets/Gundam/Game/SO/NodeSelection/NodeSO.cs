using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using UnityEngine;

/// <summary> 각 노드의 정보를 담는 SO </summary>
[CreateAssetMenu]
public class NodeSO : ScriptableObject
{
    public Scenes LoadSceneType; //해당 노드 선택시 이동하는 맵
    public string NodeType; 
    public string PrimaryInfo;
    public string SecondaryInfo;
    public string Environment;
    public string TacticalNote;
}
