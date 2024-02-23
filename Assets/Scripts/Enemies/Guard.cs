using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour, IEventListener
{
    private AINavigation aiNavigation;
    private FieldOfView fov;
    private EnemyUIController enemyUIController;

    [SerializeField] private Animator animator;
    [SerializeField] private Light spotLight;
    private Color lightColor = Color.white;

    // Waypoints
    [SerializeField] private Transform[] waypoints;
    public int WaypointIndex { get => waypointIndex; set => waypointIndex = value; }
    private int waypointIndex = 0;
    private Vector3 positionOfInterest = Vector3.zero;

    // Animations
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Sprint = Animator.StringToHash("Sprint");
    public readonly int LookAround = Animator.StringToHash("LookAround");
    public readonly int Stunned = Animator.StringToHash("Stun");

    private Coroutine increaseSuspicion = null;
    private bool caughtPlayer = false;
    private IInteractable interactable;

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

        CameraBehaviour[] cameras = FindObjectsOfType<CameraBehaviour>();
        foreach (CameraBehaviour camera in cameras)
            camera.SubscribeOnCapture((pos) => OnSuspicionIncrease(100f, pos, GuardState.SEARCH));
    }

    void Start()
    {
        aiNavigation.InitNavMeshAgent();
        currentState = GuardState.IDLE;
        PostOffice.GetInstance().Subscribe(gameObject);
    }

    private void ChangeColor(Color newColor)
    {
        StartCoroutine(DoChangeColor(newColor));
    }

    private IEnumerator DoChangeColor(Color newColor)
    {
        Color startColor = spotLight.color;
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / 0.5f;
            spotLight.color = Color.Lerp(startColor, newColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spotLight.color = newColor;
        lightColor = newColor;
    }

    public void ChangeState(GuardState nextState)
    {
        currentState = nextState;
        timer = 0;
        animator.StopPlayback();

        switch (currentState)
        {
            case GuardState.IDLE:
                animator.CrossFade(Idle, 0.05f);
                break;
            case GuardState.PATROL:
                aiNavigation.ResumeNavigation();
                ChangeColor(Color.white);
                animator.CrossFade(Walk, 0.05f);
                aiNavigation.SetNavMeshTarget(waypoints[waypointIndex].position, 2f);
                break;
            case GuardState.CHASE:
                aiNavigation.ResumeNavigation();
                ChangeColor(Color.red);
                animator.CrossFade(Sprint, 0.05f);
                break;
            case GuardState.LOOK_AROUND:
                animator.CrossFade(LookAround, 0.05f);
                break;
            case GuardState.SEARCH:
                aiNavigation.ResumeNavigation();
                ChangeColor(Color.red);
                animator.CrossFade(Walk, 0.05f);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 3f);
                break;
            case GuardState.STUNNED:
                ChangeColor(Color.yellow);
                animator.CrossFade(Stunned, 0.05f);
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

                // Check if interactable has been interacted
                if (fov.CheckInteractableInteracted(out Vector3 targetPos, out IInteractable targetInteractable))
                {
                    interactable = targetInteractable;
                    OnSuspicionIncrease(100f, targetPos, GuardState.SEARCH);
                }

                break;
            case GuardState.CHASE:
                timer += Time.deltaTime;
                if (!fov.CheckTargetInLineOfSight(out positionOfInterest, 10000) && timer >= 3f)
                {
                    timer = 0f;
                    enemyUIController.StartDecaySuspicion();
                    aiNavigation.StopNavigation();
                    ChangeState(GuardState.SEARCH);
                }

                if (PlayerController.Instance == null)
                    return;

                aiNavigation.SetNavMeshTarget(PlayerController.Instance.transform.position, 3f);

                break;
            case GuardState.LOOK_AROUND:
                timer += Time.deltaTime;
                if (timer >= 6f)
                {
                    ChangeState(GuardState.PATROL);
                    if (interactable != null)
                    {
                        interactable.OnInteract();
                        interactable = null;    
                    }
                }

                break;
            case GuardState.SEARCH:
                if (positionOfInterest == Vector3.zero)
                    ChangeState(GuardState.PATROL);

                if (aiNavigation.OnReachTarget(positionOfInterest, 3))
                {
                    enemyUIController.StartDecaySuspicion();
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

        // Check if player is in visible range
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

            if (furthestTarget != null &&
                currentState != GuardState.CHASE &&
                currentState != GuardState.STUNNED)
            {
                OnSuspicionIncrease(100f, furthestTarget.transform.position, GuardState.SEARCH);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 3f);
            }
        }

        // Check if guard catch player
        if (PlayerController.Instance == null)
            return;

        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= 1f && !caughtPlayer)
        {
            caughtPlayer = true;
            Debug.Log(gameObject + " Called Load");
            CheckpointController.Instance.Load();
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
        if (currentState == GuardState.STUNNED)
            return;

        if (increaseSuspicion == null)
            increaseSuspicion = StartCoroutine(IncreaseSuspicion(amount, position, nextState));
    }

    private IEnumerator IncreaseSuspicion(float amount, Vector3 position, GuardState nextState)
    {
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