using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    private float fadeTime = 8f;
    private MeshRenderer meshRenderer;
    private Color objColor;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //spawnTime = Time.time;
        StartCoroutine(fadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        //Destroy footprint if it is completely transparent
        if (meshRenderer.material.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator fadeOut()
    {
        float counter = 0;
        objColor = meshRenderer.material.color;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / fadeTime);

            meshRenderer.material.color = new Color(objColor.r, objColor.g, objColor.b, alpha);
            yield return null;
        }
    }
}
