using UnityEngine;
using UnityEngine.EventSystems;

public class InternalActivityButton : BaseActivityButton
{
    [SerializeField] private GameState _activityGameState;

    //if interaction is allowed - switches the game state to _activityGameState
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable) return;

        GameManager.Instance.ChangeGameState(_activityGameState);
    }
}