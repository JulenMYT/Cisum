using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SkyboxManager : MonoBehaviour
{
    [Header("Background Colors")]
    [Tooltip("Hex color for level 1")]
    public string level1ColorHex = "FFCAF9";
    [Tooltip("Hex color for level 8")]
    public string level8ColorHex = "0F1730";

    private Camera mainCamera;
    private Color level1Color;
    private Color level8Color;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        // Convert hex colors to Unity Color objects
        if (!ColorUtility.TryParseHtmlString($"#{level1ColorHex}", out level1Color))
        {
            Debug.LogError($"Failed to parse hex color: {level1ColorHex}");
        }

        if (!ColorUtility.TryParseHtmlString($"#{level8ColorHex}", out level8Color))
        {
            Debug.LogError($"Failed to parse hex color: {level8ColorHex}");
        }
    }

    /// <summary>
    /// Updates the camera background color based on the player's level.
    /// </summary>
    /// <param name="level">The level of the player (1-8).</param>
    public void UpdateSkyboxColor(int level)
    {
        // Clamp level to range [1, 8]
        level = Mathf.Clamp(level, 1, 8);

        // Calculate the interpolation factor (0 at level 1, 1 at level 8)
        float t = (level - 1) / 7f;

        // Interpolate between the two colors
        Color interpolatedColor = Color.Lerp(level1Color, level8Color, t);

        // Apply the interpolated color to the camera's background
        mainCamera.backgroundColor = interpolatedColor;
    }
}
