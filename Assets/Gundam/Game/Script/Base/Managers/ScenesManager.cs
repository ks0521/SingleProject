using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Base.Managers
{
    public enum Scenes
    {
        MainMenu = 0,
        WorldMap = 1,
        Battle = 2,
        Elite=3,
        Boss=4,
        Store = 5,
        Loading = 6,
        Length = 7
    }
    
    public class ScenesManager : MonoBehaviour
    {
        public static ScenesManager Instance;
        [Header("LOADING SCREEN")]
        [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
        public bool waitForInput = true;
        public GameObject loadingMenu;
        [Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }

        /// <summary> 씬 이동</summary>
        /// <param name="scene"> 이동할 씬 번호</param>
        public void LoadScene(int scene){
            if(scene >=0 && scene < (int)Scenes.Length)
            {
                SceneManager.LoadScene(scene);
            }
        }
    }
}