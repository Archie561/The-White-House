using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class LawManager : MonoBehaviour
{
    [SerializeField] private LawPanel _lawPanelPrefab;
    [SerializeField] private RectTransform _mainTransform;
    [SerializeField] private LawHint _acceptHint;
    [SerializeField] private LawHint _declineHint;
    [SerializeField] private CanvasGroup _allLawsFinishedText;

    public UnityEvent<Characteristic, int> OnCharacteristicModified;

    private const float ANIMATION_DURATION = 0.5f;
    private const float DECLINE_THRESHOLD = 0.45f;
    private const float ACCEPT_THRESHOLD = 0.55f;

    private LawPanel _currentLawPanel;

    void Start() => HandleNextLawDisplay();

    private void HandleNextLawDisplay()
    {
        if (GameDataManager.TryGetNextLaw(out var law))
        {
            ShowLawPanel(law);
            return;
        }

        //add UI
        _allLawsFinishedText.gameObject.SetActive(true);
        _allLawsFinishedText.DOFade(1, 1f).SetEase(Ease.OutQuart);
    }

    private void ShowLawPanel(Law law)
    {
        if (_currentLawPanel != null)
            DestroyLawPanel();

        AudioManager.Instance.PlaySFX(SFXType.Document);

        _currentLawPanel = Instantiate(_lawPanelPrefab, _mainTransform);
        _currentLawPanel.Initialize(law);
        _currentLawPanel.DragEnded += OnLawPanelReleased;
        _currentLawPanel.AnimateAppearing(ANIMATION_DURATION);

        _acceptHint.TargetTransform = _currentLawPanel.GetComponent<RectTransform>();
        _declineHint.TargetTransform = _currentLawPanel.GetComponent<RectTransform>();
    }

    private void OnLawPanelReleased()
    {
        var rectTransform = _currentLawPanel.GetComponent<RectTransform>();
        var normalizedPosition = (rectTransform.anchoredPosition.x + _mainTransform.rect.width * 0.5f) / _mainTransform.rect.width;

        if (normalizedPosition > DECLINE_THRESHOLD && normalizedPosition < ACCEPT_THRESHOLD)
            _currentLawPanel.MoveTo(new Vector2(0, rectTransform.anchoredPosition.y), ANIMATION_DURATION, allowInterruption: true);
        else
            ProcessLawDecision(accepted: normalizedPosition >= ACCEPT_THRESHOLD);
    }

    private void ProcessLawDecision(bool accepted)
    {
        AudioManager.Instance.PlaySFX(accepted ? SFXType.Accept : SFXType.Decline);

        GameDataManager.SaveLawDecision(_currentLawPanel.LawData, accepted);

        foreach (var (characteristic, value) in _currentLawPanel.LawData.affectedCharacteristics)
            OnCharacteristicModified?.Invoke(characteristic, accepted ? value : -value);

        var rectTransform = _currentLawPanel.GetComponent<RectTransform>();
        var targetPosition = new Vector2(_mainTransform.rect.width * (accepted ? 1 : -1), rectTransform.anchoredPosition.y);
        _currentLawPanel.MoveTo(targetPosition, ANIMATION_DURATION, onComplete: HandleNextLawDisplay);
    }

    private void DestroyLawPanel()
    {
        _currentLawPanel.DragEnded -= OnLawPanelReleased;
        Destroy(_currentLawPanel.gameObject);
    }

    public void ExitActivity()
    {
        GameDataManager.Save();
        AudioManager.Instance.PlaySFX(SFXType.Click);
        LevelManager.Instance.LoadScene("Main");
    }
}