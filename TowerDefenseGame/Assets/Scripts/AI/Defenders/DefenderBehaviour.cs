using System;
using UnityEngine;

public class DefenderBehaviour : MonoBehaviour
{
    /* Will be placed on each defender,
     *  - Attack Enemies within a radius
     *  - Attack One enemy at a time.
     *  
     *  Place on both Main Tower and Defenders
     */

    [Header("Health")]
    public int maxHealth;
    private int currentHealth;

    [Header("Detection")]
    public float attackRadius;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform muzzle;                // Where projectiles spawn from
    public Transform head;                  //Part that looks at player
    public float projectileSpeed = 20f;
    public float fireRate = 0.5f;           // Time between shots

    [Header("Visual")]
    public bool showDetectionRadius = true;     //Gonna have to see the radius

    private Transform currentTarget;
    private float nextFireTime;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectTargets();

        if(currentTarget != null)
        {
            if(Time.time > nextFireTime)
            {
                nextFireTime = Time.time + 1f/fireRate;
                AttackTargets();
            }

            
        }
    }

    private void AttackTargets()
    {
        if (!currentTarget || !muzzle) return;


        head.LookAt(transform.position);
        Vector3 direction = (currentTarget.transform.position - muzzle.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, head.localRotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * projectileSpeed);        //Adjust Speed
        }

        Destroy(projectile, 5f);
    }

    private void DetectTargets()
    {
        //Okay, use Physics overlap to get all the colliders
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, attackRadius);

        if(currentTarget != null)
        {
            //Check if it is in range
            foreach(var col in enemyColliders)
            {
                if(col.gameObject == currentTarget)
                {
                    //Keep Shooting
                    break;  //No need to keep looping
                }
                else
                {
                    //he left, new person to attack
                    currentTarget = null;
                }
            }

        }
        else
        {
            //Find new target
            if(enemyColliders.Length > 0)   //Check there is more enemies available
            {
                currentTarget = enemyColliders[0].transform;
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        if (showDetectionRadius)
        {
            Gizmos.color = currentTarget != null ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Check if enemies be attacking
    }
}
