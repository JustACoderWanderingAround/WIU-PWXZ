using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartHalf : MonoBehaviour
{
    float speed;

    private GameObject target;

    private Vector3 dir;

    public void Init(GameObject target, float speed)
    {
        this.target = target;
        
        this.speed = speed * 100f; 
    }
    private void Update()
    {
        dir = (target.transform.position - gameObject.transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 1)
            Destroy(gameObject);
    }
}
