using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public RectTransform rect;
    public float intensity = 20f;

    [HideInInspector] public Vector3 startPos;
}

public class UIParallaxManager : MonoBehaviour
{
    public List<ParallaxLayer> layers = new List<ParallaxLayer>();

    [Header("Motion Settings")]
    public float smoothSpeed = 5f;
    public bool invertX = false;
    public bool invertY = false;

    [Header("Dead Zone")]
    [Range(0f, 0.2f)] public float deadZone = 0.02f;

    void Start()
    {
        // Store starting positions
        foreach (var layer in layers)
        {
            if (layer.rect != null)
                layer.startPos = layer.rect.localPosition;
        }
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // Normalize mouse position to range (-1 to 1)
        float x = (mousePos.x / Screen.width - 0.5f) * 2f;
        float y = (mousePos.y / Screen.height - 0.5f) * 2f;

        // Dead zone to prevent jitter
        if (Mathf.Abs(x) < deadZone) x = 0;
        if (Mathf.Abs(y) < deadZone) y = 0;

        // Optional inversion
        if (invertX) x *= -1;
        if (invertY) y *= -1;

        Vector3 input = new Vector3(x, y, 0);

        foreach (var layer in layers)
        {
            if (layer.rect == null) continue;

            Vector3 target = layer.startPos + input * layer.intensity;

            layer.rect.localPosition = Vector3.Lerp(
                layer.rect.localPosition,
                target,
                Time.deltaTime * smoothSpeed
            );
        }
    }
}