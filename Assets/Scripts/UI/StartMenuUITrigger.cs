using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Player;
using UnityEngine;

public class StartMenuUITrigger : MonoBehaviour {
    
    [SerializeField] private GameObject startMenu;
    [SerializeField] private float cameraToPlayerTransitionThreshhold;
    [SerializeField] private float transitionDelay;
    [SerializeField] private float transitionDuration;
    [SerializeField] private Transform finalLocationWorld;


    private void OnTriggerEnter2D(Collider2D col) {
        // make start menu appear with a delay (to account for camera pan down)
        BeginStartMenuTransition();
    }

    private void BeginStartMenuTransition() {
        StartCoroutine(StartMenuTransition());
    }

    private IEnumerator StartMenuTransition()
    {

        yield return new WaitUntil(() =>
        {
            bool cameraInPlaceCheck = Camera.main.transform.position.y <=
                                      PlayerCore.Actor.transform.position.y + cameraToPlayerTransitionThreshhold;
            return cameraInPlaceCheck;
        });

        yield return new WaitForSeconds(transitionDelay);

        startMenu.SetActive(true);
        
        RectTransform startMenuTransform = startMenu.GetComponent<RectTransform>();
        CanvasGroup canvasG = startMenu.GetComponent<CanvasGroup>();

        Vector3 finalScreenPos = Camera.main.WorldToScreenPoint(finalLocationWorld.transform.position);
        //startMenuTransform.SetPositionY(finalScreenPos.y);
        canvasG.alpha = 0;
        while (canvasG.alpha < 1) {
            canvasG.alpha += Time.deltaTime * (1.0f / transitionDuration);
            //startMenuTransform.SetPositionY(Mathf.SmoothStep(0, finalScreenPos.y, canvasG.alpha));
            yield return null;
        }
    }
}
