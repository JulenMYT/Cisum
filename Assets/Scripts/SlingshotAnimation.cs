using UnityEngine;

public class SlingshotAnimation : MonoBehaviour
{
    public void EnableObject()
    {
        gameObject.SetActive(true);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}