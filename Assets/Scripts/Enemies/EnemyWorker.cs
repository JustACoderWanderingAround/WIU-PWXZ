using ConcreteMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adopted version of guard class made for enemy worker AI
/// </summary>
public class EnemyWorker : MonoBehaviour, IEventListener
{
    // vars taken from guard class
    private AINavigation aiNavigation;
    private FieldOfView fov;

    [SerializeField] private Animator animator;

    // Waypoints
    [SerializeField] private Transform[] waypoints;
    private int waypointIndex = 0;
    private Vector3 positionOfInterest = Vector3.zero;
    private float timer;

    LISTENER_TYPE listenerType;

    // Animations
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Sprint = Animator.StringToHash("Sprint");
    public readonly int Alert = Animator.StringToHash("Alert");
    // Idle Animations
    public readonly int Sad = Animator.StringToHash("Sad");
    public readonly int Stretch = Animator.StringToHash("Stretch");
    public readonly int LookAround = Animator.StringToHash("LookAround");

    public enum WorkerState
    {
        IDLE,
        MOVE, 
        RETREAT,
        SEARCH,
        ALERT,
        LOOKAROUND
    }

    public WorkerState currentState;
    public enum WorkerIdleState
    {
        STAND,
        SAD,
        STRETCH,
        LOOK
    }
    private void Awake()
    {
        aiNavigation = GetComponent<AINavigation>();
        fov = GetComponent<FieldOfView>();
        listenerType = LISTENER_TYPE.GUARD;
    }

    void Start()
    {
        aiNavigation.InitNavMeshAgent();
        currentState = WorkerState.IDLE;
        PostOffice.GetInstance().Subscribe(this.gameObject);
    }
    private void ChangeState(WorkerState nextState)
    {
        
        currentState = nextState;

        switch (currentState)
        {
            case WorkerState.IDLE:
                
                SetIdle();
                aiNavigation.StopNavigation();
                break;
            case WorkerState.MOVE:
                
                animator.CrossFade(Walk, 0.1f);
                aiNavigation.SetNavMeshTarget(waypoints[waypointIndex].position, 2f);
                break;
            case WorkerState.RETREAT:
                
                animator.CrossFade(Sprint, 0.1f);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 2f);
                break;
            case WorkerState.ALERT:
                
                aiNavigation.StopNavigation();
                animator.CrossFade(Alert, 0.2f);
                MessagePlayerHere alertMessage = new MessagePlayerHere(this, SoundWPosition.SoundType.IMPORTANT, positionOfInterest, 50f);
                PostOffice.GetInstance().SendToPostOffice(alertMessage);
                //
                break;
            case WorkerState.SEARCH:
                
                animator.CrossFade(Walk, 0.1f);
                aiNavigation.SetNavMeshTarget(positionOfInterest, 3f);
                break;
            case WorkerState.LOOKAROUND:
              
                aiNavigation.StopNavigation();
                animator.CrossFade(LookAround, 0.1f);
                break;
        }
        
    }
    public void RespondToSound(SoundWPosition sound)
    {

        if (sound.soundType == SoundWPosition.SoundType.MOVEMENT && (currentState != WorkerState.SEARCH && currentState != WorkerState.ALERT && currentState != WorkerState.LOOKAROUND))
        {
          
            positionOfInterest = sound.position;
            ChangeState(WorkerState.SEARCH);
        }
        else if (sound.soundType == SoundWPosition.SoundType.DANGER && currentState != WorkerState.RETREAT)
        {
            positionOfInterest = transform.forward - (sound.position - transform.position);
            aiNavigation.SetNavMeshTarget(positionOfInterest, 5f);
            ChangeState(WorkerState.RETREAT);
        }
    }
    void SetIdle()
    {
        int random = Random.Range(0, 100);
        if (random <= 40)
        {
            animator.CrossFade(Idle, 0.1f);
        }
        else if (random > 40 && random <= 60)
        {
            animator.CrossFade(Sad, 0.1f);
        }
        else if (random > 60 && random <= 80) {
            animator.CrossFade(Stretch, 0.1f);
        }
        else
        {
            animator.CrossFade(LookAround, 0.1f);
        }
    }
    void Update()
    {
        if (waypointIndex > waypoints.Length - 1)
            waypointIndex = 0;
        switch (currentState)
        {
            case WorkerState.IDLE:
                timer += Time.deltaTime;
                if (timer >= 5f)
                {
                    timer = 0;
                    ChangeState(WorkerState.MOVE);
                }
                break;
            case WorkerState.MOVE:
                if (aiNavigation.OnReachTarget(waypoints[waypointIndex].position, 0.3f))
                {
                    waypointIndex++;
                    if (waypointIndex > waypoints.Length - 1)
                        waypointIndex = 0;
                    ChangeState(WorkerState.IDLE);
                    aiNavigation.SetNavMeshTarget(waypoints[waypointIndex].position, 2f);
                }
                break;
            case WorkerState.RETREAT:
                timer += Time.deltaTime;
                if (timer >= 5f)
                {
                    ChangeState(WorkerState.IDLE);
                }
                break;
            case WorkerState.ALERT:
                if (!fov.FindVisibleTargets())
                {
                  
                    ChangeState(WorkerState.LOOKAROUND);
                }
                break;
            case WorkerState.SEARCH:
                if (Vector3.Distance(gameObject.transform.position, positionOfInterest) < 3)
                {
                  
                    ChangeState(WorkerState.LOOKAROUND);
                }
                break;
            case WorkerState.LOOKAROUND:
                if (fov.FindVisibleTargets())
                {
                  
                    ChangeState(WorkerState.ALERT);
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer > 5.0f)
                    {
                        timer = 0;
                        ChangeState(WorkerState.IDLE);
                    }
                }
                break;
        }
        if (fov.FindVisibleTargets() && currentState != WorkerState.ALERT)
            ChangeState(WorkerState.ALERT);
    }

    public LISTENER_TYPE GetListenerType()
    {
        return listenerType;
    }
}
