using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardRecoverManager : MonoBehaviour
{
    VisualElement root;
    VisualElement cardList;
    VisualElement cardPanel;
    Label cardName;
    Label cardDescription;
    Label cardCostLabel;
    Label title;

    // Singleton
    public static CardRecoverManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    int amountRemaining = 0;

    // Start is called before the first frame update
    void Start() {
        root = GetComponent<UIDocument>().rootVisualElement;
        cardList = root.Q<VisualElement>("CardList");
        cardPanel = root.Q<VisualElement>("CardInfoPanel");
        cardName = root.Q<Label>("CardName");
        cardDescription = root.Q<Label>("CardDescription");
        cardCostLabel = root.Q<Label>("CardCostLabel");
        title = root.Q<Label>("Title");

        root.style.display = DisplayStyle.None;
        GameManager.Instance.OnGameStateChanged += (previousState, newState) => {
            if (newState == GameState.PlayerTurnCardRecovery) return;
            root.style.display = DisplayStyle.None;
        };
    }

    public void InitalizeCardRecovery(int amount) {
        GameManager.Instance.GameState = GameState.PlayerTurnCardRecovery;
        Debug.Log("initialzing with amount " + amount);
        root.style.display = DisplayStyle.Flex;
        amountRemaining = Mathf.Min(amount, PlayerTurnEntity.Instance.discard.Count);
        
        ShowCardList();
    }

    public void ShowCardList() {
        cardList.Clear();
        if (amountRemaining == 1) {
            title.text = $"Choose a card to recover:";
        }
        else {
            title.text = $"Choose {amountRemaining} cards to recover:";
        }
        
        foreach (Card c in PlayerTurnEntity.Instance.discard) {
            if (c.HasKeyword("NoRecover")) continue;

            VisualElement cardEntry = new VisualElement();
            cardEntry.AddToClassList("ListCardBG");
            Label label = new Label();
            label.AddToClassList("ListCardText");
            cardEntry.Add(label);
            label.text = $"({c.apCost}) {c.cardName}";

            cardEntry.RegisterCallback<MouseEnterEvent>(evt => {
                ShowCard(c);
            });
            cardEntry.RegisterCallback<MouseLeaveEvent>(evt => {
                HideCard();
            });
            cardEntry.RegisterCallback<ClickEvent>(evt => {
                OnCardClick(c);
            });

            cardList.Add(cardEntry);
        }
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

    public void OnCardClick(Card c) {
        PlayerTurnEntity.Instance.RecoverCard(c);
        amountRemaining--;
        if (amountRemaining <= 0) {
            GameManager.Instance.GameState = GameState.PlayerTurnDefault;
            return;
        }
        ShowCardList();
    }
}
