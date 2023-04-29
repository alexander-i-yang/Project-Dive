using Audio;
using Player;
using UnityEngine;
using World;

namespace UI
{
    public class DivePauseUIController : ASK.UI.PauseUIController
    {
        [SerializeField] private UIPage creditsUI;
        [SerializeField] private MyScenes curScene;
        [SerializeField] private MyScenes startScene;
        
        protected override void Start()
        {
            base.Start();
            PlayerCore.Input.AddToPauseAction(OnPausePressed);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerCore.Input.RemoveFromPauseAction(OnPausePressed);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerCore.Input.AddToPauseAction(OnPausePressed);
        }

        protected override void SetPaused(bool value)
        {
            base.SetPaused(value);
            if (!value) creditsUI.Show(false);
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

        public void OnBackFromCredits()
        {
            mainUI.Show(true);
            creditsUI.Show(false);
        }
    }
}