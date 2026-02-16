using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private SelectedNode curNode;
    
    public void StartNode()
    {
        ScenesManager.Instance.LoadScene((int)curNode.NodeSO.LoadSceneType);
    }
}
