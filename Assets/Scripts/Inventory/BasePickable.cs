using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by: Tan Xiang Feng Wayne
[RequireComponent(typeof(Collider))]
public class BasePickable : MonoBehaviour, IInventoryItem
{
    [Header("Inventory Display")]
    public string itemName;
    public string itemDescription;
    public bool isStackable = false;
    public Sprite itemDisplayImage = null;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    ///Function that to be modified depending on item usage
    ///As a result of this, the gameobject cannot be deleted in order to invoke the effect (if its depending on the specific
    ///game object)
    ///However, if it is a constant effect such as health += 1, just use static
    ///If its gameObject specific effect, remember to set isStackable to 1
    /// </summary>
    /// <returns></returns>
    public System.Action GetItemEffect()
    {
        //Constant Example Start
        if (isStackable)
            return ExampleEffect;
        //Constant Example End
        //GameObject Specifc Example Start
        return delegate { 
            Debug.Log($"{gameObject.name} has Invoked Item Effect");
            //Remember to delete the gameobject after it ended
            //Destroy(gameObject);
        };
        //GameObject Specifc Example End
    }

    private static void ExampleEffect()
    {
        Debug.Log("Static Effect Has Been Called");
    }


    //no more using enter here, all collider will be handled by handler
    /*
    private void OnTriggerEnter(Collider other)
    {
        //Place into player's inventory if it is Player
        if (other.CompareTag("Player"))
        {
            bool hasAdded = other.GetComponent<InventoryHandler>().AddItemToManager(this);
            Debug.Log($"Attempting to add {gameObject.name} to the Inventory Manager: " +
                (hasAdded ? "SUCCESS" : "FAILED"));

            //GameObject Specifc Example Start
            if (hasAdded)
                gameObject.SetActive(false);
            //GameObject Specifc Example End

            //GameObject Specifc Example Start
            //if (hasAdded)
                //Destroy(gameObject);
            //
            //GameObject Specifc Example End
        }
    */

    public string GetItemName()
    {
        return itemName;
    }

    public string GetItemDescription()
    {
        return itemDescription;
    }

    public bool GetItemIsStackable()
    {
        return isStackable;
    }

    public Sprite GetItemDisplaySprite()
    {
        return itemDisplayImage;
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
