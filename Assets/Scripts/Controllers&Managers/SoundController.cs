using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;


    [SerializeField] private AudioSource audioSource;


    private void Start()
    {
        PlayMusicLoop(musicClip);
    }

    
    public void PlayMusicLoop(AudioClip musicClip)
    {
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.Play();
    }
    
}
