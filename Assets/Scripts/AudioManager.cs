using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        //handle lack of domain reloading lol
        AudioSources.Clear();
    }

    private const float MinRange = 0.5f;
    private const float MaxRange = 25f;
    private static readonly List<AudioSource> AudioSources = new List<AudioSource>();

    private void OnDestroy()
    {
        foreach (var audioSource in AudioSources)
        {
            Destroy(audioSource);
        }
    }

    public static void PlaySound(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        var audioSource = CreateOrGetAudioSource();
        audioSource.transform.position = position;
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }

    private static AudioSource CreateOrGetAudioSource()
    {
        if (AudioSources.Count == 0)
        {
            return CreateAudioSource();
        }

        foreach (var source in AudioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return CreateAudioSource();
    }

    private static AudioSource CreateAudioSource()
    {
        var gameObject = new GameObject("AudioSource");
        var result = gameObject.AddComponent<AudioSource>();
        result.spatialize = true;
        result.spatialBlend = 1;
        result.minDistance = MinRange;
        result.maxDistance = MaxRange;
        AudioSources.Add(result);
        return result;
    }
}
