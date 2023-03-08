using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Player;
using UnityEngine;

public class StartMenuUITrigger : MonoBehaviour {
    
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private float transitionStartDelay;
    [SerializeField] private float transitionDuration;
    [SerializeField] private Transform finalLocationWorld;


    private void OnTriggerEnter2D(Collider2D col) {
        // disables player input for start menu
        player.GetComponent<PlayerInputController>().enabled = false;

        // make start menu appear with a delay (to account for camera pan down)
        Invoke(nameof(BeginStartMenuTransition), transitionStartDelay);
    }

    private void BeginStartMenuTransition() {
        StartCoroutine(StartMenuTransition());
    }

    private IEnumerator StartMenuTransition() {

        startMenu.SetActive(true);
        
        RectTransform startMenuTransform = startMenu.GetComponent<RectTransform>();
        CanvasGroup canvasG = startMenu.GetComponent<CanvasGroup>();

        Vector3 finalScreenPos = Camera.main.WorldToScreenPoint(finalLocationWorld.transform.position);
        
        canvasG.alpha = 0;
        while (canvasG.alpha < 1) {
            canvasG.alpha += Time.deltaTime * (1.0f / transitionDuration);
            startMenuTransform.SetPositionY(Mathf.SmoothStep(0, finalScreenPos.y, canvasG.alpha));
            yield return null;
        }
    }
}
