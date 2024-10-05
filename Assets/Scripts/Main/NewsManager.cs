using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsManager : MonoBehaviour
{
    /*--------------------------UI SECTION--------------------------*/
    [SerializeField]
    private CanvasGroup _blackBackground;

    [SerializeField]
    private GameObject _newsPanels;

    [SerializeField]
    private Image _newsLiberty;
    [SerializeField]
    private Image _newsPatriot;
    [SerializeField]
    private Image _newsPulse;
    [SerializeField]
    private Image _photo;

    [SerializeField]
    private TextMeshProUGUI _libertyHeaderText;
    [SerializeField]
    private TextMeshProUGUI _patriotHeaderText;
    [SerializeField]
    private TextMeshProUGUI _pulseHeaderText;
    [SerializeField]
    private TextMeshProUGUI _photoText;

    [SerializeField]
    private TextMeshProUGUI _libertyDetailedText;
    [SerializeField]
    private TextMeshProUGUI _patriotDetailedText;
    [SerializeField]
    private TextMeshProUGUI _pulseDetailedText;

    [SerializeField]
    private Image _libertyImage;
    [SerializeField]
    private Image _photoImage;
    /*-------------------------END UI SECTION-------------------------*/


    /*------------------ANIMATION PARAMETERS SECTION------------------*/
    private bool _isAnimationFinished;

    private float _blackBackgroundAnimationTime = 0.8f;
    private float _newsPanelAnimationTime = 0.8f;
    private float _newsPanelRotationRange = 3.0f;
    /*----------------END ANIMATION PARAMETERS SECTION----------------*/

    private News[] _news;

    private Image _currentNewsPanel;

    private int _newsIndex;

    private void OnEnable()
    {
        _news = GameManager.Instance.GetNews();

        if (_news.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        _isAnimationFinished = true;
        _newsPanels.SetActive(true);

        _newsIndex = 0;
        _blackBackground.gameObject.SetActive(true);
        _blackBackground.LeanAlpha(1, _blackBackgroundAnimationTime);

        GetNextNews();
    }

    private void GetNextNews()
    {
        if (_newsIndex >= _news.Length)
        {
            _blackBackground.LeanAlpha(0, _blackBackgroundAnimationTime).setOnComplete(() => {
                _isAnimationFinished = true;
                _newsPanels.gameObject.SetActive(false);
                _blackBackground.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
            return;
        }

        if (!GameManager.Instance.IsConditionMet(_news[_newsIndex].condition))
        {
            _newsIndex++;
            GetNextNews();
            return;
        }

        switch (_news[_newsIndex].newsPaperType)
        {
            case 1:
                LoadLiberty();
                break;
            case 2:
                LoadPatriot();
                break;
            case 3:
                LoadPulse();
                break;
            case 4:
                LoadPhoto();
                break;
            default:
                Debug.LogError("Unknown newspaper type!");
                break;
        }
    }

    public void NewsClickHandler()
    {
        if (_isAnimationFinished)
        {
            _isAnimationFinished = false;
            AudioManager.Instance.PlaySFX("document");

            _currentNewsPanel.transform.LeanMoveY(-Screen.height / 2, _newsPanelAnimationTime).setEaseOutQuart().setOnComplete(() => {

                _isAnimationFinished = true;
                _currentNewsPanel.gameObject.SetActive(false);

                //to return the panel to its place
                _currentNewsPanel.transform.localPosition = Vector2.zero;

                _newsIndex++;
                GetNextNews();
            });
        }
    }

    private void ShowNewsPanel()
    {
        _isAnimationFinished = false;

        Vector2 panelDefaultScale = _currentNewsPanel.transform.localScale;
        _currentNewsPanel.transform.localScale = Vector2.zero;

        _currentNewsPanel.gameObject.SetActive(true);
        _currentNewsPanel.transform.eulerAngles = new Vector3(_currentNewsPanel.transform.eulerAngles.x, _currentNewsPanel.transform.eulerAngles.y, Random.Range(-_newsPanelRotationRange, _newsPanelRotationRange));
        _currentNewsPanel.transform.LeanScale(panelDefaultScale, _newsPanelAnimationTime).setEaseOutQuart().setOnComplete(() => { _isAnimationFinished = true; });
    }

    private void LoadLiberty()
    {
        _libertyImage.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + _news[_newsIndex].imageName);
        _libertyHeaderText.text = _news[_newsIndex].headerText;
        _libertyDetailedText.text = _news[_newsIndex].detailedText;

        _currentNewsPanel = _newsLiberty;
        ShowNewsPanel();
    }

    private void LoadPatriot()
    {
        _patriotHeaderText.text = _news[_newsIndex].headerText;
        _patriotDetailedText.text = _news[_newsIndex].detailedText;

        _currentNewsPanel = _newsPatriot;
        ShowNewsPanel();
    }

    private void LoadPulse()
    {
        _pulseHeaderText.text = _news[_newsIndex].headerText;
        _pulseDetailedText.text = _news[_newsIndex].detailedText;

        _currentNewsPanel = _newsPulse;
        ShowNewsPanel();
    }

    private void LoadPhoto()
    {
        _photoImage.sprite = Resources.Load<Sprite>("Textures/NewsImages/" + _news[_newsIndex].imageName);
        _photoText.text = _news[_newsIndex].headerText;

        _currentNewsPanel = _photo;
        ShowNewsPanel();
    }
}
