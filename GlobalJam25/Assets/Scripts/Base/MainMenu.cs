using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;

    public void PlayGame()
    {
        AudioManager.instance.PlayClick();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        AudioManager.instance.PlayClick();
        Application.Quit();
    }

    public void OpenSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(false);
    }
}
