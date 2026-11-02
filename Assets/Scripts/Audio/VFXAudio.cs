using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VFXAudio : MonoBehaviour
{
    [SerializeField] private AudioClip vfxSound;
    [SerializeField] private bool isLooping;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (isLooping)
            PlayLoopVFX();

    }

    private void PlayLoopVFX()
    {
        audioSource.clip = vfxSound;
        audioSource.loop = true;
        audioSource.Play();
    }


    public void PlayVFX()
    {
        audioSource.Stop();
        audioSource.clip = vfxSound;
        audioSource.Play();
    }
        
}
