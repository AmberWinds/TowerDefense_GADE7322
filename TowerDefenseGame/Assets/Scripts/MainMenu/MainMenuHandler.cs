using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public Canvas rulesCanvas;
    /*
     * Build ASync LoadScreen
     */


    public void PlayGame()
    {
        SceneManager.LoadScene(1);      //SampleScene - AKA main Game
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenRules()
    {
        rulesCanvas.enabled = true;
    }

    public void CloseRules()
    {
        rulesCanvas.enabled = false;
    }
}
