using UnityEngine;

using Player;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] private GameObject uiFrame;

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

    }

    public void OnRestart()
    {
        
    }

    public void OnBackToStart()
    {

    }

    public void OnQuit()
    {
        
    }
}
