using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInstance
{
    private Queue<AudioSource> _sourcePool = new Queue<AudioSource>();
    private List<AudioSource> _playingPool = new List<AudioSource>();

    public AudioInstance(Transform root, AudioClip clip, int instancesMax, AudioMixerGroup group)
    {     
        for (int i = 0; i < instancesMax; i++)
        {
            AudioSource aS = new GameObject($"Audio Source [{clip.name}] #{i}").AddComponent<AudioSource>();
            aS.outputAudioMixerGroup = group;
            aS.clip = clip;
            aS.loop = false;

            aS.transform.parent = root;
            _sourcePool.Enqueue(aS);
        }
    }

    public void Play(float volume, float pitch)
    {
        var aS = GetSource();

        aS.volume = volume;
        aS.pitch = pitch;

        aS.PlayScheduled(AudioSettings.dspTime);

        _playingPool.Add(aS);
    }

    private AudioSource GetSource()
    {
        if(_sourcePool.Count < 1)
        {
            var p = _playingPool[0];
            _playingPool.RemoveAt(0);
            return p;
        }
        return _sourcePool.Dequeue();
    }


    public void Update()
    {
        for (int i = _playingPool.Count - 1; i >= 0; i--)
        {
            var p = _playingPool[i];
            if (!p.isPlaying) 
            { 
                _playingPool.RemoveAt(i);
                _sourcePool.Enqueue(p);
            }
        }
    }
}