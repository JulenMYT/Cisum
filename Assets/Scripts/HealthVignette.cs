using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthBasedVignette : MonoBehaviour
{
    [SerializeField]
    private Vignette vignette;
    void Start()
    {
        Volume volume = GetComponent<Volume>();
        if (volume == null || !volume.profile.TryGet<Vignette>(out vignette))
        {
            Debug.LogWarning("Vignette effect not found in the Global Volume profile.");
        }
    }
    public void UpdateHealth(CharacterStats playerStats)
    {
        if (vignette != null)
        {
            float healthPercent = Mathf.Clamp01((float)playerStats.currentHealth / playerStats.maxHealth);

            float intensity = Mathf.Lerp(0, 0.6f, 1 - healthPercent * 2);
            vignette.intensity.value = healthPercent < 0.5f ? intensity : 0;
        }
    }
}
