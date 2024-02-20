using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created by: Tan Xiang Feng Wayne
public class CameraBehaviour : MonoBehaviour
{
    [Header("Player")]
    public LayerMask targetLayer;
    public string targetTag = "Player";
    public float detectionRange = 10f;

    [Header("Camera")]
    public Camera cameraFOV;
    public Transform headTransform;
    public Light cameraSpotlight;

    private Vector3 originalEulerRotation;
    private Color targetColor = Color.white;

    [Header("Rotation (0deg - 360deg)")]
    public float minRotationY;
    public float maxRotationY;
    private float targetRotationY;

    [Header("States")]
    public float idleTime = 3f;
    public float speedMultiplier = 2f;
    public bool isHostile = true;

    private GameObject targetGO;
    private Bounds targetBounds;

    private float cooldown = 0f;

    private System.Action<Vector3> onCaptureActions;

    public void SubscribeOnCapture(System.Action<Vector3> onCaptureAction)
    {
        onCaptureActions += onCaptureAction;
    }

    public void UnsubscribeOnCapture(System.Action<Vector3> onCaptureAction)
    {
        onCaptureActions -= onCaptureAction;
    }

    public enum CameraState
    {
        Camera_Idle,
        Camera_Patrol,
        Camera_Chase, 
    }

    public CameraState currState;

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

    private void FixedUpdate()
    {
        UpdatePhysics();
    }

    public void Initialise()
    {
        //This will be the initial rotation for idle
        originalEulerRotation = headTransform.eulerAngles;
        currState = CameraState.Camera_Idle;
        targetRotationY = minRotationY;

        RenderTexture renderTexture = new RenderTexture(640, 360, 8);
        cameraFOV.targetTexture = renderTexture;
    }

    public void UpdateTransform()
    {
        //Set to Chase state whenever targetTransform is not null
        if (targetGO != null && currState != CameraState.Camera_Chase)
            currState = CameraState.Camera_Chase;
        
        switch(currState)
        {
            case CameraState.Camera_Idle:
                IdleState();
                break;
            case CameraState.Camera_Patrol:
                PatrolState();
                break;
            case CameraState.Camera_Chase:
                ChaseState();
                break;
        }

        //If there is spot ;ight
        if (cameraSpotlight)
        {
            targetColor = targetGO != null ? Color.red : Color.white;
            //Set Spot light Color to the specifc color
            if (cameraSpotlight.color != targetColor)
                cameraSpotlight.color = Color.Lerp(cameraSpotlight.color, targetColor, Time.deltaTime * 10f);
        }
    }

    public void UpdatePhysics()
    {
        //If its not hostile, dont check for detect player
        if (!isHostile)
            return;

        if (targetGO == null)
        {
            //Detect once per physics frame to reduce calculation
            targetGO = DetectPlayer();
            //Detect Player for the first time
            if (targetGO != null)
                onCaptureActions?.Invoke(targetGO.transform.position);
        }
        else if(!IsWithinViewArea(GeometryUtility.CalculateFrustumPlanes(cameraFOV), targetBounds)
                || !CanSee(targetGO))
        {
            //Send last known position
            onCaptureActions?.Invoke(targetGO.transform.position);

            targetGO = null;
            currState = CameraState.Camera_Idle;
        }
    }

    private void IdleState()
    {
        //If the x angle (means they lowered down or went up) is not the original, slowly go back
        if (Mathf.Abs(headTransform.localEulerAngles.x - originalEulerRotation.x) > 1f)
        {
            headTransform.localEulerAngles = Vector3.Lerp(headTransform.localEulerAngles,
                new Vector3(originalEulerRotation.x, headTransform.localEulerAngles.y, 0f), Time.deltaTime);
            return;
        }

        cooldown += Time.deltaTime;
        if (cooldown > idleTime)
        {
            cooldown = 0f;
            currState = CameraState.Camera_Patrol;
            return;
        }    
    }

    private void PatrolState()
    {
        //See if the approximaty is < 1f;
        //Means reached target position
        if (Mathf.Abs(headTransform.localEulerAngles.y - (targetRotationY + 180f)) < 10f)
        {
            targetRotationY = targetRotationY == minRotationY ? maxRotationY : minRotationY;
            currState = CameraState.Camera_Idle;
            return;
        }

        //Lerp to the target rotation
        //Create a copy
        Vector3 targetEulerAngles = new Vector3(headTransform.localEulerAngles.x, headTransform.localEulerAngles.y, headTransform.localEulerAngles.z);
        //Set y to target
        targetEulerAngles.y = Mathf.Lerp(targetEulerAngles.y, targetRotationY + 180f, Time.deltaTime);
        headTransform.localEulerAngles = targetEulerAngles;
    }

    private void ChaseState()
    {
        //If player is not in view or in range anymore, return to idle state
        if (targetGO == null)
        {
            currState = CameraState.Camera_Idle;
            return;
        }

        //Calculate target direction
        Vector3 dir = (targetGO.transform.position - headTransform.position).normalized;

        //Lerp the camera forward to the dir
        headTransform.forward = Vector3.Lerp(headTransform.forward, dir, Time.deltaTime * speedMultiplier);
    }

    private GameObject DetectPlayer()
    {
        //Check if Player is within the detection range;
        //Overlap Sphere to check for Colliders
        Collider[] colliders = Physics.OverlapSphere(headTransform.position, detectionRange, targetLayer);

        //Return if there is no collision
        if (colliders.Length <= 0)
            return null;

        //If there is a collision -> possible Player inside
        //Create Frustum Based On Camera View
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraFOV);

        //By Right there should only be one collider
        foreach (Collider collided in colliders)
        {
            //If it is within view area and can see
            if (IsWithinViewArea(planes, collided.bounds) && CanSee(collided.gameObject) && collided.CompareTag(targetTag))
            {
                targetBounds = collided.bounds;
                //Return the first one detected
                return collided.gameObject;
            }
        }

        //Is not within view area or can see
        return null;
    }

    private bool IsWithinViewArea(Plane[] planes, Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    private bool CanSee(GameObject objectToCheck)
    {
        RaycastHit hit;
        //See if it hits the player or not
        //If it doesnt, means something is blocking it
        Vector3 direction = (objectToCheck.transform.position - cameraFOV.gameObject.transform.position).normalized;
        if (Physics.Raycast(cameraFOV.gameObject.transform.position, direction, out hit, detectionRange))
        {
            if (hit.transform.gameObject == objectToCheck)
            {
                return true;
            }
        }
        return false;
    }
}
