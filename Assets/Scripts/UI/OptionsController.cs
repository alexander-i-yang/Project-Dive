using UnityEngine;

namespace UI
{
    public class OptionsController : MonoBehaviour
    {
        public delegate void OnGraphicsQualityToggle(bool active);
        public static event OnGraphicsQualityToggle GraphicsQualityToggleEvent;

        public void RecieveGraphicsQualityToggle(bool state)
        {
            GraphicsQualityToggleEvent?.Invoke(state);
        }
    }
}