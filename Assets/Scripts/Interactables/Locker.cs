using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
    private Animator animator;

    [SerializeField] private Collider doorCollider;
    [SerializeField] GameObject miniGame;
    
    private bool isOpen = false;
    bool playerInside;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        miniGame.gameObject.SetActive(false);
    }
    IEnumerator StartMiniGame()
    {
        yield return new WaitForSeconds(3);
        miniGame.gameObject.SetActive(true);
    }
    public void OnInteract()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            doorCollider.isTrigger = true;
            animator.SetTrigger("open");
            AudioManager.Instance.Play("LockerOpen");
        }
        else if (!isOpen)
        {
            doorCollider.isTrigger = false;
            animator.SetTrigger("close");
            AudioManager.Instance.Play("LockerClose");
        }
    }
    public void ForceDoorOpen()
    {
        isOpen = true;
        doorCollider.isTrigger = true;
        animator.SetTrigger("open");
        miniGame.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = true;
        }
        else if (other.gameObject.CompareTag("Enemy") )
        {
            if (playerInside)
                StartCoroutine(StartMiniGame());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}