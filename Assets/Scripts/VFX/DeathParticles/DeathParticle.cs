using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DeathParticle : MonoBehaviour
    {
        private Vector3 _startPosLocal;
        private Rigidbody2D _myRB;
        
        void Awake()
        {
            _startPosLocal = transform.localPosition;
            _myRB = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 v)
        {
            _myRB.AddForce(v, ForceMode2D.Impulse);
        }

        public void Reset()
        {
            transform.localPosition = _startPosLocal;
            transform.rotation = Quaternion.identity;
            // _myRB.for
        }
    }
}