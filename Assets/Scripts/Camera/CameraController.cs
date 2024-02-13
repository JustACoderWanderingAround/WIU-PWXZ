using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    public Camera firstPersonCamera;

    public float mouseSensitivity = 2.0f;
    private Vector2 _mousePosition = Vector2.zero;

    //This Region Should ONLY be used when InputController is Not Done
    #region DebugOnly

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        ReadMouseAxisCommand(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        UpdateTransform();
    }
    #endregion

    public void Initialise()
    {
        //Locks the Canvas to the Cursor 
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReadMouseAxisCommand(float MouseX, float MouseY)
    {
        _mousePosition.x += MouseX * mouseSensitivity;
        _mousePosition.y -= MouseY * mouseSensitivity;
    }

    public void UpdateTransform()
    {
        HandleCheck();
        UpdateCamera();
    }

    private void HandleCheck()
    {
        //This is to prevent Camera Flipping
        _mousePosition.y = Mathf.Clamp(_mousePosition.y, -90f, 90f);
    }

    private void UpdateCamera()
    {
        firstPersonCamera.transform.localRotation = Quaternion.Euler(_mousePosition.y, _mousePosition.x, 0);
    }
}
