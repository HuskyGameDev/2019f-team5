//
// When We Fell
//

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Ensure we delete the saved health when starting a new game.
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Defense");
        PlayerPrefs.DeleteKey("Max Health");
        PlayerPrefs.DeleteKey("Speed");
        PlayerPrefs.DeleteKey("Damage");

        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
