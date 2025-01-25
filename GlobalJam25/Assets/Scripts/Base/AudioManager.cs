using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer musicMixer;
    public AudioMixer soundMixer;

    private AudioSource mAudioSource;

    public AudioClip click;
    public AudioClip bubble;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip hit;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        mAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        LoadVolume();
    }

    private void LoadVolume()
    {
        float musicVolume = PlayerPrefs.HasKey("Music") ? PlayerPrefs.GetFloat("Music") : 1f;
        float soundVolume = PlayerPrefs.HasKey("Sound") ? PlayerPrefs.GetFloat("Sound") : 1f;

        musicMixer.SetFloat("Volume", Mathf.Log10(musicVolume) * 20);
        soundMixer.SetFloat("Volume", Mathf.Log10(soundVolume) * 20);
    }

    public void SetMusic(float volume)
    {
        volume = Mathf.Clamp(volume, 0.001f, 1f);
        musicMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Music", volume);
        PlayerPrefs.Save();
    }

    public void SetSound(float volume)
    {
        volume = Mathf.Clamp(volume, 0.001f, 1f);
        soundMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Sound", volume);
        PlayerPrefs.Save();
    }

    public void PlayClick()
    {
        mAudioSource.pitch = Random.Range(0.8f, 1.2f);
        mAudioSource.PlayOneShot(click);
    }

    public void PlayBubble()
    {
        mAudioSource.pitch = Random.Range(0.8f, 1.2f);
        mAudioSource.PlayOneShot(bubble);
    }

    public void PlayJump()
    {
        mAudioSource.pitch = Random.Range(0.8f, 1.2f);
        mAudioSource.PlayOneShot(jump);
    }

    public void PlayLand()
    {
        mAudioSource.pitch = Random.Range(0.8f, 1.2f);
        mAudioSource.PlayOneShot(land);
    }

    public void PlayHit()
    {
        mAudioSource.pitch = Random.Range(0.8f, 1.2f);
        mAudioSource.PlayOneShot(hit);
    }
}
