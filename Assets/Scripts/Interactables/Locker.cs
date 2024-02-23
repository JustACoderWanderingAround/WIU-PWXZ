using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
    private Animator animator;

    [SerializeField] private Collider doorCollider;
    [SerializeField] GameObject miniGame;

    bool playerInside;

    [SerializeField] private bool isInteracted = false;

    bool IInteractable.isInteracted { get => isInteracted; set => isInteracted = value; }

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
        isInteracted = !isInteracted;

        if (isInteracted)
        {
            doorCollider.isTrigger = true;
            animator.SetTrigger("open");
            AudioManager.Instance.Play("LockerOpen");
        }
        else if (!isInteracted)
        {
            doorCollider.isTrigger = false;
            animator.SetTrigger("close");
            AudioManager.Instance.Play("LockerClose");
        }
    }
    public void ForceDoorOpen()
    {
        isInteracted = true;
        doorCollider.isTrigger = true;
        animator.SetTrigger("open");
        miniGame.gameObject.SetActive(false);
    }
    void ClearMiniGame()
    {
        if (miniGame.gameObject.activeInHierarchy)
        {
            miniGame.gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = true;
        }
        if (other.gameObject.CompareTag("Enemy"))
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
            ClearMiniGame();
        }
    }
}