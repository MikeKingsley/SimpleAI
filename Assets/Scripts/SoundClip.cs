using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Sound Clip")]
public class SoundClip : ScriptableObject
{
    public AudioClip sound;
    public bool mute;
    public bool loop;
    public float velocityVolume = 0.2f;
    public float volumeMax = 1;
    public float pitchMin = 1f;
    public float pitchMax = 1.5f;
}
