/*using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class ItemEditor : Editor {
    public override void OnInspectorGUI() {
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Get the target object
        Enemy spriteObject = (Enemy)target;

        // Display the sprite preview if a sprite is assigned
        if (spriteObject.initiativePicture != null) {
            GUILayout.Label("Sprite Preview:");

            // Get the texture of the sprite sheet
            Texture2D spriteSheet = spriteObject.initiativePicture.texture;

            // Get the rect of the individual sprite
            Rect spriteRect = spriteObject.initiativePicture.textureRect;

            // Create a cropped texture for preview
            Texture2D croppedTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
            croppedTexture.SetPixels(spriteSheet.GetPixels(
                (int)spriteRect.x,
                (int)spriteRect.y,
                (int)spriteRect.width,
                (int)spriteRect.height));
            croppedTexture.Apply();

            // Calculate scaling factor
            float scale = 2.0f; // Adjust this value for larger or smaller previews
            float previewWidth = spriteRect.width * scale;
            float previewHeight = spriteRect.height * scale;

            // Draw the scaled texture
            Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight, GUILayout.ExpandWidth(false));
            GUI.DrawTexture(previewRect, croppedTexture, ScaleMode.ScaleToFit);
        }
    }
}
*/