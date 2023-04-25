using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
    public class DustImpactPool : MonoBehaviour
    {
        [SerializeField] private GameObject groundDust;

        private List<DustImpactSpawner> _groundDusts = new();

        public void SpawnGroundDust(Vector3 pos)
        {
            bool found = false;
            foreach (var dust in _groundDusts)
            {
                if (dust.gameObject.activeSelf) continue;
                dust.ReInit(pos);
                found = true;
                return;
            }

            if (found == false)
            {
                var g = Instantiate(groundDust, pos, Quaternion.identity, transform).GetComponent<DustImpactSpawner>();
                g.Init();
                _groundDusts.Add(g);
            }
        }
    }
}