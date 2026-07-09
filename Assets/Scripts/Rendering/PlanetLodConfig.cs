using UnityEngine;

/// <summary>
/// Configuration asset for planet LOD switching and benchmarking.
/// </summary>
[CreateAssetMenu(fileName = "PlanetLodConfig", menuName = "Settings/Planet LOD Config")]
public class PlanetLodConfig : ScriptableObject
{
    public float[] transitionDistances = new float[] { 150f, 400f };

    [Min(0f)]
    public float updateInterval = 0.1f;

    [Min(0f)]
    public float transitionHysteresis = 10f;

    public bool enableBenchmarking = false;

    [Min(1)]
    public int benchmarkSampleSize = 120;

    [Min(0f)]
    public float benchmarkLogInterval = 5f;

    public bool showDebugGizmos = false;
}
