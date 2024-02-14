using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float emissionRange;

    public void EmitSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.Play();

        SoundWPosition sound = new SoundWPosition(audioSource, transform.position, emissionRange);

        Collider[] colliders = Physics.OverlapSphere(sound.position, sound.range);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out IHear listener))
                listener.RespondToSound(sound);
        }
    }
}