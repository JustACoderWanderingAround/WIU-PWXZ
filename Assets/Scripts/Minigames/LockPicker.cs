using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockPicker : MiniGame
{
    [SerializeField] private Slider mainKeySlider;
    [SerializeField] private GameObject handle;
    [SerializeField] float mainIncreaseRate = 1.2f;

    public List<Slider> sliderList;

    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text timerText;

    float mainDecreaseRate;

    int unlockedCount;

    private void OnEnable()
    {
        timer = maxTimer;
        EnableMinigame();
        unlockedCount = 0;
        mainDecreaseRate = mainIncreaseRate * 0.2f;
        foreach (Slider slid in sliderList)
        {
            slid.value = (Random.Range(0, 0.25f));
        }
    }
        
    private void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = System.MathF.Truncate(timer).ToString();
        if (Input.GetMouseButton(0))
        {
            mainKeySlider.value += mainIncreaseRate * Time.deltaTime;
        }
        else
        {
            mainKeySlider.value -= mainDecreaseRate * Time.deltaTime;
        }
        string newString = "";
        unlockedCount = 0;
        int counter = 1;
        foreach (Slider slid in sliderList)
        {
            if (Mathf.Abs(handle.transform.position.x - slid.gameObject.transform.position.x) < 115)
            {


                if (Input.GetMouseButton(1))
                {
                    if (slid.value < 1)
                        slid.value += mainIncreaseRate * 0.5f * Time.deltaTime;
                }
            }
            if (slid.value == 1)
            {
                unlockedCount++;
            }
            newString += counter.ToString() + ": " + System.MathF.Truncate(slid.value * 100).ToString() + "% ";
            counter++;
        }
        mainText.text = "Unlock %s:" + newString;
        if (unlockedCount >= sliderList.Count)
        {
            OnWin();
        }
        else
        {
            if (timer < 0)
            {
                OnLose();
            }
        }
    }
}
