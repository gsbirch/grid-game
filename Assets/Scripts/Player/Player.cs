using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Tilemaps;


// The player class refers to things the user does directly
// It does NOT refer to the player's character in the game
public class Player : MonoBehaviour
{
    //Singleton
    public static Player Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        playerControls = new PlayerControls();
        playerControls.Enable();

    }

    public TileBase select;

    Vector3Int prevCellPosition = Utilities.NULLVEC;

    public event Action<Vector3Int> OnCellClicked;
    public event Action<Vector3Int> OnCellHovered;

    public PlayerControls playerControls;

    // Cache the game state for pausing purposes
    public GameState cachedGameState;

    private void Start() {
        HUDBehaviour.Instance.ShowNotification("Welcome to Grid Game! Press the ? to learn how to play.", Color.black, 1.5f);

        playerControls.Player.Exit.performed += ctx => {
            switch (GameManager.Instance.GameState) {
                case GameState.PlayerTurnDefault:
                    GameManager.Instance.GameState = GameState.Paused;
                    cachedGameState = GameState.PlayerTurnDefault;
                    break;
                case GameState.PlayerTurnMovement:
                    GameManager.Instance.GameState = GameState.PlayerTurnDefault;
                    break;
                case GameState.PlayerTurnTarget:
                    GameManager.Instance.GameState = GameState.PlayerTurnDefault;
                    break;
                case GameState.PlayerTurnResponse:
                    // maybe do nothing tbh?
                    break;
                case GameState.Paused:
                    GameManager.Instance.GameState = cachedGameState;
                    break;
                case GameState.UI:
                    GameManager.Instance.GameState = GameState.PlayerTurnDefault;
                    break;
                default:
                    break;
            }
        };

        playerControls.Player.DEBUG_MENU.performed += ctx => {
            PauseMenu.Instance.ToggleDebugPanel();
        };

        
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.GameState == GameState.Paused) {
            if (prevCellPosition != Utilities.NULLVEC) {
                TilemapManager.Instance.SetTile(TileLayer.Select, prevCellPosition, null);
                prevCellPosition = Utilities.NULLVEC;
            }
            return;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = TilemapManager.Instance.WorldToCell(mousePosition);

        // Check if the mouse is within the bounds of the tilemap
        if (TilemapManager.Instance.GetTile(TileLayer.Ground, cellPosition) == null) {
            // Clear previous selection if out of bounds
            if (prevCellPosition != Utilities.NULLVEC) TilemapManager.Instance.SetTile(TileLayer.Select, prevCellPosition, null);
            prevCellPosition = Utilities.NULLVEC;

            // NEW - invoke hover with null vector to indicate no cell is hovered
            OnCellHovered?.Invoke(Utilities.NULLVEC);
            HUDBehaviour.Instance.DEBUG_CurrentHex.text = $"Hex: None";
            return;
        }
        
        if (cellPosition != prevCellPosition) {
            // Clear previous selection
            if (prevCellPosition != Utilities.NULLVEC) TilemapManager.Instance.SetTile(TileLayer.Select, prevCellPosition, null);
            prevCellPosition = cellPosition;
        }
    
        TilemapManager.Instance.SetTile(TileLayer.Select, cellPosition, select);

        OnCellHovered?.Invoke(cellPosition);
        HUDBehaviour.Instance.DEBUG_CurrentHex.text = $"Hex: {cellPosition.x}, {cellPosition.y}, {cellPosition.z}";

        if (Input.GetMouseButtonDown(0))
        {
            OnCellClicked?.Invoke(cellPosition);
        }
    }

    
}
