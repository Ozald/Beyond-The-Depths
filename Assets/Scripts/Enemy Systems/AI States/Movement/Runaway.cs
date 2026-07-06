using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Runaway : AIState
{
    public float moveSpeed;
    public string runningAnimationTrigger;

    public override void OnEnter(Enemy enemy)
    {
        AIPath enemyAI = enemy.GetComponent<AIPath>();
        Animator enemyAnim = enemy.GetComponent<Animator>();

        if (enemyAI != null)
            enemyAI.isStopped = false;

        if (enemyAnim != null)
            enemyAnim.SetTrigger(runningAnimationTrigger);
    }

    public override void OnExit(Enemy enemy) { }

    public override void OnUpdate(Enemy enemy) { }

    public override void OnFixedUpdate(Enemy enemy)
    {
        Seeker enemyAI = enemy.GetComponent<Seeker>();
        Rigidbody2D enemyRB = enemy.GetComponent<Rigidbody2D>();

        if (enemyAI != null && enemyRB != null)
        {
            // This sends a request to update the path in batches rather than every frame, which is more efficient. The callback is used to calculate the path once the request is processed.
            PathAIManager.RequestPathUpdate(enemy, () => CalculatePath(enemy));

            if (enemy.currentPath == null)
                return;

            if (enemy.currentWaypoint >= enemy.currentPath.vectorPath.Count)
            {
                enemy.reachedEndOfPath = true;
                return;
            }
            else
            {
                enemy.reachedEndOfPath = false;
            }

            Move(enemy, enemyRB);
        }
    }

    /***********************************************************************************/

    void Move(Enemy enemy, Rigidbody2D enemyRB)
    {
        int currWaypoint = enemy.currentWaypoint;
        int totalWaypoints = enemy.currentPath.vectorPath.Count;

        Vector2 targetPos = (Vector2)enemy.currentPath.vectorPath[currWaypoint];
        float distanceToTarget = Vector2.Distance(enemyRB.position, targetPos);
        bool isFinalWaypoint = currWaypoint >= totalWaypoints - 1;

        // Skip to the next waypoint if the enemy is close enough to the current one
        if (!isFinalWaypoint && distanceToTarget < 2f)
        {
            enemy.currentWaypoint++;
            targetPos = (Vector2)enemy.currentPath.vectorPath[enemy.currentWaypoint];
            distanceToTarget = Vector2.Distance(enemyRB.position, targetPos);
            isFinalWaypoint = enemy.currentWaypoint >= totalWaypoints - 1;
        }

        // If the enemy is at the final waypoint and close enough, slow it down and stop moving
        float currentSpeed = moveSpeed;
        if (isFinalWaypoint)
        {
            if (distanceToTarget < 0.5f)
            {
                enemy.reachedEndOfPath = true;
                enemyRB.velocity = Vector2.zero;
                return;
            }

            if (distanceToTarget < 2f)
            {
                currentSpeed *= (distanceToTarget / 2f);
            }
        }

        Vector2 moveDir = (targetPos - enemyRB.position).normalized;
        enemyRB.AddForce(moveDir * currentSpeed);

        // Rotate the enemy to face the direction of movement
        Vector2 currVelocity = enemyRB.velocity;
        if (currVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(currVelocity.y, currVelocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);

            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 360f * Time.fixedDeltaTime);
        }
    }

    void CalculatePath(Enemy enemy)
    {
        Seeker enemyAI = enemy.GetComponent<Seeker>();

        // This is not the best way to do this, but to avoid merge conflicts I am going to keep it like this. Sue me.
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
            return;

        // Calculate location to run to
        float maxEscapeDistance = 1f;
        Vector3 dir = (enemy.transform.position - player.position).normalized;
        Debug.DrawRay(enemy.transform.position, dir * maxEscapeDistance, Color.green, 1.0f);

        // Check for a wall (ignoring triggers)
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Obstacles"));
        contactFilter.useTriggers = false;

        RaycastHit2D[] results = new RaycastHit2D[1];
        int hitCount = Physics2D.Raycast(enemy.transform.position, dir, contactFilter, results, maxEscapeDistance);

        if (hitCount > 0)
        {
            // When the enemy hits the wall, it checks which direction is safer to run to

            Vector2 leftChoice = new Vector2(-dir.y, dir.x);
            Vector2 rightChoice = new Vector2(dir.y, -dir.x);

            // Cast a ray to the left
            int leftHit = Physics2D.Raycast(enemy.transform.position, leftChoice, contactFilter, results, 3f);
            float leftDistance = (leftHit > 0) ? results[0].distance : float.MaxValue;

            // Cast a ray to the right
            int rightHit = Physics2D.Raycast(enemy.transform.position, rightChoice, contactFilter, results, 3f);
            float rightDistance = (rightHit > 0) ? results[0].distance : float.MaxValue;

            if (leftDistance > rightDistance)
            {
                dir = leftChoice;
            }
            else
            {
                dir = rightChoice;
            }
        }

        Vector3 targetPosition = enemy.transform.position + dir * 3f;

        if (AstarPath.active != null)
        {
            targetPosition = AstarPath.active.GetNearest(targetPosition).position;
        }

        // Calculate the final path
        enemyAI.StartPath(enemy.transform.position, targetPosition, (Path p) => OnPathGenerate(p, enemy));
    }

    void OnPathGenerate(Path p, Enemy enemy)
    {
        // If there is no error with the path, set the enemy's current path to the new path and reset the waypoint index
        if (!p.error)
        {
            enemy.currentPath = p;
            int newWaypoint = 0;

            for (int i = 0; i < p.vectorPath.Count; i++)
            {
                // This is to prevent the enemy from trying to move to a waypoint that is too close to it, which can cause jittery movement
                float distance = Vector2.Distance(enemy.transform.position, p.vectorPath[i]);
                if (distance >= 1f)
                {
                    newWaypoint = i;
                    break;
                }
            }

            enemy.currentWaypoint = newWaypoint;
        }
    }
}

/*******************************************************************/

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Runaway))]
public class RunawayEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;

        Rect r = position;
        r.height = lineHeight;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 12;
        EditorGUI.LabelField(r, "Parameters", titleStyle);
        
        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("moveSpeed"));

        r.y += lineHeight + 2;
        EditorGUI.PropertyField(r, property.FindPropertyRelative("runningAnimationTrigger"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 10;
    }
}
#endif
