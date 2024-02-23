using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private GameObject shopNameCanvas;
    [SerializeField] private GameObject shopCatalogueCanvas;

    bool isShopNameActive;
    bool isShopCatalogueActive;

    public static ShopUIController Instance = null;

    public Button buttonEDDec;
    public Button buttonEDInc;
    public Button buttonTCDec;
    public Button buttonTCInc;
    public Button buttonAMDec;
    public Button buttonAMInc;
    public Button buttonBuy;

    public TMP_Text EDCountText;
    public TMP_Text TCCountText;
    public TMP_Text AMCountText;
    public TMP_Text totalText;
    public TMP_Text moneyText;

    public GameObject eDrink;
    public GameObject tCharge;
    public GameObject ammo;


    private int EDCount = 0;
    private int TCCount = 0;
    private int AMCount = 0;

    private int EDCost = 10;
    private int TCCost = 20;
    private int AMCost = 40;
    private int money = 0;
    private int cost = 0;

    private void Awake()
    {
            Instance = this;   
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
            moneyText.text = "Money: $" + money.ToString();
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


    void OnEnable()
    {
        //Register Button Events
        buttonEDDec.onClick.AddListener(() => buttonCallBack(buttonEDDec));
        buttonEDInc.onClick.AddListener(() => buttonCallBack(buttonEDInc));
        buttonTCDec.onClick.AddListener(() => buttonCallBack(buttonTCDec));
        buttonTCInc.onClick.AddListener(() => buttonCallBack(buttonTCInc));
        buttonAMDec.onClick.AddListener(() => buttonCallBack(buttonAMDec));
        buttonAMInc.onClick.AddListener(() => buttonCallBack(buttonAMInc));
        buttonBuy.onClick.AddListener(() => buttonCallBack(buttonBuy));
    }

    private void buttonCallBack(Button buttonPressed)
    {
        if (buttonPressed == buttonEDDec)
        {
            if (EDCount > 0)
            {
                EDCount--;
                EDCountText.text = EDCount.ToString();
                cost -= EDCost;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonEDInc)
        {
            EDCount++;
            EDCountText.text = EDCount.ToString() + "";
            cost += EDCost;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonTCDec)
        {
            if (TCCount > 0)
            {
                TCCount--;
                TCCountText.text = TCCount.ToString() + "";
                cost -= TCCost;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonTCInc)
        {
            TCCount++;
            TCCountText.text = TCCount.ToString() + "";
            cost += TCCost;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonAMDec)
        {
            if (AMCount > 0)
            {
                AMCount--;
                AMCountText.text = AMCount.ToString() + "";
                cost -= AMCost;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonAMInc)
        {
            AMCount++;
            AMCountText.text = AMCount.ToString() + "";
            cost += AMCost;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonBuy)
        {
            if (money - cost < 0)
                return;

            for (int i = 0; i < EDCount; i++)
            {
                GameObject ed = Instantiate(eDrink, transform.position, Quaternion.identity);
            }

            for (int i = 0; i < TCCount; i++)
            {
                GameObject tc = Instantiate(tCharge, transform.position, Quaternion.identity);
            }

            for (int i = 0; i < AMCount; i++)
            {
                GameObject am = Instantiate(ammo, transform.position, Quaternion.identity);
            }

            money -= cost;
            moneyText.text = "Money: $" + money.ToString();
        }
    }

    public void SetMoney(int moneyadded)
    {
        money += moneyadded;
    }

    public int GetMoney()
    {
        return money;
    }



    void OnDisable()
    {
        //Un-Register Button Events
        buttonEDInc.onClick.RemoveAllListeners();
        buttonEDDec.onClick.RemoveAllListeners();
        buttonAMInc.onClick.RemoveAllListeners();
        buttonAMDec.onClick.RemoveAllListeners();
        buttonTCDec.onClick.RemoveAllListeners();
        buttonTCInc.onClick.RemoveAllListeners();
        buttonBuy.onClick.RemoveAllListeners();
    }

}
