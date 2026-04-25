using UnityEngine;

public class RhythmChecker : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField]
    private AudioSource audioSource;

    [Header("Timing Settings")]
    [SerializeField]
    private float timingWindow = 0.1f;

    [Header("Music Data")]
    public float bpm;
    public float musicOffset;
    private float beatInterval;

    private float lastBeat;

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non assigné !");
            return;
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        //{
        //    float offset = CheckInputTiming();

        //    if (Mathf.Abs(offset) <= timingWindow)
        //    {
        //        Debug.Log($"En rythme ! Décalage : {offset:F3} secondes");
        //    }
        //    else if (offset < 0)
        //    {
        //        Debug.Log($"Trop tôt ! Décalage : {offset:F3} secondes");
        //    }
        //    else
        //    {
        //        Debug.Log($"Trop tard ! Décalage : {offset:F3} secondes");
        //    }
        //}
    }

    public float CheckInputTiming()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            Debug.LogWarning("AudioSource non disponible ou inactif !");
            return float.MaxValue;
        }

        int currentSample = audioSource.timeSamples;
        float currentAudioTime = (float)currentSample / audioSource.clip.frequency;

        float correctedTime = currentAudioTime - musicOffset;

        lastBeat = Mathf.Round(correctedTime / beatInterval) * beatInterval + musicOffset;

        float offset = currentAudioTime - lastBeat;

        return offset;
    }

    public bool CheckIfLastBeat()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            Debug.LogWarning("AudioSource non disponible ou inactif !");
            return false;
        }

        int currentSample = audioSource.timeSamples;
        float currentAudioTime = (float)currentSample / audioSource.clip.frequency;

        if (currentAudioTime - lastBeat < beatInterval * 1.2f)
        {
            return true;
        }
        return false;
    }

    public void UpdateData(float _bpm, float _offset)
    {
        bpm = _bpm;
        musicOffset = _offset;
        beatInterval = 60f / bpm;
    }
}
