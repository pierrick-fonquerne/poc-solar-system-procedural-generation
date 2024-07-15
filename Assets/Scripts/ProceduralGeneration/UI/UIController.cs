using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the UI for generating the solar system.
/// </summary>
public class UIController : MonoBehaviour
{
    /// <summary>
    /// The SolarSystemGenerator script to configure and start.
    /// </summary>
    public SolarSystemGenerator solarSystemGenerator;

    /// <summary>
    /// Input field for the number of stars.
    /// </summary>
    public InputField starCountInput;

    /// <summary>
    /// Input field for the number of planets.
    /// </summary>
    public InputField planetCountInput;

    /// <summary>
    /// Input field for the minimum star radius.
    /// </summary>
    public InputField minStarRadiusInput;

    /// <summary>
    /// Input field for the maximum star radius.
    /// </summary>
    public InputField maxStarRadiusInput;

    /// <summary>
    /// Input field for the minimum planet radius.
    /// </summary>
    public InputField minPlanetRadiusInput;

    /// <summary>
    /// Input field for the maximum planet radius.
    /// </summary>
    public InputField maxPlanetRadiusInput;

    /// <summary>
    /// Input field for the number of subdivisions for stars and planets.
    /// </summary>
    public InputField subdivisionsInput;

    /// <summary>
    /// Button to generate the solar system.
    /// </summary>
    public Button generateButton;

    /// <summary>
    /// Button to save the current configuration.
    /// </summary>
    public Button saveButton;

    /// <summary>
    /// Button to load a saved configuration.
    /// </summary>
    public Button loadButton;

    /// <summary>
    /// Button to reset and regenerate the solar system.
    /// </summary>
    public Button resetButton;

    private void Start()
    {
        generateButton.onClick.AddListener(OnGenerateButtonClicked);
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        loadButton.onClick.AddListener(OnLoadButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    /// <summary>
    /// Called when the generate button is clicked.
    /// Updates the SolarSystemGenerator with the values from the input fields and starts the generation.
    /// </summary>
    private void OnGenerateButtonClicked()
    {
        UpdateSolarSystemGeneratorParameters();
        solarSystemGenerator.GenerateSolarSystem();
    }

    /// <summary>
    /// Called when the save button is clicked.
    /// Saves the current configuration to PlayerPrefs.
    /// </summary>
    private void OnSaveButtonClicked()
    {
        PlayerPrefs.SetInt("StarCount", int.Parse(starCountInput.text));
        PlayerPrefs.SetInt("PlanetCount", int.Parse(planetCountInput.text));
        PlayerPrefs.SetFloat("MinStarRadius", float.Parse(minStarRadiusInput.text));
        PlayerPrefs.SetFloat("MaxStarRadius", float.Parse(maxStarRadiusInput.text));
        PlayerPrefs.SetFloat("MinPlanetRadius", float.Parse(minPlanetRadiusInput.text));
        PlayerPrefs.SetFloat("MaxPlanetRadius", float.Parse(maxPlanetRadiusInput.text));
        PlayerPrefs.SetInt("Subdivisions", int.Parse(subdivisionsInput.text));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Called when the load button is clicked.
    /// Loads the saved configuration from PlayerPrefs.
    /// </summary>
    private void OnLoadButtonClicked()
    {
        starCountInput.text = PlayerPrefs.GetInt("StarCount", 1).ToString();
        planetCountInput.text = PlayerPrefs.GetInt("PlanetCount", 3).ToString();
        minStarRadiusInput.text = PlayerPrefs.GetFloat("MinStarRadius", 10f).ToString();
        maxStarRadiusInput.text = PlayerPrefs.GetFloat("MaxStarRadius", 15f).ToString();
        minPlanetRadiusInput.text = PlayerPrefs.GetFloat("MinPlanetRadius", 2f).ToString();
        maxPlanetRadiusInput.text = PlayerPrefs.GetFloat("MaxPlanetRadius", 4f).ToString();
        subdivisionsInput.text = PlayerPrefs.GetInt("Subdivisions", 2).ToString();
    }

    /// <summary>
    /// Called when the reset button is clicked.
    /// Resets and regenerates the solar system.
    /// </summary>
    private void OnResetButtonClicked()
    {
        UpdateSolarSystemGeneratorParameters();
        solarSystemGenerator.ClearExistingSolarSystem();
        solarSystemGenerator.GenerateSolarSystem();
    }

    /// <summary>
    /// Updates the parameters of the SolarSystemGenerator based on the input fields.
    /// </summary>
    private void UpdateSolarSystemGeneratorParameters()
    {
        solarSystemGenerator.numberOfStars = int.Parse(starCountInput.text);
        solarSystemGenerator.numberOfPlanets = int.Parse(planetCountInput.text);
        solarSystemGenerator.minStarRadius = float.Parse(minStarRadiusInput.text);
        solarSystemGenerator.maxStarRadius = float.Parse(maxStarRadiusInput.text);
        solarSystemGenerator.minPlanetRadius = float.Parse(minPlanetRadiusInput.text);
        solarSystemGenerator.maxPlanetRadius = float.Parse(maxPlanetRadiusInput.text);
        solarSystemGenerator.subdivisions = int.Parse(subdivisionsInput.text);
    }
}
