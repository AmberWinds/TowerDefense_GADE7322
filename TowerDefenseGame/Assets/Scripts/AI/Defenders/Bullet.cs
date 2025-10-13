using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int attackDmg;
    public enum defenderType { basic, plant, dark }
    public int effectType = 0;                                     

    /* 0 is Basic
     * 1 is Plant
     * 2 is Dark
     */
        

    private void Start()
    {
        effectType = 0;
    }

    public void SetType(defenderType bulletType)
    {
        switch(bulletType)
        {
            case defenderType.basic:
                effectType = 0;
                break;
            case defenderType.plant:
                effectType = 1;
                break;
            case defenderType.dark:
                effectType |= 2;
                break;
            default: break;
        }
    }
}
