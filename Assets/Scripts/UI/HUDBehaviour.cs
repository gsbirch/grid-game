using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDBehaviour : MonoBehaviour
{
    public Label healthLabel;
    public Label apLabel;
    public Label refreshLabel;
    public Label movementLabel;
    public Label meteorLabel;
    public VisualElement initiativeList;

    public Label notification;

    public Button helpButton;
    public Button moveButton;
    public Button passTurnButton;

    float notificationDuration = 0f;

    public VisualElement screenOverlay;
    public Label instructions;
    public Button exitOverlayButton;

    // Other managers may add and remove functionality from this button
    public Button confirmButton;

    public VisualElement infoPanel;
    public Label infoText;

    public Label statusText;


    public VisualElement helpPanel;
    public VisualElement cardListPanel;

    public VisualElement cardPanel;
    public VisualElement cardImage;
    public Label cardName;
    public Label cardDescription;
    public Label cardCostLabel;

    public Label DEBUG_CurrentHex;

    public Dictionary<string, Button> zoneButtons = new Dictionary<string, Button>();


    // Singleton
    public static HUDBehaviour Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        var root = GetComponent<UIDocument>().rootVisualElement;
        screenOverlay = root.Q<VisualElement>("ScreenOverlay");
        instructions = root.Q<Label>("Instructions");
        helpButton = root.Q<Button>("HelpButton");
        helpPanel = root.Q<VisualElement>("HelpPanel");
        healthLabel = root.Q<Label>("HealthLabel");
        apLabel = root.Q<Label>("APLabel");
        refreshLabel = root.Q<Label>("RefreshLabel");
        movementLabel = root.Q<Label>("MovementLabel");
        notification = root.Q<Label>("Notification");
        moveButton = root.Q<Button>("MoveButton");
        passTurnButton = root.Q<Button>("PassTurnButton");
        infoPanel = root.Q<VisualElement>("InfoPanel");
        infoText = root.Q<Label>("InfoText");
        cardListPanel = root.Q<VisualElement>("CardListPanel");
        cardPanel = root.Q<VisualElement>("CardInfoPanel");
        cardImage = root.Q<VisualElement>("CardImage");
        cardName = root.Q<Label>("CardName");
        cardDescription = root.Q<Label>("CardDescription");
        cardCostLabel = root.Q<Label>("CardCostLabel");
        initiativeList = root.Q<VisualElement>("InitiativeList");
        confirmButton = root.Q<Button>("ConfirmButton");
        DEBUG_CurrentHex = root.Q<Label>("CurrentHex");
        statusText = root.Q<Label>("StatusText");
        meteorLabel = root.Q<Label>("MeteorLabel");
        

        helpButton.clicked += () => {
            StyleEnum<DisplayStyle> prev = helpPanel.style.display;

            GameManager.Instance.GameState = GameState.PlayerTurnDefault;

            GameManager.Instance.GameState = GameState.UI;

            helpPanel.style.display = prev == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        };
        helpPanel.style.display = DisplayStyle.None;


        exitOverlayButton = root.Q<Button>("ExitOverlayButton");
        exitOverlayButton.clicked += () => {
            GameManager.Instance.GameState = GameState.PlayerTurnDefault;
        };

        passTurnButton.clicked += () => {
            GameManager.Instance.GameState = GameState.PlayerTurnDefault;
            InitiativeManager.Instance.PassTurn();
        };
    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += (_, _) => {
            helpPanel.style.display = DisplayStyle.None;
            screenOverlay.style.display = DisplayStyle.None;
            HideCard();
            HideInfo();
            confirmButton.style.display = DisplayStyle.None;
        };

        PlayerTurnEntity.Instance.OnStatChanged += (_, _, _) => UpdateUI();
    }

    public void UpdateUI() {
        healthLabel.text = $"{PlayerTurnEntity.Instance.GetStat(GameManager.HEALTH_STAT)}";
        apLabel.text = $"{PlayerTurnEntity.Instance.GetStat(GameManager.AP_STAT)}";
        refreshLabel.text = $"{PlayerTurnEntity.Instance.GetStat(GameManager.REFRESH_STAT)}";
        movementLabel.text = $"{PlayerTurnEntity.Instance.GetStat(GameManager.MOVEMENT_STAT)}";
        meteorLabel.text = $"{PlayerTurnEntity.Instance.GetStat(GameManager.METEOR_STAT)}";

        // update initiative list
        initiativeList.Clear();
        foreach(var entity in InitiativeManager.Instance.initiativeOrder) {
            bool isCurrent = entity == InitiativeManager.Instance.initiativeOrder[InitiativeManager.Instance.initiativeIndex];

            VisualElement bg = new VisualElement();
            bg.AddToClassList(isCurrent ? "OnTurnBG" : "OffTurnBG");
            VisualElement entityElement = new VisualElement();
            entityElement.AddToClassList("InitiativePicture");
            entityElement.style.backgroundImage = new StyleBackground(entity.GetInitiativePicture());
            bg.Add(entityElement);

            initiativeList.Add(bg);
        }

        // update status text
        statusText.text = "";
        foreach (var status in PlayerTurnEntity.Instance.statuses) {
            statusText.text += status.Key.ToIcon() + (status.Value != 1 ? status.Value : "");
        }

    }

    public void ShowNotification(string message, Color color, float time = 3f) {
        notification.text = message;
        notification.style.display = DisplayStyle.Flex;
        notification.style.color = color;
        notificationDuration = time;
    }

    public void ShowOverlay(string instructionsText, Color color) {
        screenOverlay.style.display = DisplayStyle.Flex;
        instructions.text = instructionsText;
        instructions.style.color = color;
        screenOverlay.style.borderBottomColor = color;
        screenOverlay.style.borderTopColor = color;
        screenOverlay.style.borderLeftColor = color;
        screenOverlay.style.borderRightColor = color;
    }

    public void HideOverlay() {
        screenOverlay.style.display = DisplayStyle.None;
        instructions.text = "";
    }

    public void ShowInfo(string info) {
        infoPanel.style.display = DisplayStyle.Flex;
        infoText.text = info;
    }

    public void HideInfo() {
        infoPanel.style.display = DisplayStyle.None;
        infoText.text = "";
    }

    public void ShowCardList(CardPhase phase) {
        cardListPanel.Clear();
        List<Card> cardsToRender = new List<Card>();
        foreach (Card c in PlayerTurnEntity.Instance.hand) {
            if (c.phase != phase) continue;
            cardsToRender.Add(c);
        }
        // sort added cards by the cost of the card
        cardsToRender.Sort((a, b) => a.apCost.CompareTo(b.apCost));

        foreach (Card c in cardsToRender) {
            if (c.phase != phase) continue;

            VisualElement cardEntry = new VisualElement();
            cardEntry.AddToClassList("ListCardBG");
            Label label = new Label();
            label.AddToClassList("ListCardText");
            cardEntry.Add(label);
            label.text = $"({c.apCost}) {c.cardName}";
            Toggle useMeteor = new Toggle();
            if (c.HasKeyword(Game.KEYWORD_SpendMeteor)) {
                cardEntry.Add(useMeteor);
            }

            cardEntry.RegisterCallback<MouseEnterEvent>(evt => {
                ShowCard(c);
            });
            cardEntry.RegisterCallback<MouseLeaveEvent>(evt => {
                HideCard();
            });
            label.RegisterCallback<ClickEvent>(evt => {
                bool useMeteorValue = useMeteor != null && useMeteor.value;
                //Debug.Log($"Clicked on card {c.cardName}, useMeteor={useMeteorValue}");
                int meteorCost = 0;
                // the number at the end of spendMeteor indicates how many meteors must be spent
                if (c.HasKeyword(Game.KEYWORD_SpendMeteor) && useMeteorValue) {
                    meteorCost = c.GetKeywordValue(Game.KEYWORD_SpendMeteor);
                }
                PlayerTurnEntity.Instance.CardClick(c, meteorCost);
            });

            cardListPanel.Add(cardEntry);
        }
    }

    public void HideCardList() {
        cardListPanel.Clear();
    }

    public void ShowCard(Card c) {
        cardPanel.style.display = DisplayStyle.Flex;
        cardName.text = c.cardName;
        cardDescription.text = c.description;
        cardCostLabel.text = $"{c.apCost}";
    }

    public void HideCard() {
        cardPanel.style.display = DisplayStyle.None;
    }

    public void ShowTurnUI() {
        // The move button handles itself
        //moveButton.style.display = DisplayStyle.Flex;
        passTurnButton.style.display = DisplayStyle.Flex;
    }

    public void HideTurnUI() {
        //moveButton.style.display = DisplayStyle.None;
        passTurnButton.style.display = DisplayStyle.None;
    }


    private void Update() {
        if (notificationDuration > 0f) {
            notificationDuration -= Time.deltaTime;
            // make the text fade out in the last second
            if (notificationDuration < 1f) {
                float alpha = notificationDuration / 1f;
                notification.style.opacity = alpha;
            }
            else {
                notification.style.opacity = 1f;
            }
            if (notificationDuration <= 0f) {
                notification.style.display = DisplayStyle.None;
            }
        }
    }
}
