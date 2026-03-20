using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




public class EnemyBaseController : MonoBehaviour
{

    [SerializeField]
    public AIState fromState;
    public AIState toState;
    public Condition condition;
    
    
    
    
    void Start()
    { 
        
        
        
        
        
        /*
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D coll = GetComponent<BoxCollider2D>(); //for the base enemy's sprite
        coll.isTrigger = true;
        coll.size =  spriteRenderer.bounds.size;
        */
        
    }

    
    void Update()
    {
        
        
        
    }
}
