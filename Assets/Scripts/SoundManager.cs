using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static AudioClip death, menuHover, menuSelect, menuBack, menuClose, menuOpen, buttonClick, jump, pickup;    

    static AudioSource audioSrc;
    public AudioMixer audioMixer;

    public static SoundManager instance; // Singleton instance of SoundManager

    void Awake()
    {
        // Check if an instance of SoundManager already exists
        if (instance == null)
        {
            instance = this; // Assign this instance to the static instance
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
            audioSrc = GetComponent<AudioSource>(); // Get the AudioSource component attached to this GameObject
        }
        else
        {
            Destroy(gameObject); // Destroy this object if another instance already exists
        }
    }


    void Start()
    {
        // Load audio clips from Resources folder
        death = Resources.Load<AudioClip>("Death");
        menuHover = Resources.Load<AudioClip>("MenuHover");
        menuSelect = Resources.Load<AudioClip>("MenuSelect");
        menuBack = Resources.Load<AudioClip>("MenuBack");
        menuOpen = Resources.Load<AudioClip>("MenuOpen");
        menuClose = Resources.Load<AudioClip>("MenuClose");
        buttonClick = Resources.Load<AudioClip>("ButtonClick");
        jump = Resources.Load<AudioClip>("Jump");
        pickup = Resources.Load<AudioClip>("Pickup");
        // Set initial volume levels from PlayerPrefs
        audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master")) * 20);
        audioMixer.SetFloat("Sound", Mathf.Log10(PlayerPrefs.GetFloat("Sound")) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
    }

    
    public void SetVolume(AudioMixerGroup targetGroup, float value)
    {
        audioMixer.SetFloat(targetGroup.name, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(targetGroup.name, value);
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "death":
                audioSrc.PlayOneShot(death);
                break;
            case "menuHover":
                audioSrc.PlayOneShot(menuHover);
                break;
            case "menuSelect":
                audioSrc.PlayOneShot(menuSelect);
                break;
            case "menuBack":
                audioSrc.PlayOneShot(menuBack);
                break;
            case "menuOpen":
                audioSrc.PlayOneShot(menuOpen);
                break;
            case "menuClose":
                audioSrc.PlayOneShot(menuClose);
                break;
            case "buttonClick":
                audioSrc.PlayOneShot(buttonClick);
                break;
            case "jump":
                audioSrc.PlayOneShot(jump);
                break;
            case "pickupSFX":
                audioSrc.PlayOneShot(pickup);
                break;
            default:
                Debug.LogWarning("Sound not found: " + clip);
                break;
        }
    }

}
