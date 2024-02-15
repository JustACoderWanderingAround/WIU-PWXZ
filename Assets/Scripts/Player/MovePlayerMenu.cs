using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerMenu : MonoBehaviour
{
    private float moveSpeed = 200f;
    [SerializeField] private Transform waypoint;
    //[SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float moveDistance = moveSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * moveDistance);
        if (transform.position.x > 1500.0f)
        {
            transform.position = waypoint.position;

        }
    }
}
