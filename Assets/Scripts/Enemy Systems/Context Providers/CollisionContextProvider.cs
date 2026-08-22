using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionContextProvider : MonoBehaviour
{
    public LayerMask collisionLayerMask;

    [SerializeField] private string _isCollidingAttributeName = "IsColliding";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
            return;

        Debug.Log("Collision detected with: " + collision.gameObject.name);

        EnemyContext _enemyContext = GetComponent<Enemy>().GetData();
        _enemyContext.SetAttribute(_isCollidingAttributeName, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
            return;

        EnemyContext _enemyContext = GetComponent<Enemy>().GetData();
        _enemyContext.SetAttribute(_isCollidingAttributeName, false);
    }
}
