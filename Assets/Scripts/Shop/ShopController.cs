using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject shopNameCanvas;
    [SerializeField] private GameObject shopCatalogueCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetShopNameActive(Collider other,bool isActive)
    {
        if (other.gameObject.tag == "Shop")
        {
            shopNameCanvas.SetActive(isActive);
        }
        
    }

    public void SetShopCatalogueActive(bool isActive)
    {
        if (shopNameCanvas.activeSelf)
        {
            shopCatalogueCanvas.SetActive(isActive);
        }
    }

}
