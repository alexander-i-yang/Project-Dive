using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ASK.UI
{
    [RequireComponent(typeof(Text))]
    public class TextToggle : MonoBehaviour
    {
        [SerializeField] private string flavor;
        [SerializeField] private string saveDataName;
        [SerializeField] private string toggleOnName = "On";
        [SerializeField] private string toggleOffName = "Off";
        
        [SerializeField] private bool starting;

        [SerializeField] private UnityEvent ToggleOn;
        [SerializeField] private UnityEvent ToggleOff;

        private bool _state;
        private Text _text;

        public void Start()
        {       
            bool savedState = PlayerPrefs.GetInt(saveDataName, starting ? 1 : 0) == 1;
            _text = GetComponent<Text>();
            _state = savedState;
            UpdateText();
            if (_state) ToggleOn.Invoke();
            else ToggleOff.Invoke();
        }

        public void Toggle()
        {
            _state = !_state;
            UpdateText();
            
            PlayerPrefs.SetInt(saveDataName, _state ? 1 : 0);
            
            if (_state) ToggleOn.Invoke();
            else ToggleOff.Invoke();
        }

        private void UpdateText()
        {
            _text.text = $"{flavor}: {(_state ? toggleOnName : toggleOffName)}";
        }
    }
}