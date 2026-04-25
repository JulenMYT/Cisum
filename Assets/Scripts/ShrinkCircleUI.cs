using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShrinkCircleUI : MonoBehaviour
{
    [SerializeField] private Transform bigCircle;
    [SerializeField] private Transform smallCircle;
    [SerializeField] private float shrinkDuration = 2.0f;
    [SerializeField] private float maxShrinkScale = 3.0f;
    [SerializeField] private float minShrinkScale = 0.1f;
    public bool isActive = false;
    private Coroutine shrinkRoutine;

    public delegate void ShootTimingDelegate(float success);
    public event ShootTimingDelegate OnShootAttempt;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();

        controls.CombatUI.Action.started += OnShootStarted;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void ShowUI()
    {
        isActive = true;
        gameObject.SetActive(true);
        shrinkRoutine = StartCoroutine(ShrinkAndLoop());
    }

    private void HideUI()
    {
        isActive = false;
        gameObject.SetActive(false);
        if (shrinkRoutine != null)
        {
            StopCoroutine(shrinkRoutine);
        }
    }

    private IEnumerator ShrinkAndLoop()
    {
        Vector3 originalScale = bigCircle.localScale;

        while (isActive)
        {
            float elapsedTime = 0f;
            while (elapsedTime < shrinkDuration)
            {
                float t = elapsedTime / shrinkDuration;
                bigCircle.localScale = Vector3.Lerp(Vector3.one * maxShrinkScale, Vector3.one * minShrinkScale, t);
                elapsedTime += Time.deltaTime;
                bigCircle.Rotate(0f, 0f, 90f * Time.deltaTime);
                yield return null;
            }

            bigCircle.localScale = Vector3.one * minShrinkScale;

            elapsedTime = 0f;
            while (elapsedTime < shrinkDuration)
            {
                float t = elapsedTime / shrinkDuration;
                bigCircle.localScale = Vector3.Lerp(Vector3.one * minShrinkScale, Vector3.one * maxShrinkScale, t);
                elapsedTime += Time.deltaTime;
                bigCircle.Rotate(0f, 0f, 90f * Time.deltaTime);
                yield return null;
            }

            bigCircle.localScale = originalScale;
        }
    }

    private float CheckOverlap()
    {
        float bigRadius = bigCircle.localScale.x / 2;
        float smallRadius = smallCircle.localScale.x / 2;

        return Mathf.Abs(bigRadius - smallRadius);
    }
    private void OnShootStarted(InputAction.CallbackContext context)
    {
        float success = CheckOverlap();
        OnShootAttempt?.Invoke(success);
        HideUI();
    }
}
