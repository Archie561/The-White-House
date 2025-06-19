using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private Image _presidentImage;
    [SerializeField] private List<ResponseButton> _responseButtons;

    public event Action<string> OnResponseSelected;

    public void SetPresidentImage(Sprite image) => _presidentImage.sprite = image;

    public void Initialize(List<Response> responses)
    {
        for (int i = 0; i < responses.Count; i++)
        {
            if (i >= _responseButtons.Count)
            {
                Debug.LogError($"Not enough buttons in the panel. Expected: {responses.Count}, Found: {_responseButtons.Count}");
                break;
            }

            SetupResponseButton(_responseButtons[i], responses[i]);
        }
    }

    private void SetupResponseButton(ResponseButton responseButton, Response response)
    {
        responseButton.gameObject.SetActive(true);
        responseButton.label.text = response.text;
        responseButton.button.onClick.RemoveAllListeners();
        responseButton.button.onClick.AddListener(() => OnResponseSelected?.Invoke(response.id));
    }

    private void OnDisable()
    {
        foreach (var button in _responseButtons)
            button.gameObject.SetActive(false);
    }
}