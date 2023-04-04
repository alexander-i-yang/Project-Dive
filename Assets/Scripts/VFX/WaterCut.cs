using System;
using Core;
using UnityEngine;

namespace VFX
{
    public class WaterCut : MonoBehaviour
    {
        private float _endY;
        private SpriteRenderer _spriteRenderer;
        private float _lastLen;
        private float _fallSpeed;

        public void Init(float endY, float startLen, float fallSpeed)
        {
            
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // _spriteRenderer.transform.localScale = new Vector3(1, startLen, 1);
            _spriteRenderer.transform.localPosition = new Vector3(0, endY - startLen / 2);
            _endY = endY;
            _fallSpeed = fallSpeed;
            _lastLen = startLen;
            ResizeLine(_lastLen);
            // ResizeLine(startLen);
        }

        private void Update()
        {
            float curLen = _lastLen - _fallSpeed * Game.Instance.DeltaTime;
            // _spriteRenderer.transform.localPosition = new Vector3(0, curY, 0);
            _lastLen = curLen;
            if (curLen < 0)
            {
                Destroy(gameObject);
            }
            ResizeLine(curLen);
        }

        private void ResizeLine(float currentLength)
        {
            var transform1 = _spriteRenderer.transform;
            transform1.localScale = new Vector3(1, currentLength, 1);
            currentLength += 6;
            Vector3 pos = transform1.localPosition;
            transform1.localPosition = new Vector3(pos.x, _endY + currentLength/2 + 1, pos.z);
        }
    }
}