using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControlGameOver : MonoBehaviour
{
    void Start()
    {
        PlayerBehaviour player = GetComponent<PlayerBehaviour>();
        player.playerData.currentHealth = player.playerData.maxHealth;
        StartCoroutine(WaitALittle());
    }

    private IEnumerator WaitALittle()
    {
        yield return new WaitForSeconds(2f);
        yield return InputManager.Instance.WaitForInput();
        SceneManager.LoadScene("SampleScene");
    }
}
