using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    /*Manages Gameplay after The Map has Spawned
     */

    public static GamePlayManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
        }
    }


}
