using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{
    [Tooltip("The default color given to the materials")]
    public Color DefaultColor = Color.white;

    [Tooltip("The color given to the materials when selected")]
    public Color SelectedColor = Color.green;

    [Tooltip("Should the materials get cloned at the start?")]
    public bool CloneMaterials = true;

    [System.NonSerialized]
    private Renderer cachedRenderer;

#if UNITY_EDITOR
    protected virtual void Reset()
    {
        Awake();
    }
#endif

    protected virtual void Awake()
    {
        if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();

        var material0 = cachedRenderer.sharedMaterial;

        if (material0 != null)
        {
            DefaultColor = material0.color;
        }

        if (CloneMaterials == true)
        {
            cachedRenderer.sharedMaterials = cachedRenderer.materials;
        }
    }

    public void ChangeColor(bool selected)
    {
        if (cachedRenderer == null) cachedRenderer = GetComponent<Renderer>();

        var materials = cachedRenderer.sharedMaterials;

        if (selected)
        {
            for (var i = materials.Length - 1; i >= 0; i--)
            {
                materials[i].color = SelectedColor;
            } 
        }
        else
        {
            for (var i = materials.Length - 1; i >= 0; i--)
            {
                materials[i].color = DefaultColor;
            }
        }

    }
}
