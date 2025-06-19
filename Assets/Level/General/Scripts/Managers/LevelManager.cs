using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _loadingImageCanvasGroup;

    private const float LOADING_SCREEN_ANIMATION_TIME = 0.8f;

    public static LevelManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //TEMP
            if (GameDataManager.ActivePresident == null)
                GameDataManager.Load("KenRothwell");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        var sceneLoading = SceneManager.LoadSceneAsync(sceneName);
        sceneLoading.allowSceneActivation = false;

        StartCoroutine(LoadSceneCoroutine());

        _canvas.gameObject.SetActive(true);
        _loadingImageCanvasGroup.DOFade(1, LOADING_SCREEN_ANIMATION_TIME).OnComplete(() => sceneLoading.allowSceneActivation = true);

        IEnumerator LoadSceneCoroutine()
        {
            while (!sceneLoading.isDone || !sceneLoading.allowSceneActivation) yield return null;;
            _loadingImageCanvasGroup.DOFade(0, LOADING_SCREEN_ANIMATION_TIME).OnComplete(() => _canvas.gameObject.SetActive(false));
        }
    }
}