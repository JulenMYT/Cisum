using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputActionReference confirmAction;

    private bool isWaitingForInput = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        confirmAction.action.Enable();
        confirmAction.action.performed += OnConfirmInput;
    }

    void OnDisable()
    {
        confirmAction.action.Disable();
        confirmAction.action.performed -= OnConfirmInput;
    }

    private void OnConfirmInput(InputAction.CallbackContext context)
    {
        if (isWaitingForInput)
        {
            Debug.Log("Merci");

            isWaitingForInput = false;
        }
    }

    public IEnumerator WaitForInput()
    {
        Debug.Log("Please push something");
        if (!confirmAction.action.enabled)
        {
            confirmAction.action.Enable();
        }
        isWaitingForInput = true;

        while (isWaitingForInput)
        {
            yield return null;
        }
    }
}
