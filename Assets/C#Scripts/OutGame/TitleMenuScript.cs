using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuScript : MonoBehaviour
{
    //[SerializeField] string gameSceneName = "Game";
    //public void StartButton()
    //{
    //    Debug.Log("Game_Start");
    //    SceneManager.LoadScene("Game");
    //}
    [SerializeField] string gameSceneName = "Game";

    public void StartButton()
    {
        Debug.Log("[Title] Game_Start");

        // シーンがビルドに入っている & 名前一致かをチェック
        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError($"[Title] Scene '{gameSceneName}' is NOT in Build Settings or name mismatch.");
            return;
        }
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
    public void OptionButton()
    {
        Debug.Log("Option Selected(まだ未実装)");
    }

    public void QuitButton()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

}
