using System.Collections;
using Helpers;
using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DeathParticle : MonoBehaviour
    {
        private Vector3 _startPosLocal;
        private Rigidbody2D _myRB;
        private SpriteRenderer _mySR;
        
        public void Init()
        {
            _startPosLocal = transform.localPosition;
            _myRB = GetComponent<Rigidbody2D>();
            _mySR = GetComponent<SpriteRenderer>();
        }

        public void Launch(Vector2 v, float rotationV, float persistTime, float fadeTime)
        {
            _myRB.AddForce(v, ForceMode2D.Impulse);
            _myRB.AddTorque(rotationV, ForceMode2D.Impulse);
            StartCoroutine(FadeCoroutine(persistTime, fadeTime));
        }

        private IEnumerator FadeCoroutine(float persistTime, float fadeTime)
        {
            yield return Helper.Sleep(persistTime);
            Color origColor = _mySR.color;
            Color newColor = _mySR.color;
            newColor.a = 0;
            yield return Helper.FadeColor(
                fadeTime, 
                origColor, 
                newColor,
                c =>
                {
                    _mySR.color = c;
                }
            );
            Destroy(gameObject);
        }
    }
}