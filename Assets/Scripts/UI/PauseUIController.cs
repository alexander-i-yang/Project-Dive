using UnityEngine;

using Player;

using Core;
using UnityEngine.SceneManagement;
using Audio;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] private GameObject uiFrame;

    [SerializeField] private string startSceneName;
    [SerializeField] private string gameSceneName;

    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject creditsUI;

    public bool Paused {
        get
        {
            return uiFrame.activeSelf;
        }

        set
        {
            uiFrame.SetActive(value);
        }
    }

    private void Start()
    {
        Paused = false;
        PlayerCore.Input.AddToPauseAction(OnPausePressed);
    }

    private void OnDisable()
    {
        PlayerCore.Input.RemoveFromPauseAction(OnPausePressed);
    }

    private void OnPausePressed()
    {
        Debug.Log("Pause Pressed.");
        Paused = !Paused;
    }

    public void OnResume()
    {
        Paused = false;
    }

    public void OnRestart()
    {
        AudioManager.StopAllMusic();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnBackToStart()
    {
        AudioManager.StopAllMusic();
        SceneManager.LoadScene(startSceneName);
    }

    public void OnCredits()
    {
        mainUI.SetActive(false);
        creditsUI.SetActive(true);
    }
    
    public void OnBackFromCredits()
    {
        mainUI.SetActive(true);
        creditsUI.SetActive(false);
    }

    public void OnQuit()
    {
        Game.Quit();
    }
}
