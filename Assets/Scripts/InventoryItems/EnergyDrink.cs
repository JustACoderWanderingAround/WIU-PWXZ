using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrink : MonoBehaviour, IInventoryItem
{
    private Rigidbody drinkRB;
    private Collider drinkCol;
    private bool isPickup = false;

    private void Awake()
    {
        drinkRB = GetComponent<Rigidbody>();
        drinkCol = GetComponent<Collider>();
    }

    public string GetItemName()
    {
        return "Energy Drink";
    }

    public string GetItemDescription()
    {
        return "A delectable can of pure energy!\nI have to go back to work...";
    }

    public Sprite GetItemDisplaySprite()
    {
        return null;
    }

    public Action GetItemEffect()
    {
        return delegate
        {
        };
    }

    public bool GetItemIsStackable()
    {
        return false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.AddItem(this);
            isPickup = true;
        }
    }
}
