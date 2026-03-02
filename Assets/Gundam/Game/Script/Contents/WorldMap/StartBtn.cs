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
        if (curNode.NodeSO == null)
        {
            Debug.LogWarning("이동할 맵을 선택하지 않음");
            return;
        }
        ScenesManager.Instance.LoadScene((int)curNode.NodeSO.LoadSceneType);
    }

    public void GoToMain()
    {
        PlayerInfoManager.Instance.Clear();
        StageManager.Instance.Clear();
        ScenesManager.Instance.LoadScene(0);
    }

    public void GoToWorldMap() =>ScenesManager.Instance.LoadScene(1);
}
