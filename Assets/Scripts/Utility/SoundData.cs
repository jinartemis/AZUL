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
        NewRecord,
    }

    public enum BGM
    {
        Title,
        Home,
        Game0,
        Game1,
        Game2,
        Clear0,
        Clear1,
        Clear2,
        Failed,
    }


    [SerializeField, Header("SE")]
    public AudioClip[] se;

    [SerializeField, Header("BGM")]
    public AudioClip[] bgm;
}
