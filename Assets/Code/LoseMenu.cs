using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    void Start()
    {
        // We lost, health shouldn't be saved anymore.
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Defense");
        PlayerPrefs.DeleteKey("Max Health");
        PlayerPrefs.DeleteKey("Speed");
        PlayerPrefs.DeleteKey("Damage");

    }

    public void StartOver()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
