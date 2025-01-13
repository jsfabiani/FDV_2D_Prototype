using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] GameObject gameOverUI, victoryUI;
    [SerializeField] GameObject[] notEndGameUIObjects;
    // Start is called before the first frame update
    void Start()
    {
        BossController.bossDefeated += Victory;
        PlayerCharacter.destroyPlayer += PlayerDefeated;
        gameOverUI.SetActive(false);
        victoryUI.SetActive(false);

    }


    void Victory()
    {
        foreach(GameObject o in notEndGameUIObjects)
        {
            o.SetActive(false);
        }
        victoryUI.SetActive(true);
        Invoke("CloseGame", 5);
    }

    void PlayerDefeated()
    {
        foreach(GameObject o in notEndGameUIObjects)
            {
                o.SetActive(false);
            }
            gameOverUI.SetActive(true);
        Invoke("CloseGame", 5);
    }

    void CloseGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }



}
