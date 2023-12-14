using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueClickHandler()
    {
        SceneManager.LoadScene(1);
    }

    public void NewGameClickHandler()
    {
        //File.Delete(Application.dataPath + "/playerData.json");
        File.Delete(System.IO.Directory.GetCurrentDirectory() + "/playerData.json");
        SceneManager.LoadScene(1);
    }

    public void ExitClickHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
