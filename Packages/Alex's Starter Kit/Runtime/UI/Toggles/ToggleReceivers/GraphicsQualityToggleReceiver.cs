namespace ASK.UI
{
    public class GraphicsQualityToggleReceiver : ToggleReceiver
    {
        public delegate void OnToggle(bool active);
        public static event OnToggle ToggleEvent;

        public void ReceiveToggle(bool state)
        {
            ToggleEvent?.Invoke(state);
        }
    }
}