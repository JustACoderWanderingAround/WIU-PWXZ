using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerMenu : MonoBehaviour
{
    private float moveSpeed = 800f;
    [SerializeField] private Transform waypoint;
    //[SerializeField] private Animator animator;
    private AnimationController animationController;

    // Start is called before the first frame update
    void Start()
    {
        animationController = AnimationController.Instance;
        animationController.ChangeAnimation(animationController.Sprint, 0f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        float moveDistance = moveSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * moveDistance);
        if (transform.position.x > 2000.0f)
        {
            transform.position = waypoint.position;

        }
    }
}
