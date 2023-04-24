using System;
using Core;
using Helpers;
using MyBox;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using World;
using Random = UnityEngine.Random;

namespace VFX
{
    public class BatSpawner : MonoBehaviour
    {
        private Camera _mainCamera;
        private GameTimer _batTimer;
        
        [SerializeField] private float spawnPosBuffer = 32f;
        [SerializeField, MinMaxRange(0, 30)]
        private RangedInt batSpawnDelay = new (5, 10);
        [SerializeField] private GameObject batPrefab;

        void Awake()
        {
            _mainCamera = FindObjectOfType<Camera>();
            _batTimer = GameTimer.StartNewTimer(batSpawnDelay.GetRandom());
            _batTimer.OnFinished += SpawnBat;
        }

        private void OnEnable()
        {
            EndCutsceneManager.EndCutsceneEvent += StopSpawning;
        }

        private void OnDisable()
        {
            EndCutsceneManager.EndCutsceneEvent -= StopSpawning;
        }

        void Update()
        {
            GameTimer.Update(_batTimer);
        }

        void StopSpawning()
        {
            gameObject.SetActive(false);
        }

        private void SpawnBat()
        {
            Vector2 cameraPos = _mainCamera.transform.position;
            Vector2 cameraSize = Game.Instance.ScreenSize;

            float xCoord = cameraPos.x - cameraSize.x / 2 - spawnPosBuffer;
            float yMin = cameraPos.y - cameraSize.y/2;
            float yMax = cameraPos.y + cameraSize.y/2;

            Vector2 batPos = new Vector2(xCoord, Random.Range(yMin, yMax));
            Instantiate(batPrefab, batPos, Quaternion.identity, transform);
            
            GameTimer.Reset(_batTimer, batSpawnDelay.GetRandom());
        }
    }
}