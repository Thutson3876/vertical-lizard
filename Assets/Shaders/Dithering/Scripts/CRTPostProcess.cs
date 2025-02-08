using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CRTPostProcess : MonoBehaviour
{
    public Texture image;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(image, destination);
    }
}
