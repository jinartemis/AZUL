using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundData : ScriptableObject
{
    public enum SE
    {
        Select,
        Point,
        Error,
        Cansel,
        Touch,
    }

    public enum BGM
    {
        Title,
        Home,
        Game,
        Clear,
        Failed,
    }


    [SerializeField, Header("SE")]
    public AudioClip[] se;

    [SerializeField, Header("BGM")]
    public AudioClip[] bgm;
}
