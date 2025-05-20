using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] musicClips; // Array to hold the music clips
    private AudioSource audioSource; // Reference to the AudioSource component
    int currentClipIndex = 0; // Index to keep track of the current music clip

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to this GameObject
        audioSource.loop = false;
    }

    private void Update()
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying) // Check if the audio source is not playing
            {
                if (currentClipIndex < musicClips.Length) // Check if there are more clips to play
                {
                    audioSource.clip = musicClips[currentClipIndex]; // Set the current clip
                    audioSource.Play(); // Play the current clip
                    currentClipIndex++; // Move to the next clip
                }
                else
                {
                    currentClipIndex = 0; // Reset to the first clip if all have been played
                }
            }
        }
    }
}
