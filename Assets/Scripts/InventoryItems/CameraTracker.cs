using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create by: Tan Xiang Feng Wayne
public class CameraTracker : MonoBehaviour, IInventoryItem
{
    public Sprite cameraInventorySprite;
    public float offsetY = 0.5f;

    [Header("Texture2D")]
    public Texture2D defaultTexture;
    public Material targetMaterial;

    private List<CameraBehaviour> cameras;

    private bool isEnabled = false;
    private int textureIndex = 0;
    private int prevIndex = 0;

    private Vector3 targetPosition = Vector3.zero;

    private string loadedSceneName = string.Empty;

    private Sprite noCameraSprite = null;
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetItemDescription()
    {
        return "This Item is used track the Camera View Nearby.";
    }

    public Sprite GetItemDisplaySprite()
    {
        return cameraInventorySprite;
    }

    public Action GetItemEffect()
    {
        return OnUseItem;
    }

    public void OnUseItem()
    {
        //New Scene: Camera Destroyed
        if (loadedSceneName != SceneManagement.Instance.GetActiveSceneName())
            Initialise();

        //Set the opposite
        targetMaterial.SetTexture("_Texture2D", isEnabled ? defaultTexture : (cameras.Count != 0 ? cameras[textureIndex].cameraFOV.targetTexture : noCameraSprite.texture));
        targetPosition = new Vector3(0f, isEnabled ? 0f : offsetY, 0f);

        isEnabled = !isEnabled;
        if (isEnabled)
        {
            AudioManager.Instance.Play("TurnOn");
            AudioManager.Instance.Play("Static");
        }
        else
        {
            AudioManager.Instance.Play("TurnOff");
            AudioManager.Instance.Stop("Static");
        }
    }

    public bool GetItemIsConsumable()
    {
        return false;
    }

    public bool GetItemIsStackable()
    {
        return false;
    }

    public string GetItemName()
    {
        return "Tablet";
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    private void OnDisable()
    {
        if (isEnabled)
        {
            AudioManager.Instance.Play("TurnOff");
            AudioManager.Instance.Stop("Static");
        }

        isEnabled = false;
        targetMaterial.SetTexture("_Texture2D", defaultTexture);
    }

    public void Initialise()
    {
        //Init all the camera in the scene
        cameras = new List<CameraBehaviour>(FindObjectsOfType<CameraBehaviour>());
        textureIndex = prevIndex = 0;
        loadedSceneName = SceneManagement.Instance.GetActiveSceneName();
        if (noCameraSprite == null)
            noCameraSprite = Resources.Load<Sprite>("Sprites/nocamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            HandleKeyInput();
            UpdateTransform();
        }
        //Slowly change the position and make sure the parent is something
        if (targetPosition != transform.localPosition && (transform.parent?.name.Contains("HoldPoint") ?? false))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 5f);
        }
    }

    public void UpdateTransform()
    {
        //Set the texture
        if (textureIndex != prevIndex)
        {
            prevIndex = textureIndex;
            targetMaterial.SetTexture("_Texture2D", cameras[textureIndex].cameraFOV.targetTexture);
        }
    }

    public void HandleKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            textureIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            textureIndex++;
        }

        if (cameras.Count == 0)
            return;

        textureIndex = textureIndex % cameras.Count;
        if (textureIndex < 0)
        {
            textureIndex = cameras.Count - 1;
        }
    }

    public bool GetFollowHoldPoint()
    {
        return false;
    }
}
