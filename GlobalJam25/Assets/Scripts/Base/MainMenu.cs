using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelsMenu;
    public GameObject settingsMenu;

    public Button openLevels;
    public Button level1;
    public Button level2;
    public Button level3;
    public Button closeMenu;
    public Button openMenu;

    private void Start()
    {
        Time.timeScale = 1;

        openLevels.Select();

        int levels = PlayerPrefs.HasKey("level") ? PlayerPrefs.GetInt("level") : 0;
        if (levels < 1) level2.interactable = false;
        if (levels < 2) level3.interactable = false;
    }

    public void OpenSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(true);
        closeMenu.Select();
    }

    public void CloseSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(false);
        openMenu.Select();
    }

    public void OpenLevels()
    {
        AudioManager.instance.PlayClick();
        mainMenu.SetActive(false);
        levelsMenu.SetActive(true);
        level1.Select();
    }

    public void CloseLevels()
    {
        AudioManager.instance.PlayClick();
        mainMenu.SetActive(true);
        levelsMenu.SetActive(false);
        openLevels.Select();
    }


    public void PlayGame(int x)
    {
        AudioManager.instance.PlayClick();
        SceneManager.LoadScene(x);
    }

    public void QuitGame()
    {
        AudioManager.instance.PlayClick();
        Application.Quit();
    }
}
