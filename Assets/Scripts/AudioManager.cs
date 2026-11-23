using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip tank;
    [SerializeField] private AudioClip buy;
    [SerializeField] private AudioClip place;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource.clip = music;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void Shoot() { sfxSource.PlayOneShot(shoot); }
    public void Hit() { sfxSource.PlayOneShot(hit); }
    public void Explosion() { sfxSource.PlayOneShot(explosion); }
    public void Tank() { sfxSource.PlayOneShot(tank); }
    public void Buy() { sfxSource.PlayOneShot(buy); }
    public void Place() { sfxSource.PlayOneShot(place); }

    public void Victory()
    {
        musicSource.Pause();
        sfxSource.PlayOneShot(victory);
    }

    public void Defeat()
    {
        musicSource.Pause();
        sfxSource.PlayOneShot(defeat);
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying) musicSource.UnPause();
    }
}