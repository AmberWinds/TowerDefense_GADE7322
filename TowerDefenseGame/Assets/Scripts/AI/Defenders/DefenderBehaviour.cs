using System;
using Unity.VisualScripting;
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
    private float currentHealth;
    FloatingHealthBar healthBar;

    [Header("Detection")]
    public float attackRadius;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform muzzle;                // Where projectiles spawn from
    public Transform head;                  //Part that looks at player
    public float projectileSpeed = 150f;
    public float fireRate = 0.2f;           // Time between shots
    private Vector3 scale = new Vector3((float)0.2, (float)0.2, (float)0.2);
    public int dmg = 20;



    [Header("Visual")]
    public bool showDetectionRadius = true;     //Gonna have to see the radius

    private GameObject currentTarget;
    private float nextFireTime = 0;

    private enum defenderType { basic, plant, dark }

    [Header("Type")]
    [SerializeField] defenderType type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        
    }


    // Update is called once per frame
    protected void Update()
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
        head.LookAt(currentTarget.transform.position, Vector3.up);    

        Vector3 direction = (currentTarget.transform.position - muzzle.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, head.localRotation, transform);
        projectile.transform.localScale = scale;

        if(gameObject.GetComponent<TowerBehaviour>() != null)
        {
            projectile.GetComponent<Bullet>().attackDmg = gameObject.GetComponent<TowerBehaviour>().dmg;

        }
        else
        {
            switch(type)
            {
                case defenderType.basic:
                    projectile.GetComponent<Bullet>().SetType(Bullet.defenderType.basic);
                    break;
                case defenderType.plant:
                    projectile.GetComponent<Bullet>().SetType(Bullet.defenderType.plant);
                    break;
                case defenderType.dark:
                    projectile.GetComponent<Bullet>().SetType(Bullet.defenderType.dark);
                    break;
                default:
                    projectile.GetComponent<Bullet>().SetType(Bullet.defenderType.basic);
                    break;
            }

            projectile.GetComponent<Bullet>().attackDmg = dmg;

        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;        //Adjust Speed
        }

        Destroy(projectile, 3f);
    }



    private void DetectTargets()
    {
        //Okay, use Physics overlap to get all the colliders
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, attackRadius);

        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        // Find the closest enemy within range
        foreach (var col in enemyColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = col.gameObject;
                }
            }
        }

        // Update current target to the closest enemy
        currentTarget = closestEnemy;
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
                Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            }
        }
    }

    public void BeAttacked(float damageTaken)
    {
        currentHealth -= damageTaken;

        //Move this back into If statement when done testing
        TowerBehaviour towerBehaviour = null;
        towerBehaviour = gameObject.GetComponent<TowerBehaviour>();

        if (currentHealth <= 0)
        {

            if(towerBehaviour != null)
            {
                towerBehaviour.EndGame();
            }
            else
            {
                DefenderPlacement.Instance.SpawnSingleDefenderPlacement(transform.position);
                Destroy(gameObject);
            }

        }
        

        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }


}
