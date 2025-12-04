using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("“G‚Ì€–SƒJƒEƒ“ƒg")]
    [SerializeField] int EnemyDeadCount;
    [SerializeField] PlayerStateScript state;
    public bool GameClearFlag = false;
    public bool playerDead =false;
    [SerializeField] string gameSceneName = "GameOverSeen";
    private void Awake()
    {
        state=GetComponent<PlayerStateScript>();
    }

    public void GameOver()
    {
            Debug.Log("GameOverSeen‚ğÄ¶‚µ‚Ü‚·");
            SceneManager.LoadScene(gameSceneName,LoadSceneMode.Single);
    }

    public void Update()
    {
        if (state.DeathFlag == true)
        {
            GameOver();
        }
    }
}
 