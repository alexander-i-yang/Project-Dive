using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;

public class StartMenuUI : MonoBehaviour {
    [SerializeField] private string gameSceneName;
    [SerializeField] private FmodEventStopper audioStopper;
    
    public void OnStartButtonPress() {
        audioStopper.Stop();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitButtonPress() {
        Game.Quit();
    }
}
