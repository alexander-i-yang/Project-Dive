using ASK.UI;
using World;

namespace UI
{
    public class DiveCanvasFadeIn : CanvasFadeIn
    {
        void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += FadeIn;
        }
        
        void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= FadeIn;
        }
    }
}