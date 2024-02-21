using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FootprintController : MonoBehaviour
{
    [SerializeField] private GameObject footprintLeft;
    [SerializeField] private GameObject footprintRight;
    private bool isFootLeft = false;
    private float footprintSpacer = 0.5f;
    private Vector3 lastFootprint;
    private float footPosX;

    //checks whether to spawn left or right footprint
    public void CheckFootprint(CapsuleCollider capCollider)
    {
        float distFromLast = Vector3.Distance(lastFootprint, transform.position);
        Debug.Log(distFromLast);
        Debug.Log(footprintSpacer);
        if (distFromLast >= footprintSpacer)
        {
            if (isFootLeft)
            {
                SpawnFootprint(footprintLeft, capCollider);
                isFootLeft = false;
            }
            else
            {
                SpawnFootprint(footprintRight, capCollider);
                isFootLeft = true;
            }
            lastFootprint = transform.position;
        }
    }

    //spawns footprint based on player transform
    private void SpawnFootprint(GameObject footprint, CapsuleCollider capCollider)
    {
        //checks where to place footprint x pos depending on whether its left or right foot
        if (isFootLeft)
        {
            footPosX = this.transform.localPosition.x + 0.15f;
        }
        else
        {
            footPosX = this.transform.localPosition.x;
        }

        Vector3 printPos = new Vector3(footPosX, this.transform.localPosition.y + 0.005f, this.transform.localPosition.z);

        GameObject print = Instantiate(footprint);
        print.transform.position = printPos;
        print.transform.Rotate(Vector3.up, this.transform.eulerAngles.y);
        print.transform.Rotate(Vector3.right, 90.0f);
    }

}
