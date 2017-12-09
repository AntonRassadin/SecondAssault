using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    private AudioSource playerAudio = null;
    private float volume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);

        if (FindObjectOfType<Player>() != null)
        {
            playerAudio = FindObjectOfType<Player>().GetComponent<AudioSource>();
        }
        SetVolume();
    }

    private void Start () {
        SetVolume();
    }
	
	private void Update () {

    }

    public void PlayClipWIthVolume(AudioClip clip, AudioSource audioSource)
    {

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayCLipAtPlayer(AudioClip cLip)
    {
        playerAudio.PlayOneShot(cLip, volume);
    }

    public void SetVolume()
    {
        volume = PlayerPrefs.GetFloat("volume", .5f);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (FindObjectOfType<Player>() != null)
        {
            playerAudio = FindObjectOfType<Player>().GetComponent<AudioSource>();
        }
        SetVolume();
    }

}
