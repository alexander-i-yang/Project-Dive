using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using Helpers;
using Player;
using World;

namespace VFX
{
    public class LavaSimulation : MonoBehaviour, IFilterLoggerTarget
    {
        [SerializeField] private Material _lavaSimMat;
        [SerializeField] private Material _lavaMat;
        [SerializeField] private float impulseStrength = 0f;

        private CustomRenderTexture _lavaSimTex;

        [SerializeField] private RawImage _testImage;
        public CustomRenderTexture LavaSimTex => _lavaSimTex;

        private Room CurrRoom => PlayerCore.SpawnManager.CurrentRoom;

        private void Awake()
        {
            _lavaSimMat.SetVector("_Impulse", new Vector3(0, 0, 0));
        }

        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
        }

        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
        }

        private void Update()
        {
            if (_lavaSimTex != null)
            {
                if (_testImage != null)
                {
                    _testImage.texture = _lavaSimTex;
                }

                float velMag = PlayerCore.Actor.velocity.magnitude;
                if (CurrRoom != null && velMag > 1f)
                {
                    float impulseU = (PlayerCore.Actor.transform.position.x - CurrRoom.transform.position.x) / _lavaSimTex.width;
                    float impulseV = (PlayerCore.Actor.transform.position.y - CurrRoom.transform.position.y) / _lavaSimTex.height;
                    FilterLogger.Log(this, $"Impulse Strength: {impulseStrength * velMag / 256}");
                    _lavaSimMat.SetVector("_Impulse", new Vector3(impulseU, impulseV, impulseStrength * velMag / 256));
                }
                else
                {
                    _lavaSimMat.SetVector("_Impulse", new Vector3(0, 0, 0));
                }

            }
        }

        private void OnRoomTransition(Room roomEntering)
        {
            Bounds bounds = roomEntering.GetComponent<Collider2D>().bounds;

            _lavaSimTex = CreateLavaSimTexture((int)bounds.extents.x * 2, (int)bounds.extents.y * 2);

            _lavaMat.SetVector("_RoomPos", roomEntering.transform.position);
            _lavaMat.SetVector("_RoomSize", new Vector2(_lavaSimTex.width, _lavaSimTex.height));
            _lavaMat.SetTexture("_SimulationTex", _lavaSimTex);
        }

        private CustomRenderTexture CreateLavaSimTexture(int width, int height)
        {
            CustomRenderTexture tex = new CustomRenderTexture(width, height);
            tex.material = _lavaSimMat;

            tex.initializationMode = CustomRenderTextureUpdateMode.OnLoad;
            tex.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;
            tex.initializationColor = Color.black;

            tex.updateMode = CustomRenderTextureUpdateMode.Realtime;
            tex.doubleBuffered = true;

            return tex;
        }

        public LogLevel GetLogLevel()
        {
            return LogLevel.Warning;
        }
    }
}