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
    public int dmg = 50;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = currentHealth;
    }

    public void EndGame()
    {
        Debug.Log("End Game");
    }


    private void OnCollisionEnter(Collision collision)      //Only thing that changes health. No need to check it every frame
    {

    }
}
