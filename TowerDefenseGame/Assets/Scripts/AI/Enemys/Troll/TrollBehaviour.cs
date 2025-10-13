using UnityEngine;

public class TrollBehaviour : EnemyBehaviour 
{
    /*  Moves Slowest But has the Most Damage
     *  Will Show Up in Mid Levels
     *  HIGH HEALTH, LIKE HIGH >> It Gets Slowed Down ALOT
     *  VERY Vulnerable to PLANTS
     */

    protected override void OnTick()
    {
        switch (state)
        {
            case State.Idle:
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                break;
            case State.Pathing:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                break;
            case State.Chasing:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                break;
            case State.Attacking:
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                break;
            default:
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        int effect = other.gameObject.GetComponent<Bullet>().effectType;
        int dmg = other.gameObject.GetComponent<Bullet>().attackDmg;

        switch (effect)
        {
            case 0:
                TakeDamage(dmg);
                break;
            case 1:
                TakeDamage(dmg * 2);
                SlowEnemy(1f, 10f);
                break;
            case 2:
                TakeDamage(dmg);
                ContinousDmg(3f, 3f);
                break;
            default:
                TakeDamage(dmg);
                break;
        }
    }
}
