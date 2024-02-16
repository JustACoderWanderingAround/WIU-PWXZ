using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopNameCanvas;
    [SerializeField] private GameObject shopCatalogueCanvas;

    bool isShopNameActive;
    bool isShopCatalogueActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetShopNameActive(Collider other)
    {
        if (other.gameObject.tag == "Shop")
        {
            isShopNameActive = !isShopNameActive;
            shopNameCanvas.SetActive(isShopNameActive);
        }
        
    }

    public void SetShopCatalogueActive()
    {
        if (shopNameCanvas.activeSelf)
        {
            isShopCatalogueActive = !isShopCatalogueActive;
            shopCatalogueCanvas.SetActive(isShopCatalogueActive);
            if (isShopCatalogueActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = isShopCatalogueActive;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = isShopCatalogueActive;
            }
            
        }
    }

}
