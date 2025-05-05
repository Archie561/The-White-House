using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LawActivity : MonoBehaviour
{
    [SerializeField] private LawPanel _lawPanelPrefab;
    [SerializeField] private RectTransform _mainTransform;
    [SerializeField] private LawHint _acceptHint;
    [SerializeField] private LawHint _declineHint;

    [SerializeField] [Range(0f, 1f)] private float _acceptThreshold;
    [SerializeField] [Range(0f, 1f)] private float _declineThreshold;

    private LawPanel _currentPanel;

    private Dictionary<InternalCharacteristic, int> _playerCharacteristics = new();
    private List<Law> _reminingLaws;
    private HashSet<int> _usedlaws = new();

    void Start()
    {
        //Hint initialization
        _acceptHint.gameObject.SetActive(true);
        _acceptHint.ParentTransform = _mainTransform;
        _acceptHint.ActivityRange = new Vector2(_acceptThreshold, 1.3f);

        _declineHint.gameObject.SetActive(true);
        _declineHint.ParentTransform = _mainTransform;
        _declineHint.ActivityRange = new Vector2(-0.3f, _declineThreshold);

        GeneratePlayerCharacteristics();
        InitializeLawsDatabase();

        PrepareNextLaw();
    }

    private void InitializeLawsDatabase()
    {
        var data = Resources.Load<TextAsset>("Story/KenRothwell/0/en/laws");
        var allLaws = JsonConvert.DeserializeObject<List<Law>>(data.text);

        _reminingLaws = allLaws.Where(law => !_usedlaws.Contains(law.lawID)).ToList();
    }

    void PrepareNextLaw()
    {
        if (TryGetNextLaw(out Law law))
            ShowLawPanel(law);

        else
            throw new Exception("Law is null!");
    }

    private bool TryGetNextLaw(out Law law)
    {
        var index = UnityEngine.Random.Range(0, _reminingLaws.Count);
        law = _reminingLaws[index];
        if (law == null) return false;

        _reminingLaws.RemoveAt(index);
        _usedlaws.Add(law.lawID);
        return true;
    }

    private void ShowLawPanel(Law law)
    {
        if (_currentPanel != null)
            DestroyLawPanel();

        _currentPanel = Instantiate(_lawPanelPrefab, _mainTransform);
        _currentPanel.Initialize(law);
        _currentPanel.DragEnded += HandleLawRelease;
        _currentPanel.AnimateAppearing(0.5f);

        _acceptHint.TargetTransform = _currentPanel.GetComponent<RectTransform>();
        _declineHint.TargetTransform = _currentPanel.GetComponent<RectTransform>();
    }

    private void HandleLawRelease()
    {
        var rectTransform = _currentPanel.GetComponent<RectTransform>();
        var normalizedPosition = (rectTransform.anchoredPosition.x + _mainTransform.rect.width * 0.5f) / _mainTransform.rect.width;

        if (normalizedPosition > _declineThreshold && normalizedPosition < _acceptThreshold)
            _currentPanel.MoveTo(new Vector2(0, rectTransform.anchoredPosition.y), 1f, allowInterruption: true);
        else
            ProceedLaw(normalizedPosition >= _acceptThreshold);
    }

    private void ProceedLaw(bool accepted)
    {
        foreach (var (characteristic, value) in _currentPanel.LawData.affectedCharacteristics)
            _playerCharacteristics[characteristic] += accepted ? value : -value;

        var rectTransform = _currentPanel.GetComponent<RectTransform>();
        _currentPanel.MoveTo(new Vector2(accepted ? Screen.width * 2 : -Screen.width * 2, rectTransform.anchoredPosition.y), 1f, onComplete: PrepareNextLaw);
    }

    private void DestroyLawPanel()
    {
        _currentPanel.DragEnded -= HandleLawRelease;
        Destroy(_currentPanel.gameObject);
    }



    //Debug methods
    private void GeneratePlayerCharacteristics()
    {
        foreach (InternalCharacteristic characteristic in Enum.GetValues(typeof(InternalCharacteristic)))
            _playerCharacteristics[characteristic] = 20;
    }
    private string CharacteristicsToText(Dictionary<InternalCharacteristic, int> characteristics, int modd = 1) => string.Join(", ", characteristics.Select(c => $"{c.Key}: {c.Value * modd}"));
    private void LogLawInfo(Law law)
    {
        print(
            $"id: {law.lawID}\n" +
            $"header: {law.header}\n" +
            $"main text: {law.mainText}\n" +
            $"detailed text: {law.detailedText}\n" +
            $"prepered by: {law.preparedBy}\n" +
            $"accepted characteristics: {CharacteristicsToText(law.affectedCharacteristics)}\n" +
            $"declined characteristics: {CharacteristicsToText(law.affectedCharacteristics, -1)}\n"
            );
    }
}