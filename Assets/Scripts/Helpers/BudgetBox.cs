using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays and animates the budget from the player's characteristics
/// </summary>
public class BudgetBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _budgetText;
    [SerializeField] private Color _defaultBudgetColor;
    [SerializeField] private Color _revenuesColor;
    [SerializeField] private Color _expensesColor;

    private const float BUDGET_TEXT_ANIMATION_TIME = 0.3f;
    private const int MAX_BUDGET_VALUE = 999999;

    private PlayerDataManager _playerDataManager;
    private Transform _budgetTextTransform;
    private float _budgetTextDefaultPositionY;
    private float _budgetTextTopPositionY;
    private float _budgetTextBottomPositionY;

    private void Start()
    {
        //_playerDataManager = ServiceLocator.GetService<PlayerDataManager>();

        _budgetTextTransform = _budgetText.transform;
        _budgetTextDefaultPositionY = _budgetTextTransform.position.y;
        _budgetTextTopPositionY = _budgetTextDefaultPositionY + _budgetText.rectTransform.rect.height * _budgetTextTransform.lossyScale.y;
        _budgetTextBottomPositionY = _budgetTextDefaultPositionY - _budgetText.rectTransform.rect.height * _budgetTextTransform.lossyScale.y;

        //initialise current budget & set color
        var budget = GameDataManager.PlayerData.GetCharacteristic(Characteristic.Budget);
        _budgetText.text = BudgetToString(budget);
        _budgetText.color = budget < 0 ? _expensesColor : _defaultBudgetColor;
    }

    /// <summary>
    /// Updates the budget to the current value from the characteristics with an animation
    /// </summary>
    /// <param name="value">Defines the color of the budget text. Value > 0 - revenues color, value < 0 - expenses color, else - none</param>
    public void UpdateBudget(int value)
    {
        if (value == 0) return;

        var budget = GameDataManager.PlayerData.GetCharacteristic(Characteristic.Budget);

        //add sfx sound
        _budgetText.color = value < 0 ? _expensesColor : _revenuesColor;

        _budgetTextTransform.LeanMoveY(_budgetTextTopPositionY, BUDGET_TEXT_ANIMATION_TIME).setEaseInQuart().setOnComplete(() =>
        {
            _budgetText.text = BudgetToString(budget);
            _budgetTextTransform.position = new Vector3(_budgetTextTransform.position.x, _budgetTextBottomPositionY, _budgetTextTransform.position.z);
            _budgetTextTransform.LeanMoveY(_budgetTextDefaultPositionY, BUDGET_TEXT_ANIMATION_TIME).setEaseOutQuart()
            .setOnComplete(() => _budgetText.color = budget < 0 ? _expensesColor : _defaultBudgetColor);
        });
    }

    //formats integer budget value to string representation. In string format budget has additional "000 000" digits
    private string BudgetToString(int budget)
    {
        if (budget == 0)
            return "0 $";

        if (Math.Abs(budget) > MAX_BUDGET_VALUE)
            return (budget < 0 ? "-" : "") + "999 999 999 999+ $";

        return (budget < 0 ? "-" : "") + budget.ToString("N0") + " 000 000 $";
    }
}