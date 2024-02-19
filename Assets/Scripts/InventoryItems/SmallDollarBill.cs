using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDollarBill : MonoBehaviour, IInventoryItem
{
    [SerializeField] private Sprite itemDisplayImage = null;
    //ShopItemController shopItemController;

    public string GetItemName()
    {
        return "100 dollar bill";
    }

    public string GetItemDescription()
    {
        return "A 100 dollar bill";
    }

    public Sprite GetItemDisplaySprite()
    {
        return itemDisplayImage;
    }

    public Action GetItemEffect()
    {
        return delegate
        {
            //ShopItemController.Instance.
            ShopUIController.Instance.SetMoney(100);
            //shopItemController.AddMoney(100);
        };
    }

    public bool GetItemIsConsumable()
    {
        return true;
    }

    public bool GetItemIsStackable()
    {
        return true;
    }


    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool GetFollowHoldPoint()
    {
        throw new NotImplementedException();
    }
}
