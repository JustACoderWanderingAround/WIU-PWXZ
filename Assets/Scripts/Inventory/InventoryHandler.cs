using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created By: Tan Xiang Feng Wayne
/// <summary>
/// This Class Should be used as a intermediate step between ItemManager and BasePickable
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    [SerializeField]
    private InventoryManager manager;

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
    };

    #region DebugOnly
    // Start is called before the first frame update
    void Start()
    {
        manager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        //Loop through all the possible keycodes
        for (int i = 0; i < keyCodes.Length; i++) {
            if (Input.GetKey(keyCodes[i])) { 
                if (Input.GetKeyDown(KeyCode.U))
                {
                    UseItem(i);
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    RemoveItem(i);
                }
            }
        }
    }
    #endregion

    public void Initialise()
    {
        manager.Init();
    }

    public bool AddItemToManager(IInventoryItem item)
    {
        return manager.AddItem(item);
    }

    public bool UseItem(int listIndex)
    {
        //check if the index is correct
        if (listIndex > manager.items.Count || listIndex < 0 || manager.items.Count <= 0)
            return false;

        return manager.UseItem(manager.items[listIndex].uid);
    }

    public bool RemoveItem(int listIndex, bool all = false)
    {
        //check if the index is correct
        if (listIndex > manager.items.Count || listIndex < 0 || manager.items.Count <= 0)
            return false;

        return manager.DiscardItem(manager.items[listIndex].uid, all);
    }
}
