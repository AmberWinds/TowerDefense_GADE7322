using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehaviour : MonoBehaviour 
{
    /* Will be placed on the Enemy themselves.
     *  - Manage Navigation.
     *  - Will Manage Destruction / pooling depending.
     */

    private Animator animator;
    private NavMeshAgent agent;
    private LinkedList<List<Vector3>> paths;
    private List<Vector3> currentPath;
    private int currentWaypointIndex;
    [SerializeField] private float waypointReachThreshold = 4f;

    private float currentHealth;
    private float maxHealth;
    private float attackDmg;
    private float attackRange = 5f;
    private float scanRadius = 12f;
    private float attackRate = 5f;
    private float scanRate = 1f;
    
    private GameObject target;
    private DefenderBehaviour targetDef; 

    private float nextScanTime;
    private float nextAttackTime;

    private enum State { Idle, Pathing, Chasing, Attacking }
    private State state = State.Idle;

    [SerializeField] FloatingHealthBar healthBar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null) animator.SetBool("isIdle", true);

        agent = GetComponent<NavMeshAgent>();
        currentPath = new List<Vector3>();

        currentWaypointIndex = 0;

    }

    public void BeginTracking(Enemy me)
    {
        //Need to find the closest path
        maxHealth = me.health;
        currentHealth = me.health;
        attackDmg = me.attackDmg;
        state = State.Idle;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        paths = GameManager.Instance.paths;

        FindAndAssignClosestPath();

        if (currentPath != null && currentPath.Count > 0 && agent != null)
        {
            MoveToCurrentWaypoint();
            state = State.Pathing;
        }
        else if (agent != null && GameManager.Instance != null && GameManager.Instance.mainTower != null)
        {
            // Fallback: if no valid path, move directly to the main tower so enemies don't stall
            agent.SetDestination(GameManager.Instance.mainTower.transform.position);
            state = State.Pathing;
            if (animator != null)
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
            }
        }
    }

    private void FindAndAssignClosestPath()
    {
        float bestDistance = float.PositiveInfinity;
        List<Vector3> bestPath = null;

        foreach (var path in paths)
        {
            float dist = Vector3.Distance(transform.position, path[0]);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestPath = path;
            }
        }

        if (bestPath != null && bestPath.Count > 0)
        {
            currentPath = bestPath;
            //currentWaypointIndex = 0;
        }

    }


    // Update is called once per frame
    void Update()
    {

        if (Time.time >= nextScanTime)
        {
            nextScanTime = Time.time + scanRate;
            RefreshOrAcquireTarget();
        }

        if (target != null)
        {
            float Dist = Vector3.Distance(transform.position, target.transform.position);
            if (Dist > attackRange)     //Oustide attack range
            {
                //Chase target Down
                state = State.Chasing;
                agent.stoppingDistance = Mathf.Max(attackRange * 0.9f, 0.1f);
                agent.SetDestination(target.transform.position);
            }
            else
            {
                //At TargetWalls
                state= State.Attacking;
                agent.isStopped = true; //Stop Agent Moving.
                animator.SetBool("isAttacking", true);
                animator.SetBool("isWalking", false);

                Attack(target);
                return;
            }

        }
        else
        {
            state = State.Pathing;

            agent.isStopped = false;          
            agent.stoppingDistance = 0f;

            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);;
        }

        //Pathing Logic
        if (currentPath != null && state == State.Pathing)
        {
            // If close enough to current waypoint, advance to the next
            float sqrDist = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(currentPath[currentWaypointIndex].x, 0, currentPath[currentWaypointIndex].z)).sqrMagnitude;
            float reach = Mathf.Max(waypointReachThreshold, agent.stoppingDistance + 0.1f);

            if (sqrDist <= reach * reach)
            {
                AdvanceWaypoint();
            }
            else
            {
                MoveToCurrentWaypoint();
            }


        }
        else if(currentPath == null && state == State.Pathing)
        {
            FindAndAssignClosestPath();
            MoveToCurrentWaypoint();
            Debug.Log("repathing goblin");
        }



    }

    private void RefreshOrAcquireTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, scanRadius);
        
        foreach(Collider collider in hitColliders)
        {
            if(collider.gameObject.GetComponent<DefenderBehaviour>() != null)
            {
                target = collider.gameObject;
                break;
            }
            else
            {
                target = null;
            }
        }
    }

    public void Attack(GameObject target)
    {
        //face the target
        Vector3 dir = (target.transform.position - transform.position);
        currentPath = null;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + (1f / Mathf.Max(attackRate, 0.01f));
        targetDef = target.GetComponent<DefenderBehaviour>();
        targetDef.BeAttacked(attackDmg);

    }


    private void MoveToCurrentWaypoint()
    {
        if (currentWaypointIndex >= currentPath.Count - 1)
        {
            currentWaypointIndex = currentPath.Count -1;
        }
        Vector3 target = currentPath[currentWaypointIndex];

        agent.SetDestination(target);
        if (animator != null)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", true);
        }
    }

    private void AdvanceWaypoint()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (currentWaypointIndex < currentPath.Count - 1)
        {
            currentWaypointIndex += 4;                                  //trying to make it more smooth by moving two indexes
            MoveToCurrentWaypoint();
        }
        else
        {
            // Reached end of path: stop or handle attacking tower, etc.
            if (agent != null)
            {
                agent.ResetPath();
            }
            if (animator != null)
            {
                animator.SetBool("isAttacking", true);
                animator.SetBool("isWalking", false);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        int dmg = other.gameObject.GetComponent<Bullet>().attackDmg;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // Apply damage
        currentHealth -= dmg;
        Debug.Log($"Been shot at! Took {dmg} damage, currentHealth now {currentHealth}");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Goblin Died");
        }
    }
}

