using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    public GameObject shadowPrefab;
    public Vector3 offset;
    public Vector3 scale;

    public GameObject currentShadow;

    // Start is called before the first frame update
    void Start()
    {
        currentShadow = Instantiate(shadowPrefab);
        
    }

    // Update is called once per frame
    void Update()
    {
        currentShadow.transform.position = transform.position + offset;
        currentShadow.transform.localScale = scale;
    }

    void OnDestroy()
    {
        Destroy(currentShadow);
    }
}
