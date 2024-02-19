using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
    private Animator animator;

    [SerializeField] private Collider doorCollider;

    private bool isOpen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
}