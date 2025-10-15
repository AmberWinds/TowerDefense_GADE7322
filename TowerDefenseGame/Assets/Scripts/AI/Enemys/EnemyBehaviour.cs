using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public abstract class EnemyBehaviour : MonoBehaviour 
{
    /* Will be placed on the Enemy themselves.
     *  - Manage Navigation.
     *  - Will Manage Destruction / pooling depending.
     */

    [HideInInspector]
    public Animator animator;
    private NavMeshAgent agent;
    private LinkedList<List<Vector3>> paths;
    private List<Vector3> currentPath;

    [HideInInspector]
    public int currentWaypointIndex;
    [SerializeField] private float waypointReachThreshold = 4f;

    private float health;
    private float maxHealth;
    private float attackDmg;
    private float attackRange = 5f;
    private float scanRadius = 12f;
    private float attackRate = 5f;
    private float scanRate = 1f;
    private string enemyType;

    [HideInInspector]
    public GameObject target;
    private DefenderBehaviour targetDef; 
    FloatingHealthBar healthBar;

    private float nextScanTime;
    private float nextAttackTime;

    [HideInInspector]
    public enum State {Idle, Pathing, Chasing, Attacking }
    [HideInInspector]
    public State state = State.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        currentPath = new List<Vector3>();

        currentWaypointIndex = 0;

    }

    public void BeginTracking(Enemy me)
    {
        //Assign Health
        maxHealth = me.health;
        health = maxHealth;
        //Asssign Attack
        attackDmg = me.attackDmg;
        attackRange = me.attackRadius;
        attackRate = me.attackRate;
        enemyType = me.enemyTypeName;

        healthBar.UpdateHealthBar(health, maxHealth);
        state = State.Idle;

        //Need to find the closest path
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
            currentPath = new List<Vector3>(bestPath);
            currentWaypointIndex += 2;
        }

    }


    // Update is called once per frame
    protected void Update()
    {
        if (Time.time >= nextScanTime)
        {
            nextScanTime = Time.time + scanRate;
            RefreshOrAcquireTarget();
        }

        //Chasing
        if (target != null)     //There us a Target
        {
            float Dist = Vector3.Distance(transform.position, target.transform.position);
            if (Dist > attackRange)     //Oustide attack range
            {
                //Chase target Down
                state = State.Chasing;
                agent.isStopped = false;
                agent.stoppingDistance = Mathf.Max(attackRange * 0.9f, 0.1f);
                agent.SetDestination(target.transform.position);
            }
            else
            {
                //At TargetWalls
                state= State.Attacking;
                agent.isStopped = true; //Stop Agent Moving.
                Attack(target);
                return;
            }

        }
        else
        {
            state = State.Pathing;
            agent.isStopped = false;          
            agent.stoppingDistance = 0f;
        }

        //Pathing Logic
        if (currentPath != null && state == State.Pathing)
        {
            if(currentPath.Count == 0)
            {
                Debug.Log("currentPath has no points");
            }

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

        }

        OnTick();
    }

    protected virtual void OnTick() { } // override in children

    public void RefreshOrAcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, scanRadius);
        float best = float.PositiveInfinity;
        GameObject bestGO = null;
        var path = new NavMeshPath();


        foreach (var c in hits)
        {
            var def = c.GetComponent<DefenderBehaviour>();
            if (!def) continue;

            float d = Vector3.SqrMagnitude(c.transform.position - transform.position);
            if (d < best && NavMesh.CalculatePath(transform.position, c.transform.position, NavMesh.AllAreas, path)
                         && path.status == NavMeshPathStatus.PathComplete)
            {
                best = d; 
                bestGO = c.gameObject;
            }

            if(bestGO == null)
            {
                Debug.Log("bestGo is null");
            }
        }

        target = bestGO; // null if none reachable

    }

    public void Attack(GameObject target)
    {
        state = State.Attacking;
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
        targetDef.BeAttacked(attackDmg, enemyType);

    }


    private void MoveToCurrentWaypoint()
    {
        if (currentWaypointIndex >= currentPath.Count - 1)
        {
            currentWaypointIndex = currentPath.Count -1;
        }

        Vector3 target = currentPath[currentWaypointIndex];
        agent.SetDestination(target);

    }

    private void AdvanceWaypoint()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (currentWaypointIndex < currentPath.Count - 1)
        {
            currentWaypointIndex += 3;                                  //trying to make it more smooth by moving two indexes
            MoveToCurrentWaypoint();
        }
        else
        {
            // Reached end of path: stop or handle attacking tower, etc.
            if (agent != null)
            {
                agent.ResetPath();
            }
        }
    }



    //METHODS FOR THE BABIES

    
    public void SlowEnemy(float slowSpeed, float slowDuration)
    {
        float temp = agent.speed;
        agent.speed = slowSpeed;

        StartCoroutine(RemoveSlowAfter(slowDuration, temp));
    }

    public void ContinousDmg(float dmgTaken, float dmgTick)
    {
        StartCoroutine(ContinousDamage(dmgTaken, dmgTick));
    }


    private IEnumerator RemoveSlowAfter(float delay, float ogSpeed)
    {
        yield return new WaitForSeconds(delay);

        agent.speed = ogSpeed;
    }

    private IEnumerator ContinousDamage(float dmgTaken, float dmgTick)          //UNTIL DEATH
    {
        while (health >= 0)                                                    
        {
            yield return new WaitForSeconds(dmgTick);
            TakeDamage(dmgTaken);
            Debug.Log("Continous Damage Taken");
        }
    }


    public void TakeDamage(float dmg)
    {
        health -= dmg;
        healthBar.UpdateHealthBar(health, maxHealth);

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }


}



//OLD CODE

//private void OnTriggerEnter(Collider other)
//{
//    if (!other.gameObject.CompareTag("Player")) return;

//    int dmg = other.gameObject.GetComponent<Bullet>().attackDmg;

//    // Apply damage
//    health -= dmg;
//    healthBar.UpdateHealthBar(health, maxHealth);
//    Debug.Log($"Been shot at! Took {dmg} damage, health now {health}");


//    if (health <= 0)
//    {
//        Destroy(gameObject);
//        Debug.Log("Goblin Died");
//    }
//}
