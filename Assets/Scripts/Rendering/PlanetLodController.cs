using UnityEngine;

[CreateAssetMenu(fileName = "PlanetLodConfig", menuName = "Settings/Planet LOD Config")]
public class PlanetLodConfig : ScriptableObject
{
    public float[] transitionDistances = new float[] { 25f, 50f, 100f };
    public bool showDebugGizmos = false;
}

/// <summary>
/// Controls level of detail for a planet by switching between meshes of different resolutions.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class PlanetLodController : MonoBehaviour
{
    [SerializeField]
    private PlanetLodConfig config;

    private Mesh[] lodMeshes;
    private MeshFilter meshFilter;
    private Transform cameraTransform;

    /// <summary>
    /// Initializes the LOD controller with meshes and configuration.
    /// </summary>
    /// <param name="meshes">Meshes ordered from highest to lowest detail.</param>
    /// <param name="lodConfig">Configuration for LOD distances and gizmos.</param>
    public void Initialize(Mesh[] meshes, PlanetLodConfig lodConfig)
    {
        lodMeshes = meshes;
        config = lodConfig;
        meshFilter = GetComponent<MeshFilter>();
        cameraTransform = Camera.main != null ? Camera.main.transform : null;

        if (lodMeshes != null && lodMeshes.Length > 0)
        {
            meshFilter.mesh = lodMeshes[0];
        }
    }

    private void Update()
    {
        if (lodMeshes == null || lodMeshes.Length == 0 || cameraTransform == null || config == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, cameraTransform.position);
        int level = 0;

        for (int i = 0; i < config.transitionDistances.Length; i++)
        {
            if (distance > config.transitionDistances[i])
            {
                level = Mathf.Min(i + 1, lodMeshes.Length - 1);
            }
        }

        meshFilter.mesh = lodMeshes[level];
    }

    private void OnDrawGizmosSelected()
    {
        if (config == null || !config.showDebugGizmos)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < config.transitionDistances.Length; i++)
        {
            Gizmos.DrawWireSphere(transform.position, config.transitionDistances[i]);
        }
    }
}
