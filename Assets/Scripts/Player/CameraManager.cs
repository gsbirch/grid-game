using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    // The camera manager is responsible for moving the camera around the map.
    // It also makes sure the camera is in bounds, and handles moving the camera.

    // Singleton
    public static CameraManager Instance;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    InputAction moveAction;
    public float sensitivity = 1f;
    // Start is called before the first frame update
    void Start()
    {
        moveAction = Player.Instance.playerControls.Player.Move;
    }

    private void Update() {
        var move = moveAction.ReadValue<Vector2>();
        Camera.main.transform.position += new Vector3(move.x, move.y, 0) * Time.deltaTime * sensitivity;
    }
}
