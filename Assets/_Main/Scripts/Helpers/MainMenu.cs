using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        //!!!
        GameObject.Find("LoadScreen").GetComponent<CanvasGroup>().LeanAlpha(0, 0.5f);
    }

    public void ContinueClickHandler()
    {
        GameObject.Find("LoadScreen").GetComponent<CanvasGroup>().LeanAlpha(1, 0.5f).setOnComplete(() => { SceneManager.LoadScene(0); });
    }

    public void NewGameClickHandler()
    {
        File.Delete(Application.dataPath + "/playerData.json");
        GameObject.Find("LoadScreen").GetComponent<CanvasGroup>().LeanAlpha(1, 0.5f).setOnComplete(() => { SceneManager.LoadScene(0); });
    }

    public void ExitClickHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Exit();
#endif
    }
}
