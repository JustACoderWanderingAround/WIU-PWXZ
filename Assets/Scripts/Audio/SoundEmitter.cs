using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float emissionRange;

    public void SetEmissionRange(float newRange)
    {
        emissionRange = newRange;
    }

    public void EmitSound()
    {
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.Play();
        }

        SoundWPosition sound = new SoundWPosition(audioSource, transform.position, emissionRange);

        Collider[] colliders = Physics.OverlapSphere(sound.position, sound.range);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out IEventListener listener))
                listener.RespondToSound(sound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, emissionRange);
    }
}