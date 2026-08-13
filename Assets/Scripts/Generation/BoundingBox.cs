using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public delegate void Entered();
    public Entered OnEntered = delegate { };
    
    [HideInInspector]
    public PolygonCollider2D collider;

    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        OnEntered?.Invoke();
    }
}
