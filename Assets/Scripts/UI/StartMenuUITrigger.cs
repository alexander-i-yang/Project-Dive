using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Player;
using UnityEngine;

public class StartMenuUITrigger : MonoBehaviour {
    
    [SerializeField] private GameObject startMenu;

    private void OnTriggerEnter2D(Collider2D col) {
        // make start menu appear with a 1 second delay
        Invoke(nameof(BeginStartMenuTransition), 1.0f);
    }

    private void BeginStartMenuTransition() {
        StartCoroutine(StartMenuTransition());
    }

    private IEnumerator StartMenuTransition() {
        const float TRANSITION_DURATION = 1.0f;
        const float FINAL_START_MENU_OFFSET = 30.0f;

        startMenu.SetActive(true);
        
        RectTransform startMenuTransform = startMenu.GetComponent<RectTransform>();
        CanvasGroup canvasG = startMenu.GetComponent<CanvasGroup>();
        
        canvasG.alpha = 0;
        while (canvasG.alpha < 1) {
            canvasG.alpha += Time.deltaTime * (1.0f / TRANSITION_DURATION);
            startMenuTransform.SetPositionY(Mathf.SmoothStep(0, FINAL_START_MENU_OFFSET, canvasG.alpha));
            yield return null;
        }
    }
}
