using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   private static AudioManager instance;

    [SerializeField] private AudioSource _audioSource;
    private readonly Dictionary<string, AudioClip>
        _clips = new Dictionary<string, AudioClip>();
    public static AudioManager Instance
    {
        get {return instance;}       
    }

    private void Awake()
    {
        if(null != instance)
        {
            //既にインスタンスがある場合は自身を破棄する
            Destroy(gameObject);
            return;
        }

        //Sceneを遷移しても破棄されなくする
        DontDestroyOnLoad(gameObject);
        //インスタンスとして保持する
        instance = this;

        //Resource/2D_SEディレクトリ下のAudioClipを全てを取得する
        var audioClips =Resources.LoadAll<AudioClip>("2D_SE");
        foreach(var clip in audioClips)
        {
            //AudioClipをDirectoryに保持しておく
            _clips.Add(clip.name, clip);
        }
    }

    public void Play(string clipName)
    {
        if(!_clips.ContainsKey(clipName))
        {
            //存在しない存在を指定したらエラー
            throw new Exception("Sound" + clipName + "is not defined");
        }
        //指定の名前のクリップに差し替えて再生する
        _audioSource.clip = _clips[clipName];
        _audioSource.Play();
    }
}
