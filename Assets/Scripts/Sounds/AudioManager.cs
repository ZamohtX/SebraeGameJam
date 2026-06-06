using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource busLoopSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip busDrivingClip;
    [SerializeField] private AudioClip busStopClip;
    [SerializeField] private AudioClip theftClip;
    [SerializeField] private AudioClip frustrationClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip fanfareMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic();
        //StartBus();
        //StopBus();
        //PlayTheft();
        //PlayFrustration();
        //PlayClick();
    }

    public void PlayFanfare()
    {
        musicSource.Stop();
        musicSource.clip = fanfareMusic;
        musicSource.Play();
    }

    private void PlayMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StartBus()
    {
        if (busLoopSource.isPlaying)
            return;

        busLoopSource.clip = busDrivingClip;
        busLoopSource.Play();
    }

    public void StopBus()
    {
        busLoopSource.Stop();

        sfxSource.PlayOneShot(busStopClip);
    }

    public void PlayTheft()
    {
        busLoopSource.Stop();

        sfxSource.PlayOneShot(theftClip);
    }
    public void PlayFrustration()
    {
        busLoopSource.Stop();

        sfxSource.PlayOneShot(frustrationClip);
    }
    public void PlayClick()
    {
        sfxSource.PlayOneShot(clickClip);
    }
}