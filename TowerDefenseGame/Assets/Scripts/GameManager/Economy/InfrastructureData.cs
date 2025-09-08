using UnityEngine;

[CreateAssetMenu(fileName = "InfrastructureData", menuName = "Economy/Infrastructure Data")]
public class InfrastructureData : ScriptableObject
{
    [Header("Basic")]
    public string name;
    public Sprite icon;

    [Header("Economy")]
    public double baseCost;                 //basic Cost of the base
    public double incomePerTick;            //Income every tivk
    public double costMultiplier;           //Infrastructure becomes more expensive as you buy it.

}
