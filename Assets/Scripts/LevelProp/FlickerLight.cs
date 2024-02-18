using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] private Light flickerLight;
    [SerializeField] private float interval = 1;
    [SerializeField] private float flicker = 0.2f;

    [SerializeField] private float defaultIntensity;
    [SerializeField] private float minIntensity = 0.5f;
    private bool isOn;
    private float counter;
    private float delay;

    // Start is called before the first frame update
    void Start()
    {
        defaultIntensity = flickerLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > delay)
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        isOn = !isOn;

        if (isOn)
        {
            flickerLight.intensity = defaultIntensity;
            delay = Random.Range(0, interval);
        }

        else
        {
            flickerLight.intensity = Random.Range(minIntensity, defaultIntensity);
            delay = Random.Range(0, flicker);
        }

        counter = 0;
    }
}
