using System;
using ASK.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEngine.UI.Image ArrowLeft;
    public UnityEngine.UI.Image ArrowRight;

    private Coroutine _arrowLeftCoroutine;
    private Coroutine _arrowRightCoroutine;

    public float fadeTime = 0.5f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowArrows(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        ShowArrows(false);
    }

    private void ShowArrows(bool active)
    {
        Color newColor = active ? Color.white : Color.clear;
        if (_arrowLeftCoroutine != null)
        {
            StopCoroutine(_arrowLeftCoroutine);
        }
        if (_arrowRightCoroutine != null)
        {
            StopCoroutine(_arrowRightCoroutine);
        }

        _arrowLeftCoroutine = StartCoroutine(Helper.Fade(ArrowLeft, fadeTime, newColor));
        _arrowRightCoroutine = StartCoroutine(Helper.Fade(ArrowRight, fadeTime, newColor));
    }
}