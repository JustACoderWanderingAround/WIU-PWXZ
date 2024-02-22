using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

//Created by: Tan Xiang Feng Wayne
public class CameraCapture : MonoBehaviour
{ 
    public static string constantFolderPath { get; private set; }

    public Camera captureCamera;
    public LayerMask itemLayer;

    [Header("Write To Files")]
    public string folderPath = "Screenshots";
    public string fileName = "CaptureImage";
    public bool ifExistIncrement = false;

    public static string FileName { get; private set; }

    //Private Variables that only to be initialised within the Scipt
    private System.Action<GameObject[]> onCaptureActions;
    private List<Renderer> allRenderers = new List<Renderer>();

    private Coroutine captureHandler = null;
    private Coroutine identifyHandler = null;

    public PhotoAlbum photoAlbum;

    [SerializeField]
    private bool debugAutoSave = false;

    [Header("Effects")]
    public float bloomIntensity = 1000f;
    public float timeToResolve = 0.5f;

    private string sceneName = string.Empty;
    public Volume volumeManager = null;
    private Coroutine effectCoroutine = null;

    #region DebugOnly
    // Start is called before the first frame update
    void Start()
    {
        Initialise();
        //onCaptureActions += Test;
        UpdateRendererList();
    }

    private void Test(GameObject[] go)
    {
        foreach(GameObject g in go)
        {
            Renderer[] rs = g.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in rs)
            {
                r.material.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    CaptureScreen();
        //}
    }
    #endregion

    public void Initialise()
    {
        constantFolderPath = System.IO.Path.Combine(Application.persistentDataPath, folderPath);
        //Debug.Log("Created ConstantFolderPath: " + constantFolderPath);
        //Create folder path if such folder does not exists
        if (!System.IO.Directory.Exists(constantFolderPath))
        {
            System.IO.Directory.CreateDirectory(constantFolderPath);
        }

        if (captureCamera == null)
        {
            captureCamera = Camera.main;
        }

        FileName = fileName;
        volumeManager.weight = 0f;
    }

    //Subscribe to listener
    public void SubscribeOnCapture(System.Action<GameObject[]> onCaptureAction)
    {
        onCaptureActions += onCaptureAction;
    }
    //Unsubscribe from listener
    public void UnsubscribeOnCapture(System.Action<GameObject[]> onCaptureAction)
    {
        onCaptureActions -= onCaptureAction;
    }

    public void CaptureScreen()
    {
        if (captureHandler != null || identifyHandler != null)
        {
            Debug.Log("Please wait for the image to done Rendering and/or Identifying Before capturing again");
            return;
        }
        //Stop previous effect
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);

        effectCoroutine = StartCoroutine(CameraFlickEffect());
        captureHandler = StartCoroutine(CaptureScreenshotAtEndOfFrame());
        identifyHandler = StartCoroutine(IdentifyGameObject());
    }

    private IEnumerator CameraFlickEffect()
    {
        // While capturing Image wait
        while (captureHandler != null)
            yield return null;

        AudioManager.Instance.Play("CameraFlick");
        volumeManager.weight = 1f;

        float time = 0f;
        float multiplier = 1 / timeToResolve;
        while (time < timeToResolve)
        {
            time += Time.deltaTime;
            volumeManager.weight = 1f - (time * multiplier);
            yield return null;
        }

        volumeManager.weight = 0f;

        effectCoroutine = null;
    }

    private IEnumerator IdentifyGameObject()
    {
        //Create Frustum Based On Camera View
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(captureCamera);

        //Make a copy of the list to do all the checkings
        Dictionary<GameObject, Bounds> gameobjectBounds = new Dictionary<GameObject, Bounds>();
        List<GameObject> gameObjects = new List<GameObject>();

        //!!! COMMENTED OUT CAUSE NOT STABLE !!!
        //Populate the list if there is the renderer is active
        //allRenderers.ForEach((r) =>
        //{
        //    //Check if it is active
        //    if (r?.gameObject?.activeInHierarchy ?? false)
        //    {
        //        gameobjectBounds.Add(r.gameObject, r.bounds);
        //    }
        //});

        //Overlap Sphere to check for Colliders
        Collider[] colliders = Physics.OverlapSphere(captureCamera.gameObject.transform.position, captureCamera.farClipPlane, itemLayer);
        if (colliders != null)
        {
            foreach (Collider col in colliders)
            {
                //Check if duplicate key
                if (gameobjectBounds.ContainsKey(col.gameObject))
                    continue;

                gameobjectBounds.Add(col.gameObject, col.bounds);
            }
        }

        //Check for each Pair with their bounds
        foreach (KeyValuePair<GameObject, Bounds> pair in gameobjectBounds)
        {
            if (IsWithinViewArea(planes, pair.Value) && CanSee(pair.Key))
            {
                //Add to the list that is going to be invoked
                gameObjects.Add(pair.Key);
            }
        }

        Debug.Log("IdentifyGameObject: Waiting for Image to render finish before Invoke");

        //Wait for it to save, so it could save the image of the before affected (for eg. material change or gameobject missing)
        while (captureHandler != null)
            yield return null;

        Debug.Log("IdentifyGameObject: Invoking onCaptureActions");

        //Invoke Listener
        onCaptureActions.Invoke(gameObjects.ToArray());

        identifyHandler = null;

        yield break;
    }

    private IEnumerator CaptureScreenshotAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        //Create A Render Texture for the Camera to burn into a 16bits texture
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);

        //Set target texture to the created render texture
        captureCamera.targetTexture = screenTexture;
        //Set Active texture 
        RenderTexture.active = screenTexture;
        //Burn into the texture
        captureCamera.Render();
        //Generate a new Texture2D that can be saved into a png
        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        //Copy from the current Render Target, etc ScreenTexture
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //Once done, set into null
        RenderTexture.active = null;
        //Encode it so it can be saved
        byte[] byteArray = renderedTexture.EncodeToPNG();

        screenTexture.Release();

        //Add Image to the photo album to not reload (System.IO.Read takes long time)
        photoAlbum?.AddImage(byteArray);

        captureCamera.targetTexture = null;

        //Internal Save Image
        if (debugAutoSave)
        { 
            //Read necessary Data from JSON
            string JSONPath = System.IO.Path.Combine(constantFolderPath, $"{fileName}Data.json");
            //Check if such JSON FIle exists
            if (!System.IO.File.Exists(JSONPath))
            {
                System.IO.FileStream fs = new System.IO.FileInfo(JSONPath).Create();
                fs.Close();
            }

            string JSONText = System.IO.File.ReadAllText(JSONPath);
            //Overwrite Data and Create One if it is not initialised Properly
            ImageFolderData data = JsonUtility.FromJson<ImageFolderData>(JSONText) ?? new ImageFolderData();

            //BackTrack by one to allow for increase value in do.while loop (if incremental)
            int incremental = ifExistIncrement ? data.imageCount - 1 : -1;
            string capturePath;
            do
            {
                incremental++;
                capturePath = System.IO.Path.Combine(constantFolderPath, fileName + incremental + ".png");
            }
            while (System.IO.File.Exists(capturePath) && ifExistIncrement);

            //Update JSON
            data.imagePrefix = fileName;
            data.imageCount = data.imageCount < (incremental + 1) ? incremental + 1 : data.imageCount;
            data.lastSavedAt = System.DateTime.Now.ToString();

            //Convert back to string
            JSONText = JsonUtility.ToJson(data);

            //Write to the path
            System.IO.File.WriteAllBytes(capturePath, byteArray);
            System.IO.File.WriteAllText(JSONPath, JSONText);

            Debug.Log($"File {fileName} has been saved in {capturePath}");
            Debug.Log($"JSONFIle {JSONPath} has been Updated! : {JSONText}");
        }

        captureHandler = null;
    }
    /// <summary>
    /// This function updates the lists of renderer in the scene without a collider component
    /// This Function Should ONLY be called once at every Scene Init
    /// (Since FindObjectsOfType is very expensicve)
    /// should a scene with no renderer that is without collider need to be checked
    /// this function SHOULD not be called
    /// </summary>
    public void UpdateRendererList()
    {
        allRenderers.Clear();
        //Find inactive Gameobjects as well
        Renderer[] rs = FindObjectsOfType<Renderer>(true);
        foreach (Renderer r in rs)
        {
            Collider c = r.GetComponent<Collider>();
            //Check if There is Collider Component (if yes, then don't add and we will find them using Overlap Sphere Later On)
            if ((!c || !c.enabled) && ((itemLayer & (1 << r.gameObject.layer)) > 0))
            {
                allRenderers.Add(r);
            }
        }

        ////Debug
        //Debug.Log("-----[UpdateRenderList] : Renderers without Collider Component Activated-----");
        //if (allRenderers.Count > 0)
        //    Debug.LogWarning("IMPORTANT NOTE: If an object is hidden behind an object with no Collider Component, it will not be detected properly. " +
        //        "Please Do Ensure that Collider is enabled if another object is to meant to be hidden under a certain angle");
        //allRenderers.ForEach((r) => Debug.Log("     >" + r));
        //Debug.Log("-----[UpdateRenderList] : End Print-----");
    }

    private bool IsWithinViewArea(Plane[] planes, Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    private bool CanSee(GameObject objectToCheck)
    {
        RaycastHit hit;
        Vector3 direction = (objectToCheck.transform.position - captureCamera.transform.position).normalized;
        if (Physics.Raycast(captureCamera.transform.position, direction, out hit, captureCamera.farClipPlane, itemLayer))
        {
            if (hit.transform.gameObject == objectToCheck)
            {
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class ImageFolderData
{
    public string imagePrefix = "";
    public int imageCount = 0;
    public string lastSavedAt = System.DateTime.MinValue.ToString();

    public ImageFolderData(string name, int count)
    {

    }

    public ImageFolderData()
    {

    }
}
