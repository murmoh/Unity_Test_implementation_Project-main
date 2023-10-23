using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource source; // Change private to public


    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        private AudioSource source;

        [Range(0f, 1f)]
        public float volume = 0.7f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        [Range(0.1f, 5f)] // Adjust the range as desired
        public float fadeDuration = 1.0f; // default is 1 second
        [Range(0.5f, 2f)] // Adjust the range as desired. 1 means normal speed.
        public float speed = 1.0f; // default is normal speed

        public void SetSource(AudioSource _source)
        {
            source = _source;
            source.clip = clip;
        }

        public void Play()
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = speed; // Setting the playback speed
        source.PlayOneShot(clip, volume);
    }

    public IEnumerator FadeOut(float fadeTime)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset volume for next time
    }


    public void Stop()
    {
        AudioManager.Instance.StartCoroutine(FadeOut(fadeDuration));
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }

    }

    public Sound[] sounds;

    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        soundDictionary = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            soundDictionary[s.name] = s;
            GameObject _go = new GameObject("Sound_" + s.name);
            s.SetSource(_go.AddComponent<AudioSource>());
            _go.transform.SetParent(this.transform);
        }
    }

    private Dictionary<string, Sound> soundDictionary;

    
    public void Play(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName) && !soundDictionary[soundName].IsPlaying())
        {
            soundDictionary[soundName].Play();
        }
    }

    public void Stop(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Stop();
        }   
    }

    public void StopSound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            soundDictionary[soundName].Stop(); // This will now fade out before stopping
        }
    }

    
    public void FadeOutAndStop(string soundName, float fadeTime = 1.0f) // default fade time is 1 second
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            StartCoroutine(soundDictionary[soundName].FadeOut(fadeTime));
        }
    }


}
