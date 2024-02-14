using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private AINavigation aiNavigation;
    private FieldOfView fov;
    [SerializeField] private Animator animator;

    // Animations
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Sprint = Animator.StringToHash("Sprint");
    public readonly int LookAround = Animator.StringToHash("LookAround");

    public enum GuardState
    {
        IDLE,
        PATROL,
        CHASE,
        LOOK_AROUND
    }
    public GuardState currentState;

    private float timer;

    private void Awake()
    {
        aiNavigation = GetComponent<AINavigation>();
        fov = GetComponent<FieldOfView>();
    }

    void Start()
    {
        aiNavigation.InitNavMeshAgent();
        currentState = GuardState.IDLE;
    }

    private void ChangeState(GuardState nextState)
    {
        currentState = nextState;

        // Update animation
        switch(currentState)
        {
            case GuardState.IDLE:
                animator.CrossFade(Idle, 0.1f);
                break;
            case GuardState.PATROL:
                animator.CrossFade(Walk, 0.1f);
                break;
            case GuardState.CHASE:
                animator.CrossFade(Sprint, 0.1f);
                break;
            case GuardState.LOOK_AROUND:
                animator.CrossFade(LookAround, 0.1f);
                break;
        }
    }

    void Update()
    {
        // Update state machine
        switch (currentState)
        {
            case GuardState.IDLE:
                timer += Time.deltaTime;
                if (timer >= 2f)
                {
                    ChangeState(GuardState.LOOK_AROUND);
                    timer = 0;
                }

                break;
            case GuardState.PATROL:
                if (aiNavigation.OnReachWaypoint())
                    ChangeState(GuardState.IDLE);
                aiNavigation.UpdateNavMeshAgent(2f);

                break;
            case GuardState.CHASE:
                aiNavigation.UpdateNavMeshAgent(3f);

                break;
            case GuardState.LOOK_AROUND:
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                {
                    ChangeState(GuardState.PATROL);
                    timer = 0;
                }

                break;
            default:
                break;
        }

        fov.FindVisibleTargets();
    }
}