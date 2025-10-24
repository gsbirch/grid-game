using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum GameState {
    PlayerTurnDefault,
    PlayerTurnMovement,
    PlayerTurnTarget,
    PlayerTurnResponse,
    EnemyTurn,
    UI,
    Paused,
}
public class GameManager : MonoBehaviour
{
    public static readonly string MOVEMENT_STAT = "Movement";
    public static readonly string HEALTH_STAT = "Health";
    public static readonly string AP_STAT = "AP";
    public static readonly string REFRESH_STAT = "Refresh";
    public static readonly string METEOR_STAT = "Meteor";

    public GameObject damageTextPrefab;

    //Singleton
    public static GameManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public event Action<GameState, GameState> OnGameStateChanged;

    public GameState GameState { get { return gameState; } set {
            // Even if we change to the same state, we still want to notify subscribers
            // Some of them may want to do something on any GameState update

            // reset any UI
            HUDBehaviour.Instance.HideOverlay();

            GameState previousState = gameState; // Store the previous state
            gameState = value; 
            if (gameState == GameState.Paused) {
                Time.timeScale = 0f;
            } else {
                Time.timeScale = 1f;
            }
            OnGameStateChanged?.Invoke(previousState, gameState); // Notify subscribers of the state change
        } 
    }
    private GameState gameState = GameState.PlayerTurnDefault;

    public void CreateDamageText(Vector3 position, int damageAmount) {
        var text = Instantiate(damageTextPrefab, position, Quaternion.identity);
        text.transform.position += new Vector3(0, 0.5f, 0); // slightly above the position
        // set z to -5 to be in front of everything
        text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y, -5);
        text.GetComponent<DamageText>().Initialize(damageAmount);
    }
}
