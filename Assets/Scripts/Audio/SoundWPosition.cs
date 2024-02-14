using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWPosition
{
    public readonly Vector3 position;
    public readonly float range;
    public readonly AudioSource source;

    public SoundWPosition(AudioSource src, Vector3 pos, float radius)
    {
        position = pos;
        range = radius;
        source = src;
    }
}