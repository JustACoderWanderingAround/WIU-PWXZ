using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPipe : MonoBehaviour, IConsumable
{
    private SoundEmitter soundEmitter;
    private Rigidbody pipeRB;

    private void Awake()
    {
        pipeRB = GetComponent<Rigidbody>();
        soundEmitter = GetComponent<SoundEmitter>();
    }

    public void OnObtain()
    {
    }

    public void OnUse()
    {
        pipeRB.isKinematic = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        soundEmitter.EmitSound();
    }
}