using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    /*Manages Gameplay after The Map has Spawned
     * Manages Economy
     */

<<<<<<< Updated upstream
=======
    public static GamePlayManager Instance {  get; private set; }

    public GameObject canvas;
    public TextMeshProUGUI gameOverText;
    public Sprite pause;
    public Sprite resume;

    public Button pauseBtn;

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

    public void GameOver(bool win)
    {
        if (win)
        {
            canvas.SetActive(true);
            gameOverText.text = "You Win";
            Time.timeScale = 0f;
        }
        else
        {
            canvas.SetActive(true);
            gameOverText.text = "You Lose";
            Time.timeScale = 0f;
        }
    }

    public void TogglePause()
    {
        if(Time.timeScale > 0f)
        {
            Time.timeScale = 0f;
            pauseBtn.image.sprite = resume;
        }
        else
        {
            Time.timeScale = 1.0f;
            pauseBtn.image.sprite = pause;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SpeedUp()
    {
        Time.timeScale = 2.0f;
        pauseBtn.image.sprite = pause;
    }

    public void NormalSpeed()
    {
        Time.timeScale = 1.0f;
        pauseBtn.image.sprite = pause;
    }
>>>>>>> Stashed changes

}
