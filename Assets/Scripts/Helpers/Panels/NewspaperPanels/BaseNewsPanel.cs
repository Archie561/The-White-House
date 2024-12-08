using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseNewsPanel : MonoBehaviour, IPointerClickHandler
{
    public Action OnNewsClicked;

    public void OnPointerClick(PointerEventData eventData) => OnNewsClicked?.Invoke();

    public abstract void Initialize(News news);
}