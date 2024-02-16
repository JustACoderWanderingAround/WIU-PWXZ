using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour, IEventListener
{
    private AINavigation aiNavigation;
    private FieldOfView fov;
    private EnemyUIController enemyUIController;

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

    private Coroutine increaseSuspicion = null;

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
        enemyUIController = GetComponent<EnemyUIController>();
    }

    void Start()
    {
        aiNavigation.InitNavMeshAgent();
        currentState = GuardState.IDLE;
        PostOffice.GetInstance().Subscribe(gameObject);
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
                animator.CrossFade(Sprint, 0.1f);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 4f);
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
                if (aiNavigation.OnReachTarget(waypoints[waypointIndex].position, 0.3f))
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

                if (aiNavigation.OnReachTarget(positionOfInterest, 0.3f))
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

        if (fov.FindVisibleTargets(out List<Collider> targets) &&
            currentState != GuardState.CHASE &&
            currentState != GuardState.STUNNED)
        {
            Collider furthestTarget = null;
            float maxDistance = 0f;

            foreach (Collider col in targets)
            {
                if (col.CompareTag("Player"))
                {
                    fov.targetPos = col.transform.position;
                    OnSuspicionIncrease(100f, col.transform.position, GuardState.CHASE);
                    return;
                }
                else
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);

                    if (distance > maxDistance)
                    {
                        furthestTarget = col;
                        maxDistance = distance;
                    }
                }
            }

            if (furthestTarget != null)
            {
                OnSuspicionIncrease(100f, furthestTarget.transform.position, GuardState.SEARCH);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 3f);
            }
        }
    }

    public void RespondToSound(SoundWPosition sound)
    {
        if (currentState == GuardState.STUNNED)
            return;

        if (sound.soundType == SoundWPosition.SoundType.MOVEMENT)
            OnSuspicionIncrease(8f, sound.position, GuardState.CHASE);
        else if (sound.soundType == SoundWPosition.SoundType.IMPORTANT)
            OnSuspicionIncrease(100f, sound.position, GuardState.SEARCH);
    }

    private void OnSuspicionIncrease(float amount, Vector3 position, GuardState nextState)
    {
        if (increaseSuspicion == null)
            increaseSuspicion = StartCoroutine(IncreaseSuspicion(amount, position, nextState));
    }

    private IEnumerator IncreaseSuspicion(float amount, Vector3 position, GuardState nextState)
    {
        if (currentState == nextState)
            yield break;

        enemyUIController.IncrementSuspicion(amount);

        if (enemyUIController.GetSuspicionLevel() >= 100)
        {
            positionOfInterest = position;
            fov.targetPos = positionOfInterest;
            ChangeState(nextState);
        }

        yield return new WaitForSeconds(0.1f);

        increaseSuspicion = null;
    }

    public LISTENER_TYPE GetListenerType()
    {
        return LISTENER_TYPE.GUARD;
    }
}