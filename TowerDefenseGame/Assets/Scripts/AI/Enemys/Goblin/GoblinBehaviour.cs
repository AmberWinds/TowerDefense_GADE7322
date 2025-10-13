using UnityEngine;

public class GoblinBehaviour : EnemyBehaviour
{
    /*
     * Normal Enemy, Default Type. Will be the Enemy for The Easy Waves
     * Only Walks
     */


    private void Start()
    {
        animator.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }



    private void Update()
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
                TakeDamage(dmg);
                SlowEnemy(2f, 5f);
                break;
            case 2:
                TakeDamage(dmg);
                ContinousDmg(3f, 2f);
                break;
            default:
                TakeDamage(dmg);
                break;
        }
    }


}
