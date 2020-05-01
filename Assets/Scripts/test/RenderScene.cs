using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RenderScene : MonoBehaviour
{
    public Material mat;
    public Shader shader;

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(mat == null) {
            mat = new Material(shader);
        }


        // Copy the source Render Texture to the destination,
        // applying the material along the way.
        // Debug.Log("tests");
        Graphics.Blit(src, dest, mat);
    }
}
