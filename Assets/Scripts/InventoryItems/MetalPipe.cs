using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPipe : MonoBehaviour, IInventoryItem
{
    private SoundEmitter soundEmitter;
    private Rigidbody pipeRB;
    private bool onCollide = false;

    private void Awake()
    {
        pipeRB = GetComponent<Rigidbody>();
        soundEmitter = GetComponent<SoundEmitter>();
    }

    public string GetItemName()
    {
        return "Metal Pipe";
    }

    public string GetItemDescription()
    {
        return "A sturdy metal pipe. Makes a funny noise when dropped!";
    }

    public bool GetItemIsStackable()
    {
        return false;
    }

    public Sprite GetItemDisplaySprite()
    {
        return null;
    }

    public Action GetItemEffect()
    {
        return delegate { pipeRB.isKinematic = false; pipeRB.transform.position = PlayerController.Instance.itemHoldPoint.position; gameObject.SetActive(true); pipeRB.AddForce(Camera.main.transform.forward * 10f + Camera.main.transform.up, ForceMode.Impulse); };
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.AddItem(this);
            gameObject.SetActive(false);
        }
        else
        {
            if (!onCollide)
                soundEmitter.EmitSound();

            //onCollide = true;
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool GetItemIsConsumable()
    {
        return true;
    }
}