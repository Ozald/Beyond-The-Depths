using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionContextProvider : MonoBehaviour
{
    public LayerMask collisionLayerMask;

    private int activeCollisions = 0;

    [SerializeField] private string _isCollidingAttributeName = "IsColliding";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
            return;

        Debug.Log("Collision detected with: " + collision.gameObject.name);
        
        activeCollisions++;
        EnemyContext _enemyContext = GetComponent<Enemy>().GetData();
        _enemyContext.SetAttribute(_isCollidingAttributeName, activeCollisions > 0);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
            return;

        activeCollisions = Mathf.Max(0, activeCollisions - 1);
        EnemyContext _enemyContext = GetComponent<Enemy>().GetData();
        _enemyContext.SetAttribute(_isCollidingAttributeName, activeCollisions > 0);
    }
}
