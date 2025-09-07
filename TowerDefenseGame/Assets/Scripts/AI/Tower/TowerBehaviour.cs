using System;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    /* Tower Behaviour will be attached to the Tower and will activate when the tower has successfully Spawned
     * In Charge of:
     *  - Managing Health.
     *  - Death (like Tower Dies and Calls Death) :)
     * 
     */
    [Header("Health")]
    private int currentHealth;
    public int maxHealth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = currentHealth;
        
    }


    private void OnCollisionEnter(Collision collision)      //Only thing that changes health. No need to check it every frame
    {
        if(collision.gameObject.tag == "EnemyBot")
        {
            currentHealth--;
            if(currentHealth <= 0)
            {
                Debug.Log("Game Over");
            }
            //collision.gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
    }
}
