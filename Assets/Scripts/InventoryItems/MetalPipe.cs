using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPipe : MonoBehaviour, IInventoryItem
{
    private SoundEmitter soundEmitter;
    private Rigidbody pipeRB;
    [SerializeField] private Sprite itemDisplayImage = null;

    private bool onCollide = false;
    private bool isEnabled = false;
    private bool hasRan = true;

    private float _force = 0f;
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
        return itemDisplayImage;
    }

    public Action GetItemEffect()
    {
        return delegate 
        {
            isEnabled = true;
            hasRan = false;
            _force = 5f;
        };
    }

    private void Update()
    {
        if (isEnabled && Input.GetMouseButton(0))
        {
            _force += Time.deltaTime * 10f;
            _force = Mathf.Clamp(_force, 5f, 15f);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else if (!hasRan && isEnabled && !Input.GetMouseButton(0))
        {
            transform.parent = null;
            GetComponent<Collider>().enabled = true;
            pipeRB.useGravity = true;
            pipeRB.AddForce(Camera.main.transform.forward * _force + Camera.main.transform.up * 5f, ForceMode.Impulse);
            isEnabled = false;
            _force = 0f;
            hasRan = true;
        }

        if (!pipeRB.useGravity)
        {
            transform.SetParent(PlayerController.Instance.rightHandPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            if (!onCollide)
                soundEmitter.EmitSound(SoundWPosition.SoundType.IMPORTANT);
        }
        else
            pipeRB.useGravity = false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool GetItemIsConsumable()
    {
        return true;
    }

    public bool GetFollowHoldPoint()
    {
        return false;
    }
}