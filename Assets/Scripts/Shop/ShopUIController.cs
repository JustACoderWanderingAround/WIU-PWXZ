using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
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

            isShopCatalogueActive = !isShopCatalogueActive;
            shopCatalogueCanvas.SetActive(isShopCatalogueActive);
            if (shopCatalogueCanvas.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = shopCatalogueCanvas.activeSelf;
            }
            else if (!shopCatalogueCanvas.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = shopCatalogueCanvas.activeSelf;
            }
            
        
    }

}
