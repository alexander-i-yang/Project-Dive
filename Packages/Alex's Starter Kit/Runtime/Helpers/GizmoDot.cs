using UnityEngine;

namespace ASK.Helpers.Editor
{
    public class GizmoDot : MonoBehaviour
    {
    
        [SerializeField] private bool onSelected = true;
        [SerializeField] private float radius = 1;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private Vector3 offset = Vector3.zero;
        
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!onSelected) Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if (onSelected) Draw();
        }

        void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + offset, radius);
        }
        #endif
    }
}