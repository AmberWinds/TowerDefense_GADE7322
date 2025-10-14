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
    public float maxHealth;
    public int dmg = 50;


    public void EndGame()
    {
        GamePlayManager.Instance.GameOver(false);
    }



}
