using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RotaryLocker : MiniGame
{
    public GameObject rotaryDial;
    public GameObject rotaryShackle;
    public List<float> speedModifier = new List<float>() { 1.0f, 1.2f, 1.5f };
    List<float> rotationModifer = new List<float>() { 1, -1, 1 };

    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text timerText;

    float rotationSpeed = 50.0f;
    int curr;
    int tolerance = 3;
    int target;
    private new void OnEnable()
    {
        base.OnEnable();
        curr = 0;
        target = UnityEngine.Random.Range(0, 100);
        mainText.text = "Code: " + target + "\nNumbers left: " + (3 - curr).ToString();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = Math.Truncate(timer).ToString();
        Vector3 angles = rotaryDial.transform.rotation.eulerAngles;
        if (curr < rotationModifer.Count)
            angles.z += (rotationSpeed * rotationModifer[curr] * speedModifier[curr]) * Time.deltaTime; // + rotationSpeed for right button
        if (angles.z > 360)
        {
            angles.z -= 360;
        }
        rotaryDial.transform.rotation = Quaternion.Euler(angles);
        if (Input.GetMouseButtonDown(0))
        {
            StopRotation();
        }
        if (curr >= 3)
        {
            OnWin();
        }
        if (timer < 0)
        {
            OnLose();
        }
    }
    void StopRotation()
    {
        float currRotation = rotaryDial.transform.rotation.eulerAngles.z;
        if (currRotation < 0)
            currRotation += 180;
        float newNumber = currRotation / 3.6f;
        double newInt = Math.Truncate(newNumber);
       if (newInt > target - tolerance && newInt < target + tolerance)
       {
            curr++;
            target = UnityEngine.Random.Range(0, 100);
            mainText.text = "Code: " + target + "\nNumbers left: " + (3 - curr).ToString();
       }
    }
    IEnumerator UnlockAnimation()
    {
        while (rotaryShackle.transform.position.x < 240)
        {
            transform.position += new Vector3(5, 0);
            yield return null;
        }
    }
    protected override void OnWin()
    {
        StartCoroutine(UnlockAnimation());
        base.OnWin();
    }
}
