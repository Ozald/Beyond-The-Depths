using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyPathAI : MonoBehaviour
{
    public Transform target;
    public float speed = 400f;
    public float nextWaypointDistance = 3f;

    public Transform enemyGFX;
    
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    
    Seeker seeker;
    Rigidbody2D rb;
    
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.25f); //updates every [second]
        

        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        } else
        {
            reachedEndOfPath = false;
        }
        
        //gets an array from current pos to the one you want to be at, then normalizes it so that its length always = 1
        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized; 
        
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force); //moves obj by force
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        
        //For SPRITE FLIPPING
        //TODO: Sprites flip too often when in tight corners,
        // we can technically just design the corners to be wider tho LMAO
        if (force.x >= 0.05f) //if the obj wants to move to the right
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        } else if (force.x <= -0.05f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }

}
