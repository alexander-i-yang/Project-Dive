using System;
using UnityEngine;

using Player;

using Core;
using Audio;
using UI;
using UnityEngine.UI;
using World;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] private MyScenes curScene;
    [SerializeField] private MyScenes startScene;

    [SerializeField] private UIPage mainUI;
    [SerializeField] private UIPage creditsUI;
    [SerializeField] private UIPage optionsUI;

    [SerializeField] private Image bg;

    private bool _paused;
    public bool Paused {
        get
        {
            return _paused;
        }

        set
        {
            _paused = value;
            mainUI.Show(value);
            bg.enabled = value;
            if (!value)
            {
                creditsUI.Show(false);
                optionsUI.Show(false);
            }
            Game.Instance.IsPaused = value;
        }
    }

    private void Start()
    {
        Paused = false;
        PlayerCore.Input.AddToPauseAction(OnPausePressed);
    }

    private void OnEnable()
    {
        Game.Instance.OnDebugEvent += OnPause;
    }

    private void OnDisable()
    {
        PlayerCore.Input.RemoveFromPauseAction(OnPausePressed);
        Game.Instance.OnDebugEvent -= OnPause;
    }

    private void OnPausePressed()
    {
        Paused = !Paused;
    }

    public void OnPause()
    {
        Paused = true;
    }
    
    public void OnResume()
    {
        Paused = false;
    }

    public void OnRestartArea()
    {
        AudioManager.StopAllMusic();
        FindObjectOfType<LoadSceneManager>().LoadSceneAsync(curScene);
    }

    public void OnBackToStart()
    {
        AudioManager.StopAllMusic();
        FindObjectOfType<LoadSceneManager>().LoadSceneAsync(startScene);
    }

    public void OnCredits()
    {
        mainUI.Show(false);
        creditsUI.Show(true);
    }
    
    public void OnOptions()
    {
        mainUI.Show(false);
        optionsUI.Show(true);
    }

    public void OnBackFromCredits()
    {
        mainUI.Show(true);
        creditsUI.Show(false);
    }

    public void OnBackFromOptions()
    {
        mainUI.Show(true);
        optionsUI.Show(false);
    }

    public void OnQuit()
    {
        Game.Quit();
    }
}
