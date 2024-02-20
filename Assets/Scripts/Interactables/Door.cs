using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider doorCollider;
    private bool isOpen = false;

    bool interacted;
    public bool isInteracted { get => interacted; set => interacted = value; }

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }
    public void OnInteract()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            doorCollider.isTrigger = true;
            animator.SetTrigger("open");
        }
        else if (!isOpen)
        {
            doorCollider.isTrigger = false;
            animator.SetTrigger("close");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            doorCollider.isTrigger = true;
            animator.SetTrigger("open");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            doorCollider.isTrigger = false;
            animator.SetTrigger("close");
        }
    }
}

