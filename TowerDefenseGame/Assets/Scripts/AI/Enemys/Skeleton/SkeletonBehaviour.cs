using UnityEngine;

public class SkeletonBehaviour : EnemyBehaviour
{
    /*  Will Ignore All Other Shit and Run For Tower 
    *   VERY VULNERABLE TO DARK
    *   HIGH HP
    *   A Late Game Problem >> For PLAYERS MOUHAHAHHAHAHA
    */


    protected override void OnTick()
    {
        if (target != null)
        {
            if (target.gameObject.GetComponent<TowerBehaviour>() == null)
            {
                target = null;
                currentWaypointIndex += 2;
            }

        }

        switch (state)
        {
            case State.Idle:
                animator.SetBool("running", false);
                break;
            case State.Pathing:
                animator.SetBool("running", true);
                break;
            case State.Chasing:
                animator.SetBool("running", true);
                break;
            case State.Attacking:
                animator.SetTrigger("punch_L");
                animator.SetBool("running", false);
                break;
            default:
                animator.SetTrigger("punch_R");
                break;
        }
    }



    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.CompareTag("Player")) return;

        int effect = other.gameObject.GetComponent<Bullet>().effectType;
        Debug.Log($"Effect Number is {effect}. 0 is basic, 1 is plant, 2 is dark");
        int dmg = other.gameObject.GetComponent<Bullet>().attackDmg;

        switch (effect)
        {
            case 0:
                TakeDamage(dmg);
                break;
            case 1:
                TakeDamage(dmg * 2);
                SlowEnemy(2f, 8f);
                break;
            case 2:
                TakeDamage(dmg);
                ContinousDmg(5f, 2f);
                break;
            default:
                TakeDamage(dmg);
                break;
        }
    }
}
