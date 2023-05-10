using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextViewController : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private Typewriter _typewriter;

    public void Start()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.maxVisibleCharacters = 0;
        _typewriter = new Typewriter();

        _typewriter.Text = _text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_typewriter.IsWriting)
        {
            _typewriter.StartWriting();
        }
        else
        {
            _typewriter.SkipWriting();
        }
    }
}
