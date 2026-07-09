public static class Constants
{
    public const float AU_TO_UNITY_UNITS = 50f;
    public const float PARSEC_TO_UNITY_UNITS = 500f;

    // Gravitational constant scaled for the simulation. The real SI value
    // (6.674e-11) combined with Unity-scale masses and distances produces
    // orbital speeds indistinguishable from zero, so the constant is boosted
    // to keep Kepler-derived speeds visually meaningful.
    public const float GRAVITATIONAL_CONSTANT = 6.674e-3f;

    public const float SCALE_FACTOR = 5f;
}
