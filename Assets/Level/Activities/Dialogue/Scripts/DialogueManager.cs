using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private RawImage _backgroundImage;

    [SerializeField] private LinePanel _linePanel;
    [SerializeField] private ResponsePanel _responsePanel;

    private Queue<DialogueLine> _dialogueLines;
    private DialogueLine _currentLine;

    private Dictionary<string, Sprite> _characterSprites;

    private void Start() => Initialize();

    private void Initialize()
    {
        if (!GameDataManager.TryGetNextDialogue(out var dialogue))
        {
            ShowNoDialogueText();
            return;
        }
        _dialogueLines = new Queue<DialogueLine>(dialogue.lines);

        LoadBackroundImage(dialogue.backgroundImage);
        LoadMusic(dialogue.backgoundMusic);
        InitializeSpriteDictionary();
        InitializeResponsePanel();

        HandleNextLineDisplay();
    }

    private void ShowNoDialogueText()
    {
        Debug.LogError("No dialogue available for the current chapter.");
        LevelManager.Instance.LoadScene("Main");
    }

    private void LoadBackroundImage(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
            return;
        var backgroundImage = Resources.Load<Texture>($"Textures/BackgroundImages/{imageName}");
        if (backgroundImage != null)
            _backgroundImage.texture = backgroundImage;
        else
            Debug.LogError($"Background image not found: {imageName}");
    }

    private void LoadMusic(string musicName)
    {
        if (string.IsNullOrEmpty(musicName))
            return;
        var clip = Resources.Load<AudioClip>($"Audio/Music/{musicName}");
        if (clip != null)
            AudioManager.Instance.PlayMusic(clip);
        else
            Debug.LogError($"Music not found: {musicName}");
    }

    private void InitializeSpriteDictionary()
    {
        _characterSprites = new Dictionary<string, Sprite>();

        foreach (var line in _dialogueLines)
        {
            string spriteName = line.characterImage;

            if (_characterSprites.ContainsKey(spriteName))
                continue;

            var sprite = Resources.Load<Sprite>($"Textures/Characters/{spriteName}");
            if (sprite != null)
            {
                _characterSprites.Add(spriteName, sprite);
            }
            else
            {
                Debug.LogError($"Sprite not found: {spriteName}");
                //можна додати обробку помилки, наприклад, додати спрайт за замовчуванням
            }
        }
    }

    private void InitializeResponsePanel()
    {
        _responsePanel.SetPresidentImage(Resources.Load<Sprite>($"Textures/Characters/{GameDataManager.ActivePresident}"));
        //подивитися чи не можна зробити це через UnityEvent
        _responsePanel.OnResponseSelected += ResponseClickHandler;
    }

    private void HandleNextLineDisplay()
    {
        while (_dialogueLines.Count > 0)
        {
            _currentLine = _dialogueLines.Dequeue();

            if (string.IsNullOrEmpty(_currentLine.requiredResponseId) || GameDataManager.IsResponsePicked(_currentLine.requiredResponseId))
            {
                if (!_linePanel.gameObject.activeSelf)
                    SwapPanels();

                _linePanel.Initialize(_characterSprites[_currentLine.characterImage], _currentLine.characterName, _currentLine.text);
                return;
            }
        }

        ExitActivity();
    }

    public void LineClickHandler()
    {
        if (_linePanel.IsTextWriting)
        {
            _linePanel.SkipWriting();
            return;
        }

        if (_currentLine.responses.Count > 0)
        {
            _responsePanel.Initialize(_currentLine.responses);
            SwapPanels();
        }
        else
        {
            HandleNextLineDisplay();
        }
    }

    public void ResponseClickHandler(string id)
    {
        GameDataManager.SaveResponse(id);
        HandleNextLineDisplay();
    }

    private void SwapPanels()
    {
        _linePanel.gameObject.SetActive(!_linePanel.gameObject.activeSelf);
        _responsePanel.gameObject.SetActive(!_responsePanel.gameObject.activeSelf);
    }

    public void ExitActivity()
    {
        GameDataManager.FinishDialogue();
        GameDataManager.Save();
        LevelManager.Instance.LoadScene("Main");
    }

    private void OnDestroy()
    {
        _responsePanel.OnResponseSelected -= ResponseClickHandler;
    }
}
