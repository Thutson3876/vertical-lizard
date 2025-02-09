using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;

    public float loopPoint = 0.0F;
    
    void Start()
    {
        var existingAudioManagers = GameObject.FindObjectsByType(
            typeof(AudioManager), FindObjectsSortMode.None);

        if (existingAudioManagers.Length > 0)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
        
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
    }

    void Update()
    {
        if (Mathf.Approximately(
                _audioSource.time,
                _audioSource.clip.length))
        {
            _audioSource.Pause();
            _audioSource.time = loopPoint;
            _audioSource.Play();
        }
    }
}
