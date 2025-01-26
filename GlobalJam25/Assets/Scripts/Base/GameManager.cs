using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject settingsMenu;

    public Image[] hpIcons;
    public Sprite[] hpStatus;

    public Button closeButton;

    public int currentHp;

    private bool pause;

    private void Start()
    {
        Time.timeScale = 1;

        UpdateHp(6);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if(pause) CloseSettings();
            else OpenSettings();
        }
    }

    public void QuitGame()
    {
        AudioManager.instance.PlayClick();
        SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(true);
        Time.timeScale = 0;
        closeButton.Select();
        pause = true;
    }

    public void CloseSettings()
    {
        AudioManager.instance.PlayClick();
        settingsMenu.SetActive(false);
        Time.timeScale = 1;
        pause = false;
    }

    public void UpdateHp(int x)
    {
        currentHp = Mathf.Clamp(currentHp + x, 0 , 6);

        if(currentHp == 0) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        int aux = currentHp;

        foreach (var hp in hpIcons)
        {
            if (aux == 0)
            {
                hp.sprite = hpStatus[2];
            }
            else if (aux == 1)
            {
                hp.sprite = hpStatus[1];
                aux = 0;
            }
            else
            {
                hp.sprite = hpStatus[0];
                aux -= 2;
            }
        }
    }

    public void LoadNextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index == 1)
        {
            PlayerPrefs.SetInt("level", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene(2);
        }
        else if (index == 2)
        {
            PlayerPrefs.SetInt("level", 2);
            PlayerPrefs.Save();
            SceneManager.LoadScene(3);
        }
        else SceneManager.LoadScene(0);
    }
}
