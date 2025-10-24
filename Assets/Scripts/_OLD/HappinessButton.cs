/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HappinessButton : MonoBehaviour
{
    // Singleton
    public static HappinessButton Instance;
    private void Awake() {
        button = GetComponent<UIDocument>().rootVisualElement.Q<Button>("HappinessButton");
        button.clicked += OnClick;
        happinessIcon = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("HappinessIcon");
        happinessTab = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("HappinessTab");
        complaintBox = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ComplaintBox");
        outcomeText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("OutcomeText");
        happinessTab.style.display = DisplayStyle.None; // Hide the tab by default
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public Texture2D ecstaticTexture;
    public Texture2D happyTexture;
    public Texture2D upsetTexture;
    public Texture2D angryTexture;

    Button button;
    VisualElement happinessIcon;
    VisualElement happinessTab;
    VisualElement complaintBox;
    Label outcomeText;

    public List<string> complaints;


    private void Start() {
        

        GameManager.Instance.OnGameStateChanged += (_, _) => {
            DisplayHappinessTab(false);
        };
    }

    void OnClick() {
        StyleEnum<DisplayStyle> prev = happinessTab.style.display;

        GameManager.Instance.GameState = GameState.Default;

        GameManager.Instance.GameState = GameState.UI;
        DisplayHappinessTab(prev == DisplayStyle.None);
    }

    void ToggleHappinessTab() {
        DisplayHappinessTab(happinessTab.style.display == DisplayStyle.None);
    }

    void DisplayHappinessTab(bool b) {
        happinessTab.style.display = b ? DisplayStyle.Flex : DisplayStyle.None;
        if (b) {
            RedrawUI();
        }
    }

    public void UpdateStatus(List<string> complaints) {
        this.complaints = complaints;
        RedrawUI();
    }

    void RedrawUI() {
        happinessIcon.style.backgroundImage = new StyleBackground(
                       CityManager.Instance.Happiness switch {
                           HappinessLevel.Ecstatic => ecstaticTexture,
                           HappinessLevel.Happy => happyTexture,
                           HappinessLevel.Upset => upsetTexture,
                           HappinessLevel.Angry => angryTexture,
                           _ => null
                       });

        complaintBox.Clear();
        foreach (var complaint in complaints) {
            var label = new Label(complaint);
            label.AddToClassList("ComplaintText");
            complaintBox.Add(label);
        }

        var penalty = CityManager.Instance.GetHappinessPenalty();
        string penaltyText = (penalty * 100).ToString("F1") + "%";

        switch(CityManager.Instance.Happiness) {
            case HappinessLevel.Ecstatic:
                outcomeText.text = "Your citizens are ecstatic! The city works at its full capacity!";
                outcomeText.style.color = Color.green;
                break;
            case HappinessLevel.Happy:
                outcomeText.text = $"Your citizens are happy, but could be better. The city incurs a {penaltyText} penalty to income and research!";
                outcomeText.style.color = Color.yellow;
                break;
            case HappinessLevel.Upset:
                outcomeText.text = $"Your citizens are upset! The city incurs a {penaltyText} penalty to income and research!";
                outcomeText.style.color = new Color(1f, 0.5f, 0f); // orange
                break;
            case HappinessLevel.Angry:
                outcomeText.text = $"Your citizens are angry! The city incurs a {penaltyText} penalty to income and research!";
                outcomeText.style.color = Color.red;
                break;
        }
    }
}
*/