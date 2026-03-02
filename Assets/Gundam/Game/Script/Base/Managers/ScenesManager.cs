using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        Store = 3,
        ClearScene = 4,
        Length = 5
    }

    public enum LoadType
    {
        Default,
        BattleClear
    }
    /// <summary> 씬 이동과 이동시 이펙트 관리</summary>
    public class ScenesManager : MonoBehaviour
    {
        public static ScenesManager Instance;
        private bool _isLoading;
        public bool canPopUpReward;
        private CanvasGroup _clearHUD;
        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void GameClear()
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene((int)Scenes.ClearScene);
        }
        /// <summary> 씬 이동</summary>
        /// <param name="scene"> 이동할 씬 번호</param>
        public void LoadScene(int scene){
            LoadScene(scene,LoadType.Default);
        }

        public void LoadScene(int scene, LoadType loadType)
        {
            if (scene < 0 || scene >= (int)Scenes.Length)
            {
                Debug.LogWarning("Out of SceneRange");
                return;
            }
            StageManager.Instance.EnterStage((Scenes)scene);
            if (StageManager.Instance.Stage > 6)
            {
                GameClear();
                return;
            }
            if (loadType == LoadType.Default)
            {
                SceneManager.LoadScene(scene);
            }

            switch (loadType)
            {
                case LoadType.BattleClear:
                    _clearHUD = GameObject.Find("ClearHUD")?.GetComponent<CanvasGroup>();
                    LoadSceneAsync(scene, this.GetCancellationTokenOnDestroy()).Forget();
                    break;
                default:
                    break;
            }
        }
        async UniTaskVoid LoadSceneAsync(int scene, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f),DelayType.UnscaledDeltaTime, cancellationToken: token);
            if (_clearHUD != null)
            {
                Fade(_clearHUD,0f, 1f, 0.6f,token).Forget();
            }
            var op = SceneManager.LoadSceneAsync(scene);
            op.allowSceneActivation = false;
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f), DelayType.UnscaledDeltaTime, cancellationToken: token);
            await UniTask.WaitUntil(() => op.progress >= 0.9f, cancellationToken: token);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            op.allowSceneActivation = true;
        }

        async UniTaskVoid Fade(CanvasGroup cg, float from, float to, float duration,CancellationToken token)
        {
            cg.blocksRaycasts = true;
            cg.interactable = true;
            cg.alpha = from;
            var tween = cg.DOFade(to, duration).
                           SetUpdate(true).SetLink(cg.gameObject);
            await tween.AsyncWaitForCompletion();
        }
    }
}