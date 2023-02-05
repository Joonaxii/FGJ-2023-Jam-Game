using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager
{
    public const int MAX_AUDIO_INSTANCES = 24;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;

    public Transform audioRoot;

    private Dictionary<string, AudioInstance> _instances = new Dictionary<string, AudioInstance>();
    private List<AudioInstance> _audios = new List<AudioInstance>();

    public void Initialize()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Sfx");

        for (int i = 0; i < clips.Length; i++)
        {
            var c = clips[i];
            var n = c.name.ToLowerInvariant();

            if (_instances.ContainsKey(n)) { continue; }

            var ins = _instances[n] = new AudioInstance(audioRoot, c, MAX_AUDIO_INSTANCES, sfxGroup);
            _audios.Add(ins);
        }
    }

    public void Update()
    {
        for (int i = 0; i < _audios.Count; i++)
        {
            _audios[i].Update();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f, float pitch = 1.0f) => PlaySFX(clip.name, volume, pitch);
    public void PlaySFX(string name, float volume = 1.0f, float pitch = 1.0f)
    {
        name = name.ToLowerInvariant();
        if(_instances.TryGetValue(name, out var aud))
        {
            aud.Play(volume, pitch);
        }
    }
}