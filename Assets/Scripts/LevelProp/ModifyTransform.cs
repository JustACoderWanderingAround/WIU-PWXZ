using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyTransform : MonoBehaviour
{
    // the object to modify
    public GameObject objectToModify;
    // the increase or decrease in magnitude for each transform element
    public Vector3 positionModification = Vector3.zero;
    public Vector3 rotationModification = Vector3.zero;
    public Vector3 scaleModification = Vector3.zero;

    private Vector3 targetPos, targetScale, targetRot;

    float timer;
    public float speed;
    public bool start;
    private void OnEnable()
    {
        start = false;
        // get target transforms
        targetPos = objectToModify.transform.position + positionModification;
        targetRot = objectToModify.transform.rotation.eulerAngles + rotationModification;
        targetScale = objectToModify.transform.localScale + scaleModification;
    }

    private void Update()
    {
        if (start)
        {
            // lerp towards target transforms
            if (timer <= 1)
                timer += speed * Time.deltaTime;
            objectToModify.transform.position = Vector3.Lerp(objectToModify.transform.position, targetPos, timer);
            objectToModify.transform.localScale = Vector3.Lerp(objectToModify.transform.localScale, targetScale, timer);
            objectToModify.transform.localRotation = Quaternion.Euler(Vector3.Lerp(objectToModify.transform.localRotation.eulerAngles, targetRot, timer));
        }
    }
    public void Activate()
    {
        start = true;
    }
}
