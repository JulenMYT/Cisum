using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem damageParticles;

    public void TriggerDamageEffect(Vector3 hitPoint)
    {
        if (damageParticles != null)
        {
            damageParticles.transform.position = hitPoint;
            damageParticles.Play();
        }
    }
}
