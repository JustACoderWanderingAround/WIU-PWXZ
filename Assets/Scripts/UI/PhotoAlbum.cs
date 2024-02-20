using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Created by: Tan Xiang Feng Wayne
public class PhotoAlbum : MonoBehaviour
{
    public Image imageRenderer;
    public TMPro.TMP_Text imageText;

    public Image imagePrefab;

    [Header("Read")]
    public string readFrom = "CaptureImage";

    private Dictionary<Sprite, Texture2D> textureReference = new Dictionary<Sprite, Texture2D>();
    private List<Sprite> images = new List<Sprite>();

    private int currentIndex = 0;

    private Coroutine loadImageRoutine = null;

    public bool renderAll = false;

    public bool hasLoaded = false;

    #region DebugOnly
    // Start is called before the first frame update
    void Start()
    {
        Reload();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LeftButton();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RightButton();
        }
    }
    #endregion

    public void Reload()
    {
        if (loadImageRoutine != null)
        {
            Debug.LogWarning("PhotoAlbum: Please Wait Before it's done Loading before reloading again.");
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
            }
        }
        Debug.Log("PhotoAlbum: End Loading");

        if (renderAll)
            RenderAllImage();
        else
            RenderImage();
        hasLoaded = true;
        loadImageRoutine = null;
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
        //Check if any Images is initialised
        if (images.Count <= 0)
        {
            Debug.LogWarning("You are trying to Render Non-Existent Image");
            return;
        }

        //Clear the Image
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //Create a Gameobject for each Object
        foreach (Sprite imageToRender in images)
        {
            Image imageHandler = Instantiate(imagePrefab, transform);
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
