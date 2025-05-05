using UnityEngine;

class NewsManager : MonoBehaviour
{
    [SerializeField] private BlackoutScreen _blackoutScreen;
    [SerializeField] private BaseNewsPanel[] _newsPanelPrefabs;
    [SerializeField] private Transform _newsPanelsTransform;

    private const float NEWS_PANEL_ANIMATION_TIME = 0.8f;
    private const float NEWS_PANEL_ROTATION_RANGE = 3.0f;

    private readonly float NEWS_PANEL_OFFSCREEN_Y = Screen.height * -0.5f;

    private News[] _news;
    private BaseNewsPanel _currentNewsPanel;
    private PlayerDataManager _playerDataManager;

    private bool _isAnimationFinished = true;
    private int _newsIndex = 0;

    private void Start() => GameManager.Instance.OnGameStateChanged += OnStateChanged;

    private void OnDestroy() => GameManager.Instance.OnGameStateChanged -= OnStateChanged;

    //Handles set up of the mechanic if the state is ActiveNews
    private void OnStateChanged(GameState state)
    {
        if (state == GameState.ActiveNews) SetUpMechanic();
    }

    //Initializes the news panel system if news array is not empty
    private void SetUpMechanic()
    {
        _news = ServiceLocator.GetService<ChapterDataManager>().GetNews();
        if (_news.Length == 0)
        {
            GameManager.Instance.ChangeGameState(GameState.Default);
            return;
        }

        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        _blackoutScreen.SetPosition(BlackoutScreenPosition.overBudgetBox);
        _blackoutScreen.FadeIn();
        LoadNextNews();
    }

    //Creates the next panel and initializes it with data. Skips if the condition is not met. Exits if all news is displayed
    private void LoadNextNews()
    {
        if (_newsIndex >= _news.Length)
        {
            Exit();
            return;
        }

/*        if (!_playerDataManager.IsConditionMet(_news[_newsIndex].condition))
        {
            _newsIndex++;
            LoadNextNews();
            return;
        }*/

        _currentNewsPanel = Instantiate(_newsPanelPrefabs[_news[_newsIndex].newsPaperType - 1], _newsPanelsTransform);
        _currentNewsPanel.Initialize(_news[_newsIndex]);
        _currentNewsPanel.OnNewsClicked += NewsClickHandler;
        ShowNewsPanel();
    }

    //Displays the news panel with an animation
    private void ShowNewsPanel()
    {
        _isAnimationFinished = false;

        _currentNewsPanel.transform.localScale = Vector3.zero;
        _currentNewsPanel.transform.rotation = Quaternion.Euler(
            _currentNewsPanel.transform.rotation.eulerAngles.x,
            _currentNewsPanel.transform.rotation.eulerAngles.y,
            Random.Range(-NEWS_PANEL_ROTATION_RANGE, NEWS_PANEL_ROTATION_RANGE));

        _currentNewsPanel.transform.LeanScale(Vector3.one, NEWS_PANEL_ANIMATION_TIME)
            .setEaseOutQuart().setOnComplete(() => _isAnimationFinished = true);
    }

    //Handles the click event to hide the current news panel and show the next one.
    private void NewsClickHandler()
    {
        if (!_isAnimationFinished) return;
        _isAnimationFinished = false;

        AudioManager.Instance.PlaySFX("document");
        _currentNewsPanel.transform.LeanMoveY(NEWS_PANEL_OFFSCREEN_Y, NEWS_PANEL_ANIMATION_TIME).setEaseOutQuart().setOnComplete(() => {
            _isAnimationFinished = true;

            _newsIndex++;
            _currentNewsPanel.OnNewsClicked -= NewsClickHandler;
            Destroy(_currentNewsPanel.gameObject);
            LoadNextNews();
        });
    }

    //Exits the news manager and resets the game state
    private void Exit() => _blackoutScreen.FadeOut(() => GameManager.Instance.ChangeGameState(GameState.Default));
}