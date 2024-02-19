using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrab : MonoBehaviour
{
    [SerializeField] private Transform leftHandGrabPosition;
    [SerializeField] private Transform rightHandGrabPosition;

    private IInventoryItem itemHandler;

    private void Start()
    {
        itemHandler = GetComponent<IInventoryItem>();
    }

    private bool IsValid(InventorySlot slot)
    {
        if (itemHandler.GetItemIsStackable())
        {
            return slot.itemName == itemHandler.GetItemName();
        }
        return slot.goRef == gameObject;
    }

    private void OnEnable()
    {
        if (itemHandler == null)
            return;

        if (PlayerController.Instance.inventoryManager.items.Find((x) => IsValid(x)) != null)
        {
            if (leftHandGrabPosition != null)
                RigController.Instance.LeftHandGrab(leftHandGrabPosition);

            if (rightHandGrabPosition != null)
                RigController.Instance.RightHandGrab(rightHandGrabPosition);
        }
    }

    private void OnDisable()
    {
        
    }
}