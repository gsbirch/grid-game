using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectArrowState {
    Valid,
    Selected,
    None
}

public class SelectArrow : MonoBehaviour
{
    public Sprite validArrow;
    public Sprite selectedArrow;
    public float yMovement = 0.25f;
    public float yMovementInterval = 2f;

    SelectArrowState currentState = SelectArrowState.None;
    Vector3 defaultPosition;

    private void Start() {
        defaultPosition = transform.localPosition;
        SetState(SelectArrowState.None);

        GameManager.Instance.OnGameStateChanged += OnStateChange;
    }

    public void SetState(SelectArrowState state) {
        transform.localPosition = defaultPosition;
        switch (state) {
            case SelectArrowState.Valid:
                GetComponent<SpriteRenderer>().sprite = validArrow;
                break;
            case SelectArrowState.Selected:
                GetComponent<SpriteRenderer>().sprite = selectedArrow;
                break;
            case SelectArrowState.None:
                GetComponent<SpriteRenderer>().sprite = null;
                break;
        }
        currentState = state;
    }

    private void Update() {
        if (currentState != SelectArrowState.Valid) return;

        int toggle = (int)(Time.time / yMovementInterval) % 2;

        float yDisplacement = (toggle == 0) ? 0f : yMovement;
        transform.localPosition = defaultPosition + new Vector3(0f, yDisplacement, 0f);
    }

    void OnStateChange(GameState prevState, GameState newState) {
        SetState(SelectArrowState.None);
    }

    private void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnGameStateChanged -= OnStateChange;
        }
    }
}
