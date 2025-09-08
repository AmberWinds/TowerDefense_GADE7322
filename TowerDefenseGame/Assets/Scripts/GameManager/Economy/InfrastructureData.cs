using UnityEngine;

[CreateAssetMenu(fileName = "Infrastructure", menuName = "Data/Business", order = 1)]

public class InfrastructureData : ScriptableObject
{
    public string dataName;
    public double baseCost;
    public double costMultiplier;
    public double incomePerTick;


}
