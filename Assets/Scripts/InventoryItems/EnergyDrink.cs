using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrink : MonoBehaviour, IInventoryItem
{
    [SerializeField] private Sprite itemDisplayImage = null;
    private bool onUse = false;

    public string GetItemName()
    {
        return "Energy Drink";
    }

    public string GetItemDescription()
    {
        return "A delectable can of pure energy!\nI have to go back to work...";
    }

    public Sprite GetItemDisplaySprite()
    {
        return itemDisplayImage;
    }

    public Action GetItemEffect()
    {
        return delegate
        {
            onUse = true;
            gameObject.SetActive(true);
            transform.SetParent(PlayerController.Instance.leftHandPoint);
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            StartCoroutine(OnUseDrink());
        };
    }

    private IEnumerator OnUseDrink()
    {
        AnimationController animationController = AnimationController.Instance;
        animationController.ChangeAnimation(animationController.Drinking, 0.1f, animationController.GetAnimationClip(animationController.Drinking).length, 0);
        PlayerController.Instance.SetDontUseStamina(10f);
        AudioManager.Instance.Play("Drink");

        yield return new WaitForSeconds(animationController.GetAnimationClip(animationController.Drinking).length);

        Destroy(gameObject);
    }

    private void Update()
    {
        if (!onUse)
            return;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    public bool GetItemIsStackable()
    {
        return true;
    }

    public bool GetItemIsConsumable()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool GetFollowHoldPoint()
    {
        return false;
    }
}