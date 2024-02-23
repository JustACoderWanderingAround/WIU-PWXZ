using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Created by: Tan Xiang Feng Wayne
public class PhotoAlbum : MonoBehaviour
{
    public Transform imageTransform;

    public Image imageRenderer;
    public TMPro.TMP_Text imageText;

    public Image imagePrefab;

    [Header("Read")]
    public string readFrom = "CaptureImage";

    private Dictionary<Sprite, Texture2D> textureReference = new Dictionary<Sprite, Texture2D>();
    private List<Sprite> images = new List<Sprite>();
    private List<Sprite> unsavedImages = new List<Sprite>();

    private int currentIndex = 0;

    private Coroutine loadImageRoutine = null;
    private Coroutine saveImageRoutine = null;
    private Coroutine clearImageRoutine = null;

    public bool renderAll = false;

    public bool hasLoaded = false;

    #region DebugOnly
    // Start is called before the first frame update
    void Start()
    {
        //Reload();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearImages();
        }
        /*
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftButton();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightButton();
        }*/
    }
    #endregion

    public void Reload()
    {
        if (loadImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait Before it's done Loading before reloading again.");
            return;
        }
        if (saveImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait for the Images to done Saving before loading");
            return;
        }
        if (clearImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait for the Images to done Clearing before loading");
            return;
        }
        loadImageRoutine = StartCoroutine(LoadImageRoutine());
    }

    private IEnumerator LoadImageRoutine()
    {
        //Wait for it to load finish
        while (CameraCapture.constantFolderPath == null || CameraCapture.constantFolderPath == string.Empty)
            yield return null;

        string JSONPath = System.IO.Path.Combine(CameraCapture.constantFolderPath, $"{readFrom}Data.json");
        //Check if it exits, then update accordingly
        if (!System.IO.File.Exists(JSONPath))
        {
            Debug.LogWarning("File Doesn't Exist. Please Check if \"readFrom\" variable is initialised Properly");
            yield break;
        }
        ImageFolderData data = JsonUtility.FromJson<ImageFolderData>(System.IO.File.ReadAllText(JSONPath));
        if (data == null)
        {
            Debug.LogWarning("JSON File is not Initialised Yet");
            yield break;
        }

        Debug.Log("Begin Loading");

        images.Clear();
        //Create loading Image
        for (int i = 0; i < data.imageCount; i++)
        {
            images.Add(null);
        }
        //Render it
        if (renderAll)
            RenderAllImage();
        else
            RenderImage();

        images.Clear();
        textureReference.Clear();
        currentIndex = 0;

        Debug.Log("PhotoAlbum: Start Loading");
        //Loop through the image count
        for (int i = 0; i < data.imageCount; i++)
        {
            string imagePath = System.IO.Path.Combine(CameraCapture.constantFolderPath, $"{data.imagePrefix}{i}.png");
            if (System.IO.File.Exists(imagePath))
            {
                //Get image to be the exact size with loading in with bytes 
                byte[] textureBytes = System.IO.File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(1, 1);
                //Load the image with ref to bytes
                texture.LoadImage(textureBytes);
                Debug.Log($"Texture {data.imagePrefix}{i} Raw Size: {texture.width}x{texture.height}");

                //Get Sprite of it (Do not delete texture of it, as sprite is created with a reference to it)
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(texture.width * 0.5f, texture.height * 0.5f));

                //Add to the list
                images.Add(sprite);
                textureReference.Add(sprite, texture);
                //Wait for one frame for other threads to run
                yield return null;
            }
        }
        unsavedImages.Clear();
        Debug.Log("PhotoAlbum: End Loading");

        if (renderAll)
            RenderAllImage();
        else
            RenderImage();
        hasLoaded = true;
        loadImageRoutine = null;
    }

    public void SaveImage()
    {
        if (saveImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait for the Images to done Saving before saving again");
            return;
        }
        if (clearImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait for the Images to done Clearing before saving again");
            return;
        }
        saveImageRoutine = StartCoroutine(SaveImageRoutine());
    }

    private IEnumerator SaveImageRoutine()
    {
        //Wait for it to load finish
        while (CameraCapture.constantFolderPath == null || CameraCapture.constantFolderPath == string.Empty)
            yield return null;

        //Read necessary Data from JSON
        string JSONPath = System.IO.Path.Combine(CameraCapture.constantFolderPath, $"{CameraCapture.FileName}Data.json");
        //Check if such JSON FIle exists
        if (!System.IO.File.Exists(JSONPath))
        {
            System.IO.FileStream fs = new System.IO.FileInfo(JSONPath).Create();
            fs.Close();
        }

        string JSONText = System.IO.File.ReadAllText(JSONPath);
        //Overwrite Data and Create One if it is not initialised Properly
        ImageFolderData data = JsonUtility.FromJson<ImageFolderData>(JSONText) ?? new ImageFolderData();

        for (int i = 0; i < unsavedImages.Count; i++)
        {
            int incremental = data.imageCount + i;
            string capturePath = System.IO.Path.Combine(CameraCapture.constantFolderPath, CameraCapture.FileName + incremental + ".png");

            byte[] byteArray = unsavedImages[i].texture.EncodeToPNG();

            //Write to the path
            System.IO.File.WriteAllBytes(capturePath, byteArray);

            Debug.Log($"Unsaved Image {i} has been saved in {capturePath}");
            //Wait for one frame for other to run too
            yield return null;
        }

        //Update JSON
        data.imagePrefix = CameraCapture.FileName;
        data.imageCount = data.imageCount + unsavedImages.Count;
        data.lastSavedAt = System.DateTime.Now.ToString();

        //Convert back to string
        JSONText = JsonUtility.ToJson(data);

        System.IO.File.WriteAllText(JSONPath, JSONText);

        Debug.Log($"JSONFIle {JSONPath} has been Updated! : {JSONText}");

        saveImageRoutine = null;
    }
    
    public void ClearImages()
    {
        if (clearImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait for the Images to be done clearing");
            return;
        }
        clearImageRoutine = StartCoroutine(ClearImageRoutine());
    }

    private IEnumerator ClearImageRoutine()
    {
        Debug.Log("ISCalled");
        //Wait for it to load finish
        while (CameraCapture.constantFolderPath == null || CameraCapture.constantFolderPath == string.Empty)
            yield return null;
        Debug.Log("Starting");
        //Read necessary Data from JSON
        string JSONPath = System.IO.Path.Combine(CameraCapture.constantFolderPath, $"{CameraCapture.FileName}Data.json");
        //Check if such JSON FIle exists
        if (!System.IO.File.Exists(JSONPath))
        {
            Debug.Log("No Images To Clear, Directory/JSON Does Not Exist");
            yield break;
        }

        string JSONText = System.IO.File.ReadAllText(JSONPath);
        ImageFolderData data = JsonUtility.FromJson<ImageFolderData>(JSONText);

        if (data == null)
        {
            Debug.Log("No Images To Clear, No Previous Saves exists");
            yield break;
        }

        for (int i = 0; i < data.imageCount; i++)
        {
            string capturePath = System.IO.Path.Combine(CameraCapture.constantFolderPath, data.imagePrefix + i + ".png");

            //Delete Obj at Path
            System.IO.File.Delete(capturePath);

            Debug.Log($"Image {i} has been Cleared");
            //Wait for one frame for other to run too
            yield return null;
        }

        //Clear
        System.IO.File.WriteAllText(JSONPath, JsonUtility.ToJson(null));
        Debug.Log("PhotoAlbum: Clear Image Complete");
        clearImageRoutine = null;
    }

    /// <summary>
    /// Temporarily add Image to a list such as players not need to reload to look at new images taken.
    /// This hugely reduces the time taken.
    /// </summary>
    /// <param name="textureBytes">Image as Byte</param>
    public void AddImage(byte[] textureBytes)
    {
        Texture2D texture = new Texture2D(1, 1);
        //Load the image with ref to bytes
        texture.LoadImage(textureBytes);

        //Get Sprite of it (Do not delete texture of it, as sprite is created with a reference to it)
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(texture.width * 0.5f, texture.height * 0.5f));

        //Add to the list
        unsavedImages.Add(sprite);

        if (renderAll)
            RenderAllImage();
        else
            RenderImage();
    }

    /// <summary>
    /// Temporarily add Image to a list such as players not need to reload to look at new images taken.
    /// This hugely reduces the time taken.
    /// </summary>
    public void AddImage(Texture2D texture)
    {
        //Get Sprite of it (Do not delete texture of it, as sprite is created with a reference to it)
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(texture.width * 0.5f, texture.height * 0.5f));

        //Add to the list
        unsavedImages.Add(sprite);

        if (renderAll)
            RenderAllImage();
        else
            RenderImage();
    }

    public void RenderImage()
    {
        //Check if any Images is initialised
        if (images.Count <= 0)
        {
            Debug.LogWarning("You are trying to Render Non-Existent Image");
            return;
        }

        currentIndex %= images.Count;
        //Set the image and text accordingly
        imageRenderer.overrideSprite = images[currentIndex];
        imageText.text = readFrom + currentIndex + ".png";
    }

    public void RenderAllImage()
    {
        //Clear the Image
        foreach (Transform child in imageTransform)
        {
            Destroy(child.gameObject);
        }

        //Check if any Images is initialised
        if (images.Count <= 0 && unsavedImages.Count <= 0)
        {
            Debug.LogWarning("You are trying to Render Non-Existent Image");
            return;
        }

        //Create a Gameobject for each Object
        foreach (Sprite imageToRender in images)
        {
            Image imageHandler = Instantiate(imagePrefab, imageTransform);
            imageHandler.transform.GetChild(0).GetComponent<Image>().sprite = imageToRender;
        }

        //Create a Gameobject for each Object
        foreach (Sprite imageToRender in unsavedImages)
        {
            Image imageHandler = Instantiate(imagePrefab, imageTransform);
            imageHandler.transform.GetChild(0).GetComponent<Image>().sprite = imageToRender;
        }
    }

    public void LeftButton()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = images.Count - 1;

        RenderImage();
    }

    public void RightButton()
    {
        currentIndex++;
        RenderImage();
    }
}
