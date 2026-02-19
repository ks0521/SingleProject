using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI score;

    private void Awake()
    {
        score = GetComponent<TextMeshProUGUI>();
    }

    public void ScoreRenew(string text)
    {
        score.text = text;
    }
}
