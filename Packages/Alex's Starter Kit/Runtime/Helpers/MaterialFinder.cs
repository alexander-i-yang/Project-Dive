using System.Collections.Generic;
using UnityEngine;

namespace ASK.Helpers
{
    public class MaterialFinder
    {
        public static List<Material> FindMaterialsWithShader(string shaderName)
        {
            List<Material> materials = new List<Material>();
            Shader shader = Shader.Find(shaderName);

            Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>(true);
            foreach (Renderer r in allRenderers)
            {
                if (r.material.shader == shader)
                {
                    materials.Add(r.material);
                }
            }

            return materials;
        }

        public static void ReplaceMaterial(Material from, Material to)
        {
            Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>(true);
            foreach (Renderer r in allRenderers)
            {
                if (r.sharedMaterial.shader == from.shader)
                {
                    r.sharedMaterial = to;
                }
            }
        }
    }
}
