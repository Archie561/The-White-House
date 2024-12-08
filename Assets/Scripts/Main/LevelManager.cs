using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _loadingImageCanvasGroup;
    [SerializeField] private CanvasGroup _chapterTitleCanvasGroup;
    [SerializeField] private TextMeshProUGUI _monthText;
    [SerializeField] private TextMeshProUGUI _presidentNameText;

    private const float LOADING_SCREEN_ANIMATION_TIME = 0.8f;
    private const float CHAPTER_TITLE_ANIMATION_TIME = 3.0f;

    public static LevelManager Instance {  get; private set; }

    private void Awake()
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

    /// <summary>
    /// Asynchronously loads the scene
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <param name="presidenName">Name of the president to show in a chapter title. If null, chapter title won't be displayed</param>
    /// <param name="monthName">Name of the month to show in a chapter title. If null, chapter title won't be displayed</param>
    public void LoadScene(string sceneName, string presidenName = null, string monthName = null)
    {
        var sceneLoading = SceneManager.LoadSceneAsync(sceneName);
        sceneLoading.allowSceneActivation = false;

        StartCoroutine(LoadSceneCoroutine());

        //Fade in animation
        _canvas.gameObject.SetActive(true);
        _loadingImageCanvasGroup.LeanAlpha(1, LOADING_SCREEN_ANIMATION_TIME).setOnComplete(() => sceneLoading.allowSceneActivation = true);

        IEnumerator LoadSceneCoroutine()
        {
            while (!sceneLoading.isDone || !sceneLoading.allowSceneActivation) yield return null;

            //Fade out animation depends on whether it's need to show the chapter title or not
            if (presidenName != null && monthName != null) ShowChapterTitle(presidenName, monthName);
            else _loadingImageCanvasGroup.LeanAlpha(0, LOADING_SCREEN_ANIMATION_TIME).setOnComplete(() => _canvas.gameObject.SetActive(false));
        }
    }

    private void ShowChapterTitle(string presidenName, string monthName)
    {
        _presidentNameText.text = presidenName;
        _monthText.text = monthName;

        _chapterTitleCanvasGroup.gameObject.SetActive(true);
        _chapterTitleCanvasGroup.LeanAlpha(1, LOADING_SCREEN_ANIMATION_TIME).setOnComplete(() =>
        {
            _loadingImageCanvasGroup.LeanAlpha(0, LOADING_SCREEN_ANIMATION_TIME).setDelay(CHAPTER_TITLE_ANIMATION_TIME).setOnComplete(() =>
            {
                _chapterTitleCanvasGroup.alpha = 0;
                _chapterTitleCanvasGroup.gameObject.SetActive(false);
                _canvas.gameObject.SetActive(false);
            });
        });
    }
}