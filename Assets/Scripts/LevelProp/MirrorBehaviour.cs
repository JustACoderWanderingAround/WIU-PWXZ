using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBehaviour : MonoBehaviour
{
    public Shader shader;

    [Header("For Update")]
    private Camera _camera = null;
    public bool toUpdate = true;
    private Material _material;
    private Renderer _renderer;
    private MeshFilter _mf;

    [Header("Scale")]
    [Range(100f, 10000f)]
    public float scaleMultiplier = 1000f;
    public float adjustValue = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        //Update Camera Position
        Vector3 localPlayer = transform.InverseTransformPoint(Camera.main.transform.position);
        _camera.transform.position = transform.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));
        //Update Camera Rotation
        Vector3 lookAtMirror = transform.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        _camera.transform.LookAt(lookAtMirror);

        _camera.nearClipPlane = Vector3.Distance(_camera.transform.position, transform.position);

        //Alternate Calculation for More Exact Result
        /*
        //Get World Position of ALL the vertices of the mesh
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        Vector3[] world_v = new Vector3[_mf.mesh.vertices.Length];

        //Convert into world position
        for (int i = 0; i < _mf.mesh.vertices.Length; ++i)
        {
            world_v[i] = localToWorld.MultiplyPoint3x4(_mf.mesh.vertices[i]);
        }

        //Get the closest vertices
        Vector3 positionToCompare = NearestVector(_camera.transform.position, world_v);
        Vector3 diff = positionToCompare - _camera.transform.position;
        _camera.fieldOfView = Mathf.Atan2(diff.y, -diff.z) * Mathf.Rad2Deg;
        */

        //Calculation that is used
        //To minimaise calculation
        _camera.fieldOfView = 60f - Vector3.Distance(localPlayer, transform.position) * adjustValue;
    }

    private void OnValidate()
    {
        UpdateCameraTexture();
    }

    public void Initialise()
    {
        UpdateCameraTexture();
        _mf = GetComponent<MeshFilter>();
    }

    private void UpdateCameraTexture()
    {
        //Create a Render Texture based on the new local Scale
        RenderTexture newRenderTexture = new RenderTexture(Mathf.FloorToInt(transform.localScale.x * scaleMultiplier),
                                                    Mathf.FloorToInt(transform.localScale.y * scaleMultiplier), 16);
        _camera = GetComponentInChildren<Camera>();
        if (_camera == null)
            Debug.LogWarning("MirrorBehaviour: There is no Camera Attached to the Mirror");

        _camera.targetTexture = newRenderTexture;

        _renderer = GetComponent<Renderer>();

        //If in Playing Mode
        if (Application.isPlaying)
        {
            _material = _renderer.material;
        }
        //In Edit Mode
        else if (_renderer.sharedMaterial == null || _material == null)
        {
            //Update the render texture
            _material = new Material(shader);
            _renderer.sharedMaterial = _material;
        }

        _material.SetTexture("_Texture2D", newRenderTexture);

        StartCoroutine(WaitForNextFrame(() => _camera.backgroundColor = AverageColorFromTexture(toTexture2D(_camera.targetTexture))));

        IEnumerator WaitForNextFrame(System.Action func)
        {
            yield return null;
            func.Invoke();
        }
    }

    Color32 AverageColorFromTexture(Texture2D texture)
    {
        Color32[] texColors = texture.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {
            r += texColors[i].r;
            g += texColors[i].g;
            b += texColors[i].b;
        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
    }

    Texture2D toTexture2D(RenderTexture renderTex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = renderTex;
        tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    Vector3 NearestVector(Vector3 comparison, Vector3[] vectors)
    {
        float minDistance = float.MaxValue;
        Vector3 valueToReturn = Vector3.zero;
        for (int i = 0; i < vectors.Length; i++)
        {
            float distance = Vector3.Distance(comparison, vectors[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                valueToReturn = vectors[i];
            }
        }
        return valueToReturn;
    }
}
