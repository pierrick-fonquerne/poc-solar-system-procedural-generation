using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collects simple runtime statistics about LOD evaluation to help benchmark changes.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PlanetLodController))]
public class PlanetLodBenchmark : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int sampleCapacity = 120;

    [SerializeField]
    [Min(0f)]
    private float logIntervalSeconds = 5f;

    private PlanetLodController lodController;
    private readonly Queue<float> samples = new Queue<float>();
    private float accumulatedDuration;
    private float nextLogTime;

    /// <summary>
    /// Applies configuration values derived from the shared planet LOD settings asset.
    /// </summary>
    /// <param name="config">Planet LOD configuration to pull benchmark settings from.</param>
    public void ApplyConfig(PlanetLodConfig config)
    {
        if (config == null)
        {
            return;
        }

        sampleCapacity = Mathf.Max(1, config.benchmarkSampleSize);
        logIntervalSeconds = Mathf.Max(0f, config.benchmarkLogInterval);
        ResetLogTimer();
    }

    private void Awake()
    {
        lodController = GetComponent<PlanetLodController>();
        if (lodController == null)
        {
            Debug.LogWarning("PlanetLodBenchmark requires a PlanetLodController component to operate.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (lodController != null)
        {
            lodController.LodEvaluated += HandleLodEvaluated;
        }

        ResetLogTimer();
    }

    private void OnDisable()
    {
        if (lodController != null)
        {
            lodController.LodEvaluated -= HandleLodEvaluated;
        }

        samples.Clear();
        accumulatedDuration = 0f;
    }

    private void Update()
    {
        if (logIntervalSeconds <= 0f || samples.Count == 0)
        {
            return;
        }

        if (Time.realtimeSinceStartup >= nextLogTime)
        {
            LogSamples();
            ResetLogTimer();
        }
    }

    /// <summary>
    /// Forces an immediate log of current benchmark statistics.
    /// </summary>
    public void ForceLog()
    {
        if (samples.Count == 0)
        {
            Debug.Log($"LOD benchmark for {name}: no samples collected yet.", this);
            return;
        }

        LogSamples();
        ResetLogTimer();
    }

    private void HandleLodEvaluated(float durationMs)
    {
        samples.Enqueue(durationMs);
        accumulatedDuration += durationMs;

        while (samples.Count > sampleCapacity)
        {
            float removed = samples.Dequeue();
            accumulatedDuration -= removed;
        }
    }

    private void LogSamples()
    {
        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (float sample in samples)
        {
            if (sample < min)
            {
                min = sample;
            }

            if (sample > max)
            {
                max = sample;
            }
        }

        float average = accumulatedDuration / samples.Count;
        Debug.Log(
            $"LOD benchmark for {name}: avg {average:F4} ms, min {min:F4} ms, max {max:F4} ms over {samples.Count} samples.",
            this);
    }

    private void ResetLogTimer()
    {
        nextLogTime = Time.realtimeSinceStartup + Mathf.Max(0.001f, logIntervalSeconds);
    }
}
