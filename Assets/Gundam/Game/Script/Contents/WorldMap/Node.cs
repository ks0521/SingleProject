using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField] private NodeSO node;
    [SerializeField] private SelectedNode curNode;
    [SerializeField] private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SelectNode);
    }

    public void SelectNode()
    {
        curNode.NodeSO = node;
    }
}
