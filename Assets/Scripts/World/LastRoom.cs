namespace World
{
    public class LastRoom : Room
    {
        private void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += OnEndCutscene;
        }
        
        private void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= OnEndCutscene;
        }

        private void OnEndCutscene()
        {
            print("SWITCHIN TIME");
            SwitchCamera();
        }
    }
}