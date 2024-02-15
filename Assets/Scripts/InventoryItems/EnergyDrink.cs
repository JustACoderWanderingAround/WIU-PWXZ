using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrink : MonoBehaviour, IInventoryItem
{
    [SerializeField] private Sprite itemDisplayImage = null;

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
        return itemDisplayImage;
    }

    public Action GetItemEffect()
    {
        return delegate
        {
            PlayerController.Instance.SetDontUseStamina(10f);
        };
    }

    public bool GetItemIsStackable()
    {
        return true;
    }

    public bool GetItemIsConsumable()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}