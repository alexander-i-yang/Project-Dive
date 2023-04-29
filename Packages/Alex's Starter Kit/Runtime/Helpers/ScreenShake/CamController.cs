using System;
using UnityEngine;

namespace ASK.ScreenShake
{
    public class CameraController : MonoBehaviour
    {
        private Camera _mainCamera;
        
        [NonSerialized]
        public ScreenShakeManager ScreenShakeMan;
        
        public Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    return FindObjectOfType<Camera>();
                }
                return _mainCamera;
            }

            private set { }
        }
        
        void Awake()
        {
            Application.targetFrameRate = 60;
            _mainCamera = FindObjectOfType<Camera>();
            ScreenShakeMan = FindObjectOfType<ScreenShakeManager>();
        }
    }
}