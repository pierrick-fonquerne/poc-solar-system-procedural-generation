using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISetup : MonoBehaviour
{
    [MenuItem("Tools/Setup Solar System UI")]
    public static void SetupUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Event System
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Create UI elements
        CreateInputField(canvasGO.transform, "StarCountInput", "Enter number of stars", new Vector2(-200, 200));
        CreateInputField(canvasGO.transform, "PlanetCountInput", "Enter number of planets", new Vector2(-200, 150));
        CreateInputField(canvasGO.transform, "MinStarRadiusInput", "Enter min star radius", new Vector2(-200, 100));
        CreateInputField(canvasGO.transform, "MaxStarRadiusInput", "Enter max star radius", new Vector2(-200, 50));
        CreateInputField(canvasGO.transform, "MinPlanetRadiusInput", "Enter min planet radius", new Vector2(-200, 0));
        CreateInputField(canvasGO.transform, "MaxPlanetRadiusInput", "Enter max planet radius", new Vector2(-200, -50));
        CreateInputField(canvasGO.transform, "SubdivisionsInput", "Enter number of subdivisions", new Vector2(-200, -100));

        CreateButton(canvasGO.transform, "GenerateButton", "Generate", new Vector2(0, -200));
        CreateButton(canvasGO.transform, "SaveButton", "Save", new Vector2(-150, -250));
        CreateButton(canvasGO.transform, "LoadButton", "Load", new Vector2(150, -250));
        CreateButton(canvasGO.transform, "ResetButton", "Reset", new Vector2(0, -300));

        // Add UIController to Canvas
        UIController uiController = canvasGO.AddComponent<UIController>();

        // Assign references
        uiController.solarSystemGenerator = FindObjectOfType<SolarSystemGenerator>();
        uiController.starCountInput = GameObject.Find("StarCountInput").GetComponent<InputField>();
        uiController.planetCountInput = GameObject.Find("PlanetCountInput").GetComponent<InputField>();
        uiController.minStarRadiusInput = GameObject.Find("MinStarRadiusInput").GetComponent<InputField>();
        uiController.maxStarRadiusInput = GameObject.Find("MaxStarRadiusInput").GetComponent<InputField>();
        uiController.minPlanetRadiusInput = GameObject.Find("MinPlanetRadiusInput").GetComponent<InputField>();
        uiController.maxPlanetRadiusInput = GameObject.Find("MaxPlanetRadiusInput").GetComponent<InputField>();
        uiController.subdivisionsInput = GameObject.Find("SubdivisionsInput").GetComponent<InputField>();
        uiController.generateButton = GameObject.Find("GenerateButton").GetComponent<Button>();
        uiController.saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        uiController.loadButton = GameObject.Find("LoadButton").GetComponent<Button>();
        uiController.resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
    }

    private static void CreateInputField(Transform parent, string name, string placeholderText, Vector2 position)
    {
        // Create Input Field
        GameObject inputFieldGO = new GameObject(name);
        inputFieldGO.transform.SetParent(parent);

        RectTransform rectTransform = inputFieldGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 30);
        rectTransform.anchoredPosition = position;

        InputField inputField = inputFieldGO.AddComponent<InputField>();

        // Create child Text (Placeholder)
        GameObject placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(inputFieldGO.transform);
        Text placeholderTextComponent = placeholderGO.AddComponent<Text>();
        placeholderTextComponent.text = placeholderText;
        placeholderTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderTextComponent.fontSize = 14;
        placeholderTextComponent.color = Color.gray;
        RectTransform placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.sizeDelta = rectTransform.sizeDelta;
        placeholderRect.anchoredPosition = Vector2.zero;

        inputField.placeholder = placeholderTextComponent;

        // Create child Text (Text)
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(inputFieldGO.transform);
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.black;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = rectTransform.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        inputField.textComponent = textComponent;
    }

    private static void CreateButton(Transform parent, string name, string buttonText, Vector2 position)
    {
        // Create Button
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent);

        RectTransform rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 30);
        rectTransform.anchoredPosition = position;

        Button button = buttonGO.AddComponent<Button>();

        // Create child Text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform);
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = buttonText;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.alignment = TextAnchor.MiddleCenter;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.sizeDelta = rectTransform.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        button.targetGraphic = textComponent;
    }
}
