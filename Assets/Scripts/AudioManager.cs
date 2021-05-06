//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AudioManager : Singleton<AudioManager>
//{
//    public AudioClip[] audioClips;

//    public class Track
//    {
//        public AudioSource Audio { get; set; }
//        public int Index { get; set; }
//        public Track(AudioSource source, int index)
//        {
//            Index = index;
//            Audio = source;
//        }
//    }

//    private List<Track> m_tracks = new List<Track>();
//    void Awake()
//    {
//        for(int i = 0; i < audioClips.Length; ++i)
//        {
//            AudioSource source = gameObject.AddComponent<AudioSource>();
//            source.clip = audioClips[i];
//            m_tracks.Add(new Track(source, i));
//        }
//    }

//    void Update()
//    {
//    }

//    IEnumerator PlayAudio_Coroutine(Track track, float minVolume, float maxVolume, float duration)
//    {
//        track.Audio.Play();
//        float dt = 0.0f;
//        while (dt < duration)
//        {
//            dt += Time.deltaTime;
//            float v = minVolume + dt / duration * (maxVolume - minVolume);
//            track.Audio.volume = v;
//            yield return null;//new WaitForSeconds(.1f);
//        }
//        track.Audio.volume = maxVolume;
//    }
//    IEnumerator StopAudio_Coroutine(Track track, float minVolume, float maxVolume, float duration)
//    {
//        float dt = 0.0f;
//        while (dt < duration)
//        {
//            dt += Time.deltaTime;
//            float v = maxVolume - dt / duration * (maxVolume - minVolume);
//            track.Audio.volume = v;
//            yield return null;// new WaitForSeconds(.1f);
//        }
//        track.Audio.Stop();
//    }

//    public void Play(int index, float volume = 1.0f, bool loop = true)
//    {
//        m_tracks[index].Audio.loop = loop;
//        m_tracks[index].Audio.volume = volume;
//        m_tracks[index].Audio.Play();
//    }

//    public void Stop(int index)
//    {
//        m_tracks[index].Audio.Stop();
//    }

//    public void PlayFadeIn(int index, float fadeInTime, float minVolume, float maxVolume, bool loop)
//    {
//        //Play(index, 0.8f, loop);
//        m_tracks[index].Audio.loop = loop;
//        IEnumerator coroutine = PlayAudio_Coroutine(m_tracks[index], minVolume, maxVolume, fadeInTime);
//        StartCoroutine(coroutine);
//    }

//    public void StopFadeOut(int index, float fadeInTime, float minVolume, float maxVolume)
//    {
//        //Stop(index);
//        IEnumerator coroutine = StopAudio_Coroutine(m_tracks[index], minVolume, maxVolume, fadeInTime);
//        StartCoroutine(coroutine);
//    }
//};