using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public void FadeIn()
    {
        animator.SetBool("fadeOut", false);
    }
    public void FadeOut()
    {
        animator.SetBool("fadeOut", true);
    }
}
