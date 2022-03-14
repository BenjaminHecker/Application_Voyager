using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [HideInInspector] public bool mainTheme;

    // key: reference oject, value: sound player
    [HideInInspector] public Dictionary<GameObject, GameObject> tempSounds;
    
    public Sound[] sounds;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        tempSounds = new Dictionary<GameObject, GameObject>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("Theme");
        mainTheme = true;
    }

    private void Update()
    {
        foreach (var d in tempSounds)
        {
            d.Value.transform.position = new Vector3(d.Key.transform.position.x, d.Key.transform.position.y, Camera.main.transform.position.z);
        }
    }

    public void PlayTheme(bool theme)
    {
        if (mainTheme != theme)
        {
            if (theme)
            {
                Play("Theme");
                Stop("Space Ambient");
            }
            else
            {
                Play("Space Ambient");
                Stop("Theme");
            }
        }
        mainTheme = theme;
    }

    public void Play(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Play();
                return;
            }
        }
        Debug.LogWarning("Sound " + name + " not found");
    }

    private void Stop(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
                return;
            }
        }
        Debug.LogWarning("Sound " + name + " not found");
    }

    public void Play3D(string name, GameObject reference, float minDist, float maxDist)
    {
        if (mainTheme)
            return;
        
        if (tempSounds.ContainsKey(reference))
        {
            tempSounds[reference].GetComponent<AudioSource>().Play();
            return;
        }
        
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                GameObject gameObject = new GameObject("TempSound: " + reference.name);
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = s.clip;
                audioSource.volume = s.volume;
                audioSource.pitch = s.pitch;
                audioSource.loop = s.loop;
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = minDist;
                audioSource.maxDistance = maxDist;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.Play();

                tempSounds.Add(reference, gameObject);
            }
        }
    }

    public void Stop3D(GameObject reference)
    {
        if (tempSounds.ContainsKey(reference))
        {
            GameObject tempSound = tempSounds[reference];

            tempSounds.Remove(reference);
            Destroy(tempSound);
        }
    }
}
