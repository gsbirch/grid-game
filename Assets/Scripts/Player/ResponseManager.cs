using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// When a player takes damage, they have the opportunity to respond with a response card
// They may only play one response card per damage instance
// If they don't want to play a card, they can click space to quickly skip
// An attack must proc before a response can be played, so its assumed that
// in this phase, the attack procs
public class ResponseManager : MonoBehaviour {
    // Singleton
    public static ResponseManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

    }

    Card currentCard = null;

    int amount;
    ITargettable actualSource;

    ITargettable Source {
        get => actualSource;
        set {
            //if (value == null)
                //Debug.Log("setting source to null");
            //else
                //Debug.Log("Setting response source to " + ((MonoBehaviour)value).name);
            actualSource = value;
        }

    }

    private void Start() {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnGameStateChanged(GameState oldState, GameState newState) {
        if (oldState != GameState.PlayerTurnResponse) return;
        CleanupResponsePhase();
    }

    public void InitializeResponseSelection(int amount, ITargettable source) {
        // Ensure no dangling state from a previous response
        CleanupResponsePhase();

        //Debug.Log("Initializing response selection, responding to " + ((MonoBehaviour)source).name);
        GameManager.Instance.GameState = GameState.PlayerTurnResponse;
        this.amount = amount;
        this.Source = source;

        Player.Instance.playerControls.Player.NextButton.performed += Skip;
        HUDBehaviour.Instance.confirmButton.clicked += ConfirmClicked;

        string instructionsText = $"You will take {amount} damage, you may play one response card. Press SPACE to skip.";
        HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.red);
        HUDBehaviour.Instance.ShowCardList(CardPhase.Response);

        HUDBehaviour.Instance.confirmButton.style.display = DisplayStyle.Flex;
        HUDBehaviour.Instance.confirmButton.text = "Skip";
    }


    public void SelectCard(Card c) {
        string instructionsText = $"You will take {amount} damage, respond with {c.cardName}?. Press SPACE to skip.";
        HUDBehaviour.Instance.ShowOverlay(instructionsText, Color.green);
        HUDBehaviour.Instance.confirmButton.text = "Confirm";
        currentCard = c;
    }

    void Skip(InputAction.CallbackContext ctx) {
        // Skip response phase
        ConfirmResponse(null);
    }

    void ConfirmClicked() {
        ConfirmResponse(currentCard);
    }

    // c can be null to skip
    public void ConfirmResponse(Card c) {
        if (Source == null) {
            Debug.LogWarning("ConfirmResponse called but Source is null — likely double trigger");
            return;
        }

        //Debug.Log("Confirming response to " + ((MonoBehaviour)Source).name);

        if (amount == int.MinValue) {
            Debug.LogError("ConfirmResponse called at incorrect time");
            return;
        }

        if (c != null) {
            PlayerTurnEntity.Instance.PlayCard(c, new() { Source });
        }
        var prevSource = (EnemyTurnEntity) Source;
        int prevAmount = amount;
        // Clean up, but before the enemy continues its turn
        CleanupResponsePhase();

        // Only continue if source is still valid
        // we need to use the previous source because cleanup will set it to null
        if (prevSource != null) {
            PlayerTurnEntity.Instance.ProcessDamage(prevAmount, prevSource, true);
            prevSource.ContinueTurn();
        }
    }


    void CleanupResponsePhase() {
        // Unsubscribe listeners
        Player.Instance.playerControls.Player.NextButton.performed -= Skip;
        HUDBehaviour.Instance.confirmButton.clicked -= ConfirmClicked;

        // Reset UI
        HUDBehaviour.Instance.confirmButton.text = "Confirm";

        // Reset state
        currentCard = null;
        amount = int.MinValue;
        Source = null;
    }

}
