using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by: Tan Xiang Feng Wayne
public interface IInventoryItem
{
    public string GetItemName();
    public string GetItemDescription();
    public bool GetItemIsStackable();
    public bool GetItemIsConsumable();
    public bool GetFollowHoldPoint();
    public Sprite GetItemDisplaySprite();
    public System.Action GetItemEffect();

    public GameObject GetGameObject();
}
