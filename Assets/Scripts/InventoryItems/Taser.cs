using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taser : MonoBehaviour, IInventoryItem
{
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private Transform[] arcPoints;
    [SerializeField] private GameObject electricArc;
    private Rigidbody taserRB;
    private Collider taserCol;
    private bool isPickup = false;

    private void Awake()
    {
        taserRB = GetComponent<Rigidbody>();
        taserCol = GetComponent<Collider>();
    }

    public string GetItemName()
    {
        return "Taser";
    }

    public string GetItemDescription()
    {
        return "A bright yellow taser for a bright yellow soul!\n...It hurts.";
    }

    public Sprite GetItemDisplaySprite()
    {
        return null;
    }

    public Action GetItemEffect()
    {
        return delegate 
        {
            ShootTaser();
        };
    }

    private void ShootTaser()
    {
        StartCoroutine(OnShoot());
    }

    private IEnumerator OnShoot()
    {
        RaycastHit hit;
        if (!Physics.Raycast(raycastPoint.position, Camera.main.transform.forward, out hit, 20f))
            yield break;

        electricArc.SetActive(true);
        float dist = Vector3.Distance(transform.position, hit.point);
        int numSegments = arcPoints.Length;
        float segmentDistance = dist / (numSegments + 1);

        Vector3 direction = (hit.point - transform.position).normalized;
        arcPoints[0].position = hit.point;
        Vector3 directionXZ = new Vector3(direction.x, 0f, direction.z).normalized;

        for (int i = 1; i < arcPoints.Length - 1; i++)
        {
            Vector3 arcPointPosition = transform.position + directionXZ * segmentDistance * (i + 1);
            arcPointPosition.y = arcPoints[i].position.y;
            arcPoints[i].position = arcPointPosition;
        }

        if (hit.collider.TryGetComponent(out Guard guard))
            guard.ChangeState(Guard.GuardState.STUNNED);

        yield return new WaitForSeconds(0.75f);

        electricArc.SetActive(false);
    }

    public bool GetItemIsStackable()
    {
        return false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.AddItem(this);
            taserRB.isKinematic = true;
            taserCol.isTrigger = true;
            isPickup = true;
            transform.localPosition = Vector3.zero;
            transform.SetParent(PlayerController.Instance.itemHoldPoint);
        }
    }

    private void LateUpdate()
    {
        if (!isPickup)
            return;

        transform.localRotation = Quaternion.Euler(0, -90, 0);
        transform.right = Camera.main.transform.forward;
    }
}