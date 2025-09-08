using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private FloatingHealthBar healthBar;

    private float attackDmg;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null) animator.SetBool("isIdle", true);
        agent = GetComponent<NavMeshAgent>();
        currentPath = new List<Vector3>();

<<<<<<< Updated upstream
=======
        currentWaypointIndex = 0;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    public void BeginTracking(Enemy me)
    {
        //Need to find the closest path
        maxHealth = me.health;
        currentHealth = maxHealth;
        attackDmg = me.attackDmg;
<<<<<<< Updated upstream
=======
        state = State.Idle;
        
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

        paths = GameManager.Instance.paths;

        FindAndAssignClosestPath();

        if (currentPath != null && currentPath.Count > 0 && agent != null)
        {
            MoveToCurrentWaypoint();
        }
        else if (agent != null && GameManager.Instance != null && GameManager.Instance.mainTower != null)
        {
            // Fallback: if no valid path, move directly to the main tower so enemies don't stall
            agent.SetDestination(GameManager.Instance.mainTower.transform.position);
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
            currentWaypointIndex = 0;
        }

    }


    // Update is called once per frame
    void Update()
    {

        // If close enough to current waypoint, advance to the next
        float sqrDist = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(currentPath[currentWaypointIndex].x, 0, currentPath[currentWaypointIndex].z)).sqrMagnitude;
        float reach = Mathf.Max(waypointReachThreshold, agent.stoppingDistance + 0.1f);
        if (sqrDist <= reach * reach)
        {
            AdvanceWaypoint();
        }
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


    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
            
        int dmg = collision.gameObject.GetComponent<Bullet>().attackDmg;

        Debug.Log($"Damage taken = {dmg}");

        // Apply damage
        currentHealth -= dmg;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Goblin Died");
        }
    }
}

