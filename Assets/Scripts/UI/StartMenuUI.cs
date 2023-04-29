using ASK.Core;
using Audio;
using UnityEngine;
using World;

namespace UI
{
    public class StartMenuUI : MonoBehaviour {
        [SerializeField] private MyScenes sceneName;
        [SerializeField] private FmodEventStopper audioStopper;
        [SerializeField] private LoadSceneManager sceneManager;
    
        public void OnStartButtonPress() {
            audioStopper.Stop();
            sceneManager.LoadSceneAsync(sceneName);
            // sceneManager.SwitchScene(MyScenes.Area1Scene);
        }

        public void OnQuitButtonPress() {
            Game.Quit();
        }
    }

}