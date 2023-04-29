using ASK.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ASK.UI
{
    public class PauseUIController : MonoBehaviour
{

    [SerializeField] protected UIPage mainUI;
    [SerializeField] protected UIPage optionsUI;

    [SerializeField] protected Image bg;

    private bool _paused;
    public bool Paused {
        get
        {
            return _paused;
        }
    }

    protected virtual void SetPaused(bool value)
    {
        _paused = value;
        mainUI.Show(value);
        bg.enabled = value;
        if (!value)
        {
            optionsUI.Show(false);
        }
        Game.Instance.IsPaused = value;
    }

    protected virtual void Start()
    {
        SetPaused(false);
    }

    protected virtual void OnEnable()
    {
        Game.Instance.OnDebugEvent += OnPause;
    }

    protected virtual void OnDisable()
    {
        Game.Instance.OnDebugEvent -= OnPause;
    }

    protected virtual void OnPausePressed()
    {
        SetPaused(!Paused);
    }

    public void OnPause()
    {
        SetPaused(true);
    }
    
    public void OnResume()
    {
        SetPaused(false);
    }

    public void OnQuit()
    {
        Game.Quit();
    }
    
    public void OnOptions()
    {
        mainUI.Show(false);
        optionsUI.Show(true);
    }
    
    public void OnBackFromOptions()
    {
        mainUI.Show(true);
        optionsUI.Show(false);
    } 
}
}