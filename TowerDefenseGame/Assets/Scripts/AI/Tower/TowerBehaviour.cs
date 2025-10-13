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
    private float currentHealth;
    public float maxHealth;
    public int dmg = 50;
    FloatingHealthBar healthBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }


    public void EndGame()
    {
        GamePlayManager.Instance.GameOver(false);
    }


    private void OnCollisionEnter(Collision collision)      //Only thing that changes currentHealth. No need to check it every frame
    {
        if(collision.gameObject.tag == "Enemy")
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
            if(currentHealth <= 0)
            {
                Debug.Log("Game Over");
            }

            Debug.Log("Goblin has Reached the Tower");
        }
    }
}
