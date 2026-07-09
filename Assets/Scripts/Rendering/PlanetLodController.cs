using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Controls level of detail for a planet by switching between meshes of different resolutions.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class PlanetLodController : MonoBehaviour
{
    // Fallback values used when no PlanetLodConfig asset is assigned, so LOD
    // switching still works out of the box.
    private static readonly float[] DefaultTransitionDistances = { 150f, 400f };
    private const float DefaultTransitionHysteresis = 10f;
    private const float DefaultUpdateInterval = 0.1f;

    [SerializeField]
    private PlanetLodConfig config;

    private Mesh[] lodMeshes = Array.Empty<Mesh>();
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Transform cameraTransform;
    private bool isInitialized;
    private int currentLodIndex;
    private float nextLodUpdateTime;
    private float[] degradeSqrThresholds = Array.Empty<float>();
    private float[] upgradeSqrThresholds = Array.Empty<float>();
    private readonly Stopwatch benchmarkStopwatch = new Stopwatch();

    public event Action<float> LodEvaluated;

    /// <summary>
    /// Initializes the LOD controller with meshes and configuration.
    /// </summary>
    /// <param name="meshes">Meshes ordered from highest to lowest detail.</param>
    /// <param name="lodConfig">Configuration for LOD distances and gizmos.</param>
    public void Initialize(Mesh[] meshes, PlanetLodConfig lodConfig)
    {
        lodMeshes = meshes ?? Array.Empty<Mesh>();
        config = lodConfig;
        meshFilter = meshFilter != null ? meshFilter : GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            UnityEngine.Debug.LogWarning("PlanetLodController requires a MeshFilter to operate.", this);
            lodMeshes = Array.Empty<Mesh>();
            isInitialized = false;
            return;
        }

        cameraTransform = Camera.main != null ? Camera.main.transform : null;
        currentLodIndex = 0;
        isInitialized = lodMeshes.Length > 0;

        if (isInitialized)
        {
            RebuildThresholds();
            ApplyLod(currentLodIndex);
        }

        nextLodUpdateTime = Time.time;
    }

    public void SetMeshCollider(MeshCollider collider)
    {
        meshCollider = collider;
        if (meshCollider != null && lodMeshes.Length > 0)
        {
            meshCollider.sharedMesh = lodMeshes[Mathf.Clamp(currentLodIndex, 0, lodMeshes.Length - 1)];
        }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void OnEnable()
    {
        if (!isInitialized && lodMeshes.Length > 0 && meshFilter != null)
        {
            RebuildThresholds();
            ApplyLod(Mathf.Clamp(currentLodIndex, 0, lodMeshes.Length - 1));
            isInitialized = true;
        }
    }

    private void Update()
    {
        if (!isInitialized || lodMeshes.Length == 0 || meshFilter == null)
        {
            return;
        }

        if (cameraTransform == null)
        {
            Camera main = Camera.main;
            if (main == null)
            {
                return;
            }

            cameraTransform = main.transform;
        }

        float updateInterval = config != null ? Mathf.Max(0f, config.updateInterval) : DefaultUpdateInterval;
        float now = Time.time;
        if (now < nextLodUpdateTime)
        {
            return;
        }

        nextLodUpdateTime = now + updateInterval;

        Vector3 offset = cameraTransform.position - transform.position;
        float sqrDistance = offset.sqrMagnitude;
        EvaluateAndApplyLod(sqrDistance);
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

    private void EvaluateAndApplyLod(float sqrDistance)
    {
        bool benchmarking = LodEvaluated != null;
        if (benchmarking)
        {
            benchmarkStopwatch.Restart();
        }

        int targetIndex = DetermineLodIndex(sqrDistance);
        if (targetIndex != currentLodIndex)
        {
            ApplyLod(targetIndex);
        }

        if (benchmarking)
        {
            benchmarkStopwatch.Stop();
            LodEvaluated?.Invoke((float)benchmarkStopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private int DetermineLodIndex(float sqrDistance)
    {
        int targetIndex = currentLodIndex;

        for (int i = targetIndex; i < degradeSqrThresholds.Length; i++)
        {
            if (sqrDistance > degradeSqrThresholds[i])
            {
                targetIndex = i + 1;
            }
            else
            {
                break;
            }
        }

        for (int i = targetIndex - 1; i >= 0; i--)
        {
            if (sqrDistance < upgradeSqrThresholds[i])
            {
                targetIndex = i;
            }
            else
            {
                break;
            }
        }

        return Mathf.Clamp(targetIndex, 0, lodMeshes.Length - 1);
    }

    private void ApplyLod(int targetIndex)
    {
        if (lodMeshes.Length == 0)
        {
            return;
        }

        currentLodIndex = Mathf.Clamp(targetIndex, 0, lodMeshes.Length - 1);
        Mesh targetMesh = lodMeshes[currentLodIndex];
        if (targetMesh == null)
        {
            return;
        }

        if (meshFilter.sharedMesh != targetMesh)
        {
            meshFilter.sharedMesh = targetMesh;
        }

        if (meshCollider != null && meshCollider.sharedMesh != targetMesh)
        {
            meshCollider.sharedMesh = targetMesh;
        }
    }

    private void RebuildThresholds()
    {
        int transitions = Mathf.Max(0, lodMeshes.Length - 1);
        if (transitions == 0)
        {
            degradeSqrThresholds = Array.Empty<float>();
            upgradeSqrThresholds = Array.Empty<float>();
            return;
        }

        float hysteresis = config != null ? Mathf.Max(0f, config.transitionHysteresis) : DefaultTransitionHysteresis;
        float[] transitionDistances = config != null ? config.transitionDistances : DefaultTransitionDistances;
        degradeSqrThresholds = new float[transitions];
        upgradeSqrThresholds = new float[transitions];

        for (int i = 0; i < transitions; i++)
        {
            float distance = (transitionDistances != null && i < transitionDistances.Length)
                ? transitionDistances[i]
                : float.PositiveInfinity;

            if (float.IsPositiveInfinity(distance))
            {
                degradeSqrThresholds[i] = float.PositiveInfinity;
                upgradeSqrThresholds[i] = float.PositiveInfinity;
                continue;
            }

            degradeSqrThresholds[i] = distance * distance;
            float upgradeDistance = Mathf.Max(0f, distance - hysteresis);
            upgradeSqrThresholds[i] = upgradeDistance * upgradeDistance;
        }
    }
}
