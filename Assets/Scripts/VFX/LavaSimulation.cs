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
        private const string lavaShaderName = "Shader Graphs/Lava_World";

        [SerializeField] private Material lavaSimMat;
        [SerializeField, Range(0f, 1f)] private float impulseStrength = 0f;

        private CustomRenderTexture _lavaSimTex;
        private Material _lavaSimMatInstance;
        private List<Material> _lavaMatInstances;

        [SerializeField] private RawImage _testImage;
        public CustomRenderTexture LavaSimTex => _lavaSimTex;

        private Room CurrRoom => PlayerCore.SpawnManager.CurrentRoom;

        private void OnEnable()
        {
            Room.RoomTransitionEvent += OnRoomTransition;
        }

        private void OnDisable()
        {
            Room.RoomTransitionEvent -= OnRoomTransition;
        }

        private void Start()
        {
            _lavaMatInstances = MaterialFinder.FindMaterialsWithShader(lavaShaderName);
        }

        private void Update()
        {
            if (_lavaSimTex != null)
            {
                float velMag = PlayerCore.Actor.velocity.magnitude;
                if (CurrRoom != null && velMag > 1f)
                {
                    //Note: y is flipped because it is the upper left corner.
                    float impulseU = (PlayerCore.Actor.transform.position.x - CurrRoom.transform.position.x) / _lavaSimTex.width;
                    float impulseV = (CurrRoom.transform.position.y - PlayerCore.Actor.transform.position.y) / _lavaSimTex.height;
                    FilterLogger.Log(this, $"Impulse Strength: {impulseStrength * velMag / 256}");
                    _lavaSimMatInstance.SetVector("_Impulse", new Vector3(impulseU, impulseV, impulseStrength * velMag / 256));
                }
                else
                {
                    _lavaSimMatInstance.SetVector("_Impulse", new Vector3(0, 0, 0));
                }

            }
        }

        private void OnRoomTransition(Room roomEntering)
        {
            Bounds bounds = roomEntering.GetComponent<Collider2D>().bounds;

            _lavaSimTex = CreateLavaSimTexture((int)bounds.extents.x * 2, (int)bounds.extents.y * 2);
            if (_testImage != null)
            {
                _testImage.texture = _lavaSimTex;
            }

            _lavaMatInstances.ForEach(lavaMat =>
            {
                lavaMat.SetVector("_RoomPos", roomEntering.transform.position);
                lavaMat.SetVector("_RoomSize", new Vector2(_lavaSimTex.width, _lavaSimTex.height));
                lavaMat.SetTexture("_SimulationTex", _lavaSimTex);
            });

        }

        private CustomRenderTexture CreateLavaSimTexture(int width, int height)
        {
            CustomRenderTexture tex = new CustomRenderTexture(width, height);
            tex.material = new Material(lavaSimMat);
            _lavaSimMatInstance = tex.material;

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