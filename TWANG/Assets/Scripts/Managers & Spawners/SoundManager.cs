using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

/// <summary>
/// manages sound effects playback and provides global access through singleton pattern
/// </summary>
public class SoundManager : MonoBehaviour
{
    // constants for sound configuration
    private const float DEFAULT_PITCH_VARIATION = 0.1f;
    private const float PITCH_MODIFIER_MULTIPLIER = 0.05f;
    private const float SOUND_CLEANUP_DELAY_MULTIPLIER = 1.5f;


    [SerializeField] private AudioClip regularMusic, menuMusic;
    [SerializeField] private AudioSource musicPlayer;

    //generify and move this later
    public void SwitchToMenuMusic()
    {
        var playback = musicPlayer.time;
        musicPlayer.clip = menuMusic;
        musicPlayer.time = playback;
        musicPlayer.Play();
    }
    public void SwitchToRegularMusic()
    {
        var playback = musicPlayer.time;
        musicPlayer.clip = regularMusic;
        musicPlayer.time = playback;
        musicPlayer.Play();
    }

    // serialized fields for configuration
    [SerializeField]
    [Range(0, 1)]
    private float volumeMultiplier = 1;

    [SerializeField]
    private List<SoundEffect> soundEffects;

    // singleton instance
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    /// <summary>
    /// initializes the singleton instance
    /// </summary>
    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// plays the default click sound effect
    /// </summary>
    public void PlayClickSoundEffect() => PlaySoundEffect("click");

    /// <summary>
    /// plays a sound effect by name with default modifier
    /// </summary>
    public void PlaySoundEffect(string soundName) => PlaySoundEffect(soundName, 0);

    /// <summary>
    /// plays a sound effect with specified name and pitch modifier
    /// </summary>
    /// <param name="soundName">name of the sound effect to play</param>
    /// <param name="modifier">pitch modifier value</param>
    public void PlaySoundEffect(string soundName, int modifier)
    {
        SoundEffect? soundEffect = GetRandomMatchingSoundEffect(soundName);
        if (!soundEffect.HasValue || !HasClips(soundEffect.Value)) return;

        AudioClip clip = GetRandomClip(soundEffect.Value);
        GameObject soundObject = CreateSoundGameObject(soundName, clip);
        ConfigureAndPlaySound(soundObject, soundEffect.Value, clip, modifier);
    }

    /// <summary>
    /// finds a random sound effect matching the given name
    /// </summary>
    private SoundEffect? GetRandomMatchingSoundEffect(string soundName)
    {
        var matchingEffects = soundEffects
            .Where(s => s.SoundName.Equals(soundName))
            .ToList();

        if (!matchingEffects.Any()) return null;

        return matchingEffects[Random.Range(0, matchingEffects.Count)];
    }

    /// <summary>
    /// checks if the sound effect has any clips available
    /// </summary>
    private bool HasClips(SoundEffect soundEffect)
    {
        return soundEffect.Clips != null && soundEffect.Clips.Count > 0;
    }

    /// <summary>
    /// gets a random clip from the sound effect
    /// </summary>
    private AudioClip GetRandomClip(SoundEffect soundEffect)
    {
        return soundEffect.Clips[Random.Range(0, soundEffect.Clips.Count)];
    }

    /// <summary>
    /// creates a game object to play the sound effect
    /// </summary>
    private GameObject CreateSoundGameObject(string soundName, AudioClip clip)
    {
        var soundObject = new GameObject($"Sound: {soundName}, {clip.length}s");
        soundObject.transform.parent = transform;
        Destroy(soundObject, clip.length * SOUND_CLEANUP_DELAY_MULTIPLIER);
        return soundObject;
    }

    /// <summary>
    /// configures and plays the sound effect
    /// </summary>
    private void ConfigureAndPlaySound(GameObject soundObject, SoundEffect soundEffect, AudioClip clip, int modifier)
    {
        AudioSource source = AddAndConfigureAudioSource(soundObject, clip, soundEffect);
        ApplyPitchModification(source, soundEffect, modifier);
        source.Play();
    }

    /// <summary>
    /// adds and configures the audio source component
    /// </summary>
    private AudioSource AddAndConfigureAudioSource(GameObject soundObject, AudioClip clip, SoundEffect soundEffect)
    {
        var source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = soundEffect.Volume * volumeMultiplier;
        return source;
    }

    /// <summary>
    /// applies pitch modifications to the audio source
    /// </summary>
    private void ApplyPitchModification(AudioSource source, SoundEffect soundEffect, int modifier)
    {
        if (soundEffect.Vary)
        {
            source.pitch += Random.Range(-DEFAULT_PITCH_VARIATION, DEFAULT_PITCH_VARIATION);
        }
        source.pitch += PITCH_MODIFIER_MULTIPLIER * modifier;
    }
}

/// <summary>
/// data structure to define a sound effect with its properties
/// </summary>
[System.Serializable]
public struct SoundEffect
{
    private string name;

    public string SoundName;
    public List<AudioClip> Clips;

    [Range(0, 1)]
    public float Volume;

    public bool Vary;
}