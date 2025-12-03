using UnityEngine;

/// <summary>
/// Renders mech sprites to a RenderTexture and displays with a silhouette outline shader.
/// This creates a unified outline around the entire mech rather than individual sprite outlines.
/// </summary>
public class MechOutlineRenderer : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float outlineWidth = 4f;
    [SerializeField] private Color outlineColor = Color.white;

    [Header("Render Settings")]
    [SerializeField] private int renderTextureSize = 512;
    [SerializeField] private float quadScale = 5f;

    [Header("Layer Settings")]
    [SerializeField] private string mechOutlineLayerName = "MechOutline";

    private Camera mechCamera;
    private RenderTexture mechRenderTexture;
    private GameObject outlineQuad;
    private Material outlineMaterialInstance;
    private MechController mechController;

    private int playerIndex = -1; // 0-3 for players 1-4
    private int mechSpritesLayer = -1;
    private int mechOutlineLayer;

    void Awake()
    {
        mechController = GetComponent<MechController>();
        mechOutlineLayer = LayerMask.NameToLayer(mechOutlineLayerName);

        if (mechOutlineLayer == -1)
        {
            Debug.LogWarning($"[MechOutlineRenderer] Layer '{mechOutlineLayerName}' not found. Please create it in Tags and Layers.");
        }
    }

    void Start()
    {
        if (mechOutlineLayer == -1 || outlineMaterial == null)
        {
            Debug.LogError("[MechOutlineRenderer] Missing required setup. Outline rendering disabled.");
            enabled = false;
            return;
        }

        // If player index wasn't set, default to 0 and warn
        if (playerIndex == -1)
        {
            Debug.LogWarning("[MechOutlineRenderer] Player index not set. Defaulting to player 0. Call SetPlayerIndex() before Start.");
            SetPlayerIndex(0);
        }

        SetupRenderTexture();
        SetupMechCamera();
        SetupOutlineQuad();
        AssignMechSpritesToLayer();
    }

    void LateUpdate()
    {
        // Keep outline quad centered on mech
        if (outlineQuad != null)
        {
            outlineQuad.transform.position = transform.position;
        }
    }

    private void SetupRenderTexture()
    {
        // Depth buffer (16-bit) required by URP Render Graph API for camera output textures
        mechRenderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 16, RenderTextureFormat.ARGB32);
        mechRenderTexture.filterMode = FilterMode.Bilinear;
        mechRenderTexture.Create();
    }

    private void SetupMechCamera()
    {
        GameObject cameraObj = new GameObject("MechOutlineCamera");
        cameraObj.transform.SetParent(transform);
        cameraObj.transform.localPosition = new Vector3(0, 0, -10);

        mechCamera = cameraObj.AddComponent<Camera>();
        mechCamera.orthographic = true;
        mechCamera.orthographicSize = quadScale / 2f;
        mechCamera.nearClipPlane = 0.1f;
        mechCamera.farClipPlane = 20f;
        mechCamera.clearFlags = CameraClearFlags.SolidColor;
        mechCamera.backgroundColor = new Color(0, 0, 0, 0);
        mechCamera.cullingMask = 1 << mechSpritesLayer;
        mechCamera.targetTexture = mechRenderTexture;
        mechCamera.depth = -10; // Render before main camera

        // Remove AudioListener if one was added
        AudioListener listener = cameraObj.GetComponent<AudioListener>();
        if (listener != null)
        {
            Destroy(listener);
        }
    }

    private void SetupOutlineQuad()
    {
        outlineQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        outlineQuad.name = "MechOutlineQuad";
        outlineQuad.transform.SetParent(null); // Don't parent to mech so it doesn't flip with mech
        outlineQuad.transform.position = transform.position;
        outlineQuad.transform.localScale = new Vector3(quadScale, quadScale, 1f);
        outlineQuad.layer = mechOutlineLayer;

        // Remove collider
        Collider col = outlineQuad.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        // Setup material
        MeshRenderer renderer = outlineQuad.GetComponent<MeshRenderer>();
        outlineMaterialInstance = new Material(outlineMaterial);
        outlineMaterialInstance.SetTexture("_MainTex", mechRenderTexture);
        outlineMaterialInstance.SetFloat("_OutlineWidth", outlineWidth);
        outlineMaterialInstance.SetColor("_OutlineColor", outlineColor);
        renderer.material = outlineMaterialInstance;
        renderer.sortingLayerName = mechOutlineLayerName;
        renderer.sortingOrder = 0;
    }

    private void AssignMechSpritesToLayer()
    {
        if (mechController == null || mechController.skeletonRig == null)
        {
            Debug.LogWarning("[MechOutlineRenderer] Cannot assign sprite layers - MechController or skeletonRig is null");
            return;
        }

        if (mechSpritesLayer == -1)
        {
            Debug.LogWarning("[MechOutlineRenderer] Cannot assign sprite layers - layer not found. Did you create MechSprites1-4?");
            return;
        }

        SpriteRenderer[] renderers = mechController.skeletonRig.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in renderers)
        {
            sr.gameObject.layer = mechSpritesLayer;
        }
    }

    /// <summary>
    /// Call this after parts are swapped to update sprite layers.
    /// </summary>
    public void RefreshSpriteLayers()
    {
        AssignMechSpritesToLayer();
    }

    /// <summary>
    /// Sets the player index for layer isolation. Each player gets their own sprite layer
    /// so outline cameras don't capture other players' mechs.
    /// Must be called before Start() runs, ideally right after mech instantiation.
    /// </summary>
    /// <param name="index">0-based player index (0-3 for players 1-4)</param>
    public void SetPlayerIndex(int index)
    {
        playerIndex = Mathf.Clamp(index, 0, 3);

        // Get the per-player layer (MechSprites1, MechSprites2, etc.)
        string layerName = $"MechSprites{playerIndex + 1}";
        mechSpritesLayer = LayerMask.NameToLayer(layerName);

        if (mechSpritesLayer == -1)
        {
            Debug.LogWarning($"[MechOutlineRenderer] Layer '{layerName}' not found. Please create layers MechSprites1-4 in Tags and Layers.");
        }

        // If camera already exists, update its culling mask
        if (mechCamera != null)
        {
            mechCamera.cullingMask = 1 << mechSpritesLayer;
        }

        // If sprites already assigned, reassign to new layer
        if (mechController != null && mechController.skeletonRig != null)
        {
            AssignMechSpritesToLayer();
        }
    }

    /// <summary>
    /// Set the outline color for this mech.
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        if (outlineMaterialInstance != null)
        {
            outlineMaterialInstance.SetColor("_OutlineColor", color);
        }
    }

    /// <summary>
    /// Set the outline width.
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        if (outlineMaterialInstance != null)
        {
            outlineMaterialInstance.SetFloat("_OutlineWidth", width);
        }
    }

    void OnDestroy()
    {
        if (mechRenderTexture != null)
        {
            mechRenderTexture.Release();
            Destroy(mechRenderTexture);
        }

        if (outlineMaterialInstance != null)
        {
            Destroy(outlineMaterialInstance);
        }

        if (outlineQuad != null)
        {
            Destroy(outlineQuad);
        }
    }
}
