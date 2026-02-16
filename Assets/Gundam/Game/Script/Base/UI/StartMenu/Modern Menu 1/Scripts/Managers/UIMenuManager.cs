using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Base.Managers;

namespace SlimUI.ModernMenu{
	public class UIMenuManager : MonoBehaviour {
		private Animator CameraObject;

		// campaign button sub menu
        [Header("MENUS")]
        [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;
        [Tooltip("THe first list of buttons")]
        public GameObject firstMenu;
        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject playMenu;
        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;
        [Tooltip("Optional 4th Menu")]
        public GameObject extrasMenu;

        public enum Theme {custom1, custom2, custom3};
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;
        [Tooltip("The UI Panel that holds the CONTROLS window tab")]
        public GameObject PanelControls;
        [Tooltip("The UI Panel that holds the VIDEO window tab")]
        public GameObject PanelVideo;
        [Tooltip("The UI Panel that holds the GAME window tab")]
        public GameObject PanelGame;
        [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
        public GameObject PanelKeyBindings;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
        public GameObject PanelMovement;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
        public GameObject PanelCombat;
        [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
        public GameObject PanelGeneral;
        

        // highlights in settings screen
        [Header("SETTINGS SCREEN")]
        [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;
        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;
        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;
        [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
        public GameObject lineKeyBindings;
        [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineMovement;
        [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineCombat;
        [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineGeneral;

        [Header("LOADING SCREEN")]
		[Tooltip("If this is true, the loaded scene won't load until receiving user input")]
		public bool waitForInput = true;
        public GameObject loadingMenu;
		[Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;
        public TMP_Text loadPromptText;
		public KeyCode userPromptKey;

		[Header("SFX")]
        [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;
        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;
        [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

		void Start(){
			CameraObject = transform.GetComponent<Animator>();

			playMenu.SetActive(false);
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			firstMenu.SetActive(true);
			mainMenu.SetActive(true);

			SetThemeColors();
		}
		/// <summary>
		/// 테마 컬러 설정
		/// 이후 테마색을 설정하고 싶은 UI버튼이나 텍스트에 ThemedUIElement 컴포넌트를 추가하면 됨
		/// </summary>
		void SetThemeColors()
		{
			switch (theme)
			{
				case Theme.custom1:
					themeController.currentColor = themeController.custom1.graphic1;
					themeController.textColor = themeController.custom1.text1;
					themeIndex = 0;
					break;
				case Theme.custom2:
					themeController.currentColor = themeController.custom2.graphic2;
					themeController.textColor = themeController.custom2.text2;
					themeIndex = 1;
					break;
				case Theme.custom3:
					themeController.currentColor = themeController.custom3.graphic3;
					themeController.textColor = themeController.custom3.text3;
					themeIndex = 2;
					break;
				default:
					Debug.Log("Invalid theme selected.");
					break;
			}
		}
		/// <summary>게임 시작 버튼(extras창과 exit창이 열려있으면 끔)</summary>
		public void PlayCampaign(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			playMenu.SetActive(true);
		}
		//모바일에서 사용
		public void PlayCampaignMobile(){
			exitMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			playMenu.SetActive(true);
			mainMenu.SetActive(false);
		}
		/// <summary>메뉴창으로 돌아오기(play나 extras, exit메뉴 열려있으면 다 끔)</summary>
		public void ReturnMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(false);
			exitMenu.SetActive(false);
			mainMenu.SetActive(true); //모바일용
		}
		/// <summary> 씬 이동</summary>
		/// <param name="scene"> 이동할 씬 번호</param>
		/*public void LoadScene(Scenes scene){
			if((int)scene >=0 && (int)scene <= (int)Scenes.Length)
			{
				StartCoroutine(LoadAsynchronously((int)scene));
			}
		}
			*/
		public void  DisablePlayCampaign(){
			playMenu.SetActive(false);
		}
		//설정 창으로 이동
		public void Position2(){
			DisablePlayCampaign();
			CameraObject.SetFloat("Animate",1);
		}
		//게임 시작 창으로 이동
		public void Position1(){
			CameraObject.SetFloat("Animate",0);
		}
		/// <summary> 환경설정 패널 다 끄기</summary>
		void DisablePanels(){
			PanelControls.SetActive(false);
			PanelVideo.SetActive(false);
			PanelGame.SetActive(false);
			PanelKeyBindings.SetActive(false);

			lineGame.SetActive(false);
			lineControls.SetActive(false);
			lineVideo.SetActive(false);
			lineKeyBindings.SetActive(false);

			PanelMovement.SetActive(false);
			lineMovement.SetActive(false);
			PanelCombat.SetActive(false);
			lineCombat.SetActive(false);
			PanelGeneral.SetActive(false);
			lineGeneral.SetActive(false);
		}
		/// <summary> 환경설정 - Game부분 열기</summary>
		public void GamePanel(){
			DisablePanels();
			PanelGame.SetActive(true);
			lineGame.SetActive(true);
		}
		/// <summary> 환경설정 - Video부분 열기 </summary>
		public void VideoPanel(){
			DisablePanels();
			PanelVideo.SetActive(true);
			lineVideo.SetActive(true);
		}
		/// <summary> 환경설정 - Controls 부분 열기 </summary>
		public void ControlsPanel(){
			DisablePanels();
			PanelControls.SetActive(true);
			lineControls.SetActive(true);
		}
		/// <summary> 환경설정 - KeyBinding부분 열기</summary>
		public void KeyBindingsPanel(){
			DisablePanels();
			MovementPanel();
			PanelKeyBindings.SetActive(true);
			lineKeyBindings.SetActive(true);
		}
		/// <summary>Key Binding - Movement 설정창 열기</summary>
		public void MovementPanel(){
			DisablePanels();
			PanelKeyBindings.SetActive(true);
			PanelMovement.SetActive(true);
			lineMovement.SetActive(true);
		}
		/// <summary> Key Binding - Combat 설정창 열기 </summary>
		public void CombatPanel(){
			DisablePanels();
			PanelKeyBindings.SetActive(true);
			PanelCombat.SetActive(true);
			lineCombat.SetActive(true);
		}
		/// <summary> Key Binding - General 설정창 열기 </summary>
		public void GeneralPanel(){
			DisablePanels();
			PanelKeyBindings.SetActive(true);
			PanelGeneral.SetActive(true);
			lineGeneral.SetActive(true);
		}
		
		public void PlayHover(){
			hoverSound.Play();
		}

		public void PlaySFXHover(){
			sliderSound.Play();
		}

		public void PlaySwoosh(){
			swooshSound.Play();
		}

		// Are You Sure - Quit Panel Pop Up
		public void AreYouSure(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void AreYouSureMobile(){
			exitMenu.SetActive(true);
			if(extrasMenu) extrasMenu.SetActive(false);
			mainMenu.SetActive(false);
			DisablePlayCampaign();
		}

		public void ExtrasMenu(){
			playMenu.SetActive(false);
			if(extrasMenu) extrasMenu.SetActive(true);
			exitMenu.SetActive(false);
		}

		public void QuitGame(){
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}

		// Load Bar synching animation
		IEnumerator LoadAsynchronously(int sceneIndex){ // scene name is just the name of the current scene being loaded
			AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
			operation.allowSceneActivation = false;
			mainCanvas.SetActive(false);
			loadingMenu.SetActive(true);

			while (!operation.isDone){
				float progress = Mathf.Clamp01(operation.progress / .95f);
				loadingBar.value = progress;

				if (operation.progress >= 0.9f && waitForInput){
					loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
					loadingBar.value = 1;

					if (Input.GetKeyDown(userPromptKey)){
						operation.allowSceneActivation = true;
					}
                }else if(operation.progress >= 0.9f && !waitForInput){
					operation.allowSceneActivation = true;
				}

				yield return null;
			}
		}
	}
}