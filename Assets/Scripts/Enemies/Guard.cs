using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour, IEventListener
{
    private AINavigation aiNavigation;
    private FieldOfView fov;

    [SerializeField] private Animator animator;

    // Waypoints
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex = 0;
    private Vector3 positionOfInterest = Vector3.zero;

    // Animations
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Sprint = Animator.StringToHash("Sprint");
    public readonly int LookAround = Animator.StringToHash("LookAround");
    public readonly int Stunned = Animator.StringToHash("Stun");

    // for postoffice
    public LISTENER_TYPE listenerType = LISTENER_TYPE.GUARD;

    public enum GuardState
    {
        IDLE,
        PATROL,
        CHASE,
        LOOK_AROUND,
        SEARCH,
        STUNNED
    }
    public GuardState currentState;

    private float timer;

    private void Awake()
    {
        aiNavigation = GetComponent<AINavigation>();
        fov = GetComponent<FieldOfView>();
        listenerType = LISTENER_TYPE.GUARD;
    }

    void Start()
    {
        aiNavigation.InitNavMeshAgent();
        currentState = GuardState.IDLE;
        PostOffice.GetInstance().Subscribe(this.gameObject);
    }

    public void ChangeState(GuardState nextState)
    {
        currentState = nextState;
        timer = 0;

        switch (currentState)
        {
            case GuardState.IDLE:
                animator.CrossFade(Idle, 0.1f);
                break;
            case GuardState.PATROL:
                animator.CrossFade(Walk, 0.1f);
                aiNavigation.SetNavMeshTarget(waypoints[waypointIndex].position, 2f);
                break;
            case GuardState.CHASE:
                aiNavigation.SetNavMeshTarget(fov.target.position, 4f);
                animator.CrossFade(Sprint, 0.1f);
                break;
            case GuardState.LOOK_AROUND:
                animator.CrossFade(LookAround, 0.1f);
                break;
            case GuardState.SEARCH:
                animator.CrossFade(Walk, 0.1f);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 3f);
                break;
            case GuardState.STUNNED:
                animator.CrossFade(Stunned, 0.1f);
                aiNavigation.StopNavigation();
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
                    ChangeState(GuardState.LOOK_AROUND);

                break;
            case GuardState.PATROL:
                if (aiNavigation.OnReachTarget(waypoints[waypointIndex].position))
                {
                    ChangeState(GuardState.IDLE);

                    waypointIndex++;
                    if (waypointIndex > waypoints.Length - 1)
                        waypointIndex = 0;
                }

                break;
            case GuardState.CHASE:
                if (!fov.CheckTargetInLineOfSight(out positionOfInterest, 10000))
                    ChangeState(GuardState.SEARCH);

                aiNavigation.SetNavMeshTarget(positionOfInterest, 3.5f);

                break;
            case GuardState.LOOK_AROUND:
                timer += Time.deltaTime;
                if (timer >= 6f)
                    ChangeState(GuardState.PATROL);

                break;
            case GuardState.SEARCH:
                if (positionOfInterest == Vector3.zero)
                    ChangeState(GuardState.PATROL);

                if (aiNavigation.OnReachTarget(positionOfInterest))
                {
                    positionOfInterest = Vector3.zero;
                    ChangeState(GuardState.LOOK_AROUND);
                }

                break;

            case GuardState.STUNNED:
                timer += Time.deltaTime;
                if (timer >= 5f)
                    ChangeState(GuardState.PATROL);

                break;
            default:
                break;
        }

        if (fov.FindVisibleTargets() && 
            currentState != GuardState.CHASE && 
            currentState != GuardState.STUNNED)
            ChangeState(GuardState.CHASE);
    }

    public void RespondToSound(SoundWPosition sound)
    {
        if (currentState == GuardState.STUNNED)
            return;

        if (sound.soundType == SoundWPosition.SoundType.INTEREST && currentState != GuardState.CHASE)
        {
            positionOfInterest = sound.position;
            ChangeState(GuardState.SEARCH);
        }
        else if (sound.soundType == SoundWPosition.SoundType.DANGER)
        {
            positionOfInterest = transform.forward - (sound.position - transform.position);
            ChangeState(GuardState.SEARCH);
        }
    }

    public LISTENER_TYPE GetListenerType()
    {
        return listenerType;
    }
}