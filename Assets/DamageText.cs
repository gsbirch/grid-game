using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour {
    TextMeshPro textMesh;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    private float lifetime = 0f;

    private void Update() {
        lifetime += Time.deltaTime;

        // float upwards movement
        transform.position += moveSpeed * Time.deltaTime * Vector3.up;

        // nonlinear fade (ease-out)
        Color color = textMesh.color;
        float fadeAmount = Mathf.Pow(lifetime * fadeSpeed, 2f); // squared for ease-out
        color.a = Mathf.Lerp(1f, 0f, fadeAmount);
        textMesh.color = color;

        if (color.a <= 0f) {
            Destroy(gameObject);
        }
    }

    public void Initialize(int damageAmount) {
        if (damageAmount == 0) {
            Destroy(gameObject);
            return;
        }
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = Mathf.Abs(damageAmount).ToString();
        textMesh.color = (damageAmount > 0) ? Color.green : Color.red;
    }
}

