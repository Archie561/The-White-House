using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LawManager : MonoBehaviour
{
    /*---------------------------UI Elements---------------------------*/
    [SerializeField]
    private Image _document;
    [SerializeField]
    private TextMeshProUGUI _headerText;
    [SerializeField]
    private TextMeshProUGUI _mainText;
    [SerializeField]
    private TextMeshProUGUI _detailedText;
    [SerializeField]
    private TextMeshProUGUI _preparedByText;
    /*---------------------------End UI Section---------------------------*/

    private Law _currentLaw;

    private void Start()
    {
        TryGetNextLaw();
    }

    private void TryGetNextLaw()
    {
        GameManager.Instance.LawLockCheck();
        if (GameManager.Instance.IsLawLocked)
        {
            GameManager.Instance.LoadMainScene();
            return;
        }

        _currentLaw = GameManager.Instance.GetNextLaw();

        _headerText.text = _currentLaw.header;
        _mainText.text = _currentLaw.mainText;
        _detailedText.text = _currentLaw.detailedText;
        _preparedByText.text = _currentLaw.preparedBy;
    }

    public void LawClickHandler()
    {
        if (EventSystem.current.currentSelectedGameObject.CompareTag("ChoiceOption1"))
        {
            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenApplied);
        }
        else
        {
            GameManager.Instance.UpdateCharacteristics(_currentLaw.characteristicsUpdateWhenDeclined);
        }

        DataManager.PlayerData.lawID++;
        DataManager.SaveData();

        TryGetNextLaw();
    }

    public void BackClickHandler()
    {
        GameManager.Instance.LoadMainScene();
    }
}
