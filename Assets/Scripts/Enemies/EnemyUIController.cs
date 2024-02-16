using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private Slider suspicionMeter;
    [SerializeField] private float suspicionDecayRate;

    [SerializeField] private Image suspicionSymbol;
    [SerializeField] private Sprite questionMark;
    [SerializeField] private Sprite exclamationMark;

    private float suspicionAmount = 0;

    private Coroutine suspicionDecay;
    private Coroutine incrementSuspicion;

    public void IncrementSuspicion(float amount)
    {
        suspicionAmount += amount;

        if (suspicionDecay != null)
        {
            StopCoroutine(suspicionDecay);
            suspicionDecay = null;
        }

        if (incrementSuspicion != null)
            StopCoroutine(incrementSuspicion);

        incrementSuspicion = StartCoroutine(OnIncrementSuspicion());
    }

    private IEnumerator OnIncrementSuspicion()
    {
        yield return new WaitForSeconds(5f);

        suspicionDecay = StartCoroutine(DecaySuspicion());
    }

    public void StartDecaySuspicion()
    {
        suspicionAmount -= 100;
        suspicionDecay = StartCoroutine(DecaySuspicion());
    }

    private IEnumerator DecaySuspicion()
    {
        while (suspicionMeter.value > 0)
        {
            suspicionAmount -= suspicionDecayRate;
            yield return null;
        }
    }

    public float GetSuspicionLevel()
    {
        return suspicionAmount;
    }

    private void Update()
    {
        suspicionMeter.value = suspicionAmount;

        if (suspicionAmount >= 100)
        {
            suspicionMeter.gameObject.SetActive(true);
            suspicionSymbol.sprite = exclamationMark;
        }
        else if (suspicionAmount > 0 & suspicionAmount < 100)
        {
            suspicionMeter.gameObject.SetActive(true);
            suspicionSymbol.sprite = questionMark;
        }
        else
            suspicionMeter.gameObject.SetActive(false);
    }
}