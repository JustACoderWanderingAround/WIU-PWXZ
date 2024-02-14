using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWPosition
{
    public enum SoundType
    {
        INTEREST,
        DANGER
    }

    public SoundType soundType;
    public AudioSource source;

    public readonly Vector3 position;
    public readonly float range;

    public SoundWPosition(AudioSource src, Vector3 pos, float radius)
    {
        position = pos;
        range = radius;
        source = src;
    }
}