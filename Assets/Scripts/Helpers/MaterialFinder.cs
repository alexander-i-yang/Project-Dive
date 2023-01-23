﻿using System.Collections.Generic;
using UnityEngine;

namespace Helpers
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
    }
}
