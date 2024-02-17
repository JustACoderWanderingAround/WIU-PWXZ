using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemController : MonoBehaviour
{
    public static ShopItemController Instance = null;

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
    public GameObject player;


    private int EDCount = 0;
    private int TCCount = 0;
    private int AMCount = 0;

    private int money = 0;
    private int EDcost = 0;
    private int TCcost = 0;
    private int AMCost = 0;
    private int cost = 0;

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    Destroy(gameObject.transform.parent.gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(this);

        Instance = this;
        moneyText.text = "Money: $" + money.ToString();
        //moneyText.text = money.ToString();
    }

    void OnEnable()
    {
        //Register Button Events
        buttonEDDec.onClick.AddListener(() => buttonCallBack(buttonEDDec));
        buttonEDInc.onClick.AddListener(() => buttonCallBack(buttonEDInc));
        buttonTCInc.onClick.AddListener(() => buttonCallBack(buttonTCDec));
        buttonTCInc.onClick.AddListener(() => buttonCallBack(buttonTCInc));
        buttonAMInc.onClick.AddListener(() => buttonCallBack(buttonAMDec));
        buttonAMInc.onClick.AddListener(() => buttonCallBack(buttonAMInc));
        buttonBuy.onClick.AddListener(() => buttonCallBack(buttonBuy));
    }

    private void buttonCallBack(Button buttonPressed)
    {
        if (buttonPressed == buttonEDDec)
        {
            //Your code for button 2
            if (EDCount > 0)
            {
                EDCount--;
                EDCountText.text = EDCount.ToString();
                cost -= 5;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonEDInc)
        {
            //Your code for button 1
            EDCount++;
            EDCountText.text = EDCount.ToString() + "";
            cost += 5;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonTCDec)
        {
            //Your code for button 3
            if (TCCount > 0)
            {
                TCCount--;
                TCCountText.text = TCCount.ToString() + "";
                cost -= 10;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonTCInc)
        {
            //Your code for button 3
            TCCount++;
            TCCountText.text = TCCount.ToString() + "";
            cost += 10;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonAMDec)
        {
            if (AMCount > 0)
            {
                //Your code for button 3
                AMCount--;
                AMCountText.text = AMCount.ToString() + "";
                cost -= 15;
                totalText.text = "Total: $" + cost.ToString();
                moneyText.text = "Money: $" + money.ToString();
            }
        }

        if (buttonPressed == buttonAMInc)
        {
            //Your code for button 3
            AMCount++;
            AMCountText.text = AMCount.ToString() + "";
            cost += 15;
            totalText.text = "Total: $" + cost.ToString();
            moneyText.text = "Money: $" + money.ToString();
        }

        if (buttonPressed == buttonBuy)
        {
            if (money - cost < 0)
                return;

            //Your code for button 3
            for (int i = 0; i < EDCount; i++)
            {
                GameObject ed = Instantiate(eDrink, player.transform.position,Quaternion.identity);
            }

            for (int i = 0; i < TCCount; i++)
            {
                GameObject tc = Instantiate(tCharge, player.transform.position, Quaternion.identity);
            }

            for (int i = 0; i < AMCount; i++)
            {
                GameObject am = Instantiate(ammo, player.transform.position, Quaternion.identity);
            }

            money -= cost;
            moneyText.text = "Money: $" + money.ToString();
        }
    }

    public void AddMoney(int moneyadded)
    {
        money += moneyadded;
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
