using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;

    private void Start()
    {
        SetSliders();

        musicSlider.onValueChanged.AddListener((volume) =>
        {
            ChangeMusicVolume(volume);
        });

        soundSlider.onValueChanged.AddListener((volume) =>
        {
            ChangeSoundVolume(volume);
        });
    }

    public void ChangeMusicVolume(float volume)
    {
        AudioManager.instance.SetMusic(volume);
    }

    public void ChangeSoundVolume(float volume)
    {
        AudioManager.instance.SetSound(volume);
    }

    private void SetSliders()
    {
        float musicVolume = PlayerPrefs.HasKey("Music") ? PlayerPrefs.GetFloat("Music") : 1f;
        float soundVolume = PlayerPrefs.HasKey("Sound") ? PlayerPrefs.GetFloat("Sound") : 1f;

        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
    }
}
