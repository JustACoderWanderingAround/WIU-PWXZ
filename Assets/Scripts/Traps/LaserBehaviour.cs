using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour, IEventListener
{
    private static System.Action<Collider> onHit;

    public static void OnSubscribeHit(System.Action<Collider> _onHit)
    {
        onHit += _onHit;
    }

    public static void OnUnsubscribeHit(System.Action<Collider> _onHit)
    {
        onHit -= _onHit;
    }

    [Header("Physics")]
    [Range(0f, float.MaxValue)]
    public float maxDistance = 1000f;
    public LayerMask layerAffected;
    public LayerMask targetLayer;
    public bool bRespondToSound = false;
    public float timeToDeactivate = 10f;

    private float activatedTime = 0f;
    private float rayDistance = 0f;

    [Header("Animation")]
    public float laserTransitionTimeScale = 100f;
    public float positionTransitionTimeScale = 1f;

    [Header("Waypoints")]
    public int wayIndex = 0;
    public List<Transform> waypoints;
    public float pauseTime = 5f;
    public float distanceOffset = 0.001f;
    public float rotationOffset = 1f;

    private float pausedTime = 0f;

    private LineRenderer lineRenderer = null;

    private bool bIsActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
    }

    public void Initialise()
    {
        //If there is no line renderer component, add a line renderer component
        lineRenderer = GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();

        //If target layer is not part of layer affected
        if ((layerAffected & targetLayer) <= 0)
        {
            //Add in target layer as part of the layer affected
            layerAffected |= targetLayer;
            //Add warning
            Debug.LogWarning(gameObject + ": Layer Affected DOES NOT contain Target Layer. " +
                "Added Target Layer to affected layer");
        }

        pausedTime = pauseTime;
    }

    public void UpdateTransform()
    {
        if (!bRespondToSound || bIsActivated)
        {
            //If respond to sound, then check for it
            activatedTime = bRespondToSound ? Time.deltaTime : 0f;
            if (bRespondToSound && activatedTime > timeToDeactivate)
            {
                bIsActivated = false;
                rayDistance = 0f;
            }

            HandleWaypoints();
            HandleRayCast();
        }
    }

    private void HandleWaypoints()
    {
        if (waypoints.Count <= 0)
            return;

        //Set cooldown
        if (pausedTime < pauseTime)
        {
            pausedTime += Time.deltaTime;
            return;
        }

        //If position is correct, do rotation
        if (Vector3.Distance(transform.position, waypoints[wayIndex].position) <= distanceOffset)
        {
            if (Quaternion.Angle(transform.rotation, waypoints[wayIndex].rotation) <= rotationOffset)
            {
                //Set to next waypoint
                wayIndex++;
                wayIndex %= waypoints.Count;
                //Wait for next frame to start updating
                pausedTime = 0f;
                return;
            }
            //Lerp the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, waypoints[wayIndex].rotation, 
                Time.deltaTime * positionTransitionTimeScale);
            return;
        }
        //Lerp the position
        transform.position = Vector3.Lerp(transform.position, waypoints[wayIndex].position,
                Time.deltaTime * positionTransitionTimeScale);
    }

    private void HandleRayCast()
    {
        //Rotate Vector3.up based on rotation
        Vector3 shootRotation = transform.rotation * Vector3.up;

        //Setup line Renderer
        Vector3[] linePosition = new Vector3[2];
        linePosition[0] = transform.position;

        //When havent extent to max
        if (rayDistance < maxDistance)
        {
            //Add on
            rayDistance += laserTransitionTimeScale * Time.deltaTime;
            rayDistance = Mathf.Clamp(rayDistance, 0f, maxDistance);
        }

        //Check for RaycastHit
        RaycastHit hit;
        if (Physics.Raycast(transform.position, shootRotation, out hit, rayDistance, layerAffected))
        {
            linePosition[1] = hit.point;

            //If it hits target, Invoke
            if ((targetLayer & (1 << hit.collider.gameObject.layer)) > 0)
            {
                onHit?.Invoke(hit.collider);
            }
        }
        else
        {
            linePosition[1] = (shootRotation * rayDistance) + transform.position;
        }

        //Set the positions
        lineRenderer.SetPositions(linePosition);
    }

    public void RespondToSound(SoundWPosition sound)
    {
        if (sound.soundType == SoundWPosition.SoundType.MOVEMENT)
        {
            bIsActivated = true;
            activatedTime = 0f;
        }
    }

    public LISTENER_TYPE GetListenerType()
    {
        return LISTENER_TYPE.GUARD;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (transform.rotation * Vector3.up) * maxDistance + transform.position);
    }
}
