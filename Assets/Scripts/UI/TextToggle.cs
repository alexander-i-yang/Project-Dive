using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Text))]
    public class TextToggle : MonoBehaviour
    {
        [SerializeField] private string flavor;
        [SerializeField] private string toggleOnName = "On";
        [SerializeField] private string toggleOffName = "Off";
        
        [Tooltip("Does not call events when applying the starting value.")]
        [SerializeField] private bool starting;

        [SerializeField] private UnityEvent ToggleOn;
        [SerializeField] private UnityEvent ToggleOff;

        private bool _state;
        private Text _text;

        void Awake()
        {
            _text = GetComponent<Text>();
            _state = starting;
            UpdateText();
        }

        public void Toggle()
        {
            _state = !_state;
            UpdateText();
            if (_state) ToggleOn.Invoke();
            else ToggleOff.Invoke();
        }

        private void UpdateText()
        {
            _text.text = $"{flavor}: {(_state ? toggleOnName : toggleOffName)}";
        }
    }
}