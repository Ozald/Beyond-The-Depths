using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleOutline : MonoBehaviour
{
    public Color outlineColor = Color.black;
    public float outlineSize = 1.1f;

    private GameObject outlineObject;

    void OnEnable()
    {
        CreateOutline();
    }

    void OnDisable()
    {
        if (outlineObject != null)
            Destroy(outlineObject);
    }

    void CreateOutline()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * outlineSize;

        SpriteRenderer outlineSR = outlineObject.AddComponent<SpriteRenderer>();
        outlineSR.sprite = sr.sprite;
        outlineSR.color = outlineColor;
        int layerID = SortingLayer.NameToID("Foreground");
        outlineSR.sortingLayerID = layerID;
        outlineSR.sortingOrder = sr.sortingOrder - 1;
    }
}