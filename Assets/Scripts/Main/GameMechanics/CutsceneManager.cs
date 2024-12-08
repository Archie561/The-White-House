using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private ParticleSystem _particles;

    private PlayerDataManager _playerDataManager;

    /*------------------------CUTSCENE UI ELEMENTS------------------------*/
    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private RawImage _frameBackground;
    [SerializeField]
    private Image _leftCharacterImage;
    [SerializeField]
    private Image _rightCharacterImage;
    [SerializeField]
    private TextMeshProUGUI _subtitles;
    [SerializeField]
    private GameObject _skipPanel;
    [SerializeField]
    private CanvasGroup _pauseBackground;
    [SerializeField]
    private ParticleSystem _snowParticles;
    [SerializeField]
    private ParticleSystem _rainParticles;
    [SerializeField]
    private ParticleSystem _dustParticles;
    /*---------------------------END UI SECTION---------------------------*/


    /*-------------------BACKGROUND TRANSFORM PROPERTIES------------------*/
    private Vector2 _backgroundDefaultScale;
    private Vector2 _backgroundDefaultPosition;

    private Vector2 _backgroundZoomedScale;
    private Vector2 _backgroundMostLeftPosition;
    private Vector2 _backgroundMostRightPosition;
    private Vector2 _backgroundMostUpPosition;
    private Vector2 _backgroundMostDownPosition;

    private float _zoomCoeff = 1.15f;
    /*------------------------END TRANSFORM SECTION-----------------------*/


    private Cutscene[] _cutscenes;
    private Cutscene _currentCutscene;

    private int _sceneIndex = 0;
    private int _frameIndex = 0;
    private int _characterIndex = 0;


    private Vector2 _skipPanelDefaultScale;
    private float _leftCharacterPosition;
    private float _rightCharacterPosition;
    private float _hideCharacterPosition;

    private float _blackScreenAnimationTime = 0.8f;
    private float _characterAnimationTime = 0.3f;
    private float _panelAnimationTime = 0.35f;


    private float _frameTime = 0;
    private float _characterTime = 0;
    private float _subtitlesTime = 0;

    private float _frameDuration = Int32.MaxValue;
    private float _characterDuration = Int32.MaxValue;
    private float _subtitlesDuration = Int32.MaxValue;

    private List<string> _subtitlesFragments = new List<string>();
    private Typewriter _typewriter;
    private int _maxSymbols = 130;
    private int _minSymbols = 90;


    private bool _isPaused;

    void Start()
    {
        _playerDataManager = ServiceLocator.GetService<PlayerDataManager>();
        var chapter_playerDataManager = ServiceLocator.GetService<ChapterDataManager>();
        _cutscenes = chapter_playerDataManager.GetStartingCutscenes();

        _typewriter = new Typewriter(_subtitles);
        _audioSource = GetComponent<AudioSource>();

        _leftCharacterPosition = _leftCharacterImage.transform.localPosition.y;
        _rightCharacterPosition = _rightCharacterImage.transform.localPosition.y;
        _backgroundDefaultPosition = _frameBackground.transform.localPosition;
        _backgroundDefaultScale = _frameBackground.transform.localScale;
        _skipPanelDefaultScale = _skipPanel.transform.localScale;

        _backgroundZoomedScale = new Vector2(_frameBackground.transform.localScale.x * _zoomCoeff, _frameBackground.transform.localScale.y * _zoomCoeff);
        _backgroundMostRightPosition = new Vector2((_frameBackground.rectTransform.rect.width * _zoomCoeff - Screen.width) / 2, _frameBackground.transform.localPosition.y);
        _backgroundMostLeftPosition = new Vector2(-_backgroundMostRightPosition.x, _backgroundMostRightPosition.y);
        _backgroundMostUpPosition = new Vector2(_frameBackground.transform.localPosition.x, (_frameBackground.rectTransform.rect.height * _zoomCoeff - Screen.height) / 2);
        _backgroundMostDownPosition = new Vector2(_backgroundMostUpPosition.x, -_backgroundMostUpPosition.y);
        _hideCharacterPosition = -_leftCharacterImage.rectTransform.rect.height;

        _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, _hideCharacterPosition);
        _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, _hideCharacterPosition);
        _skipPanel.transform.localScale = Vector2.zero;

        GetNextCutscene();
    }

    private void Update()
    {
        if (_frameTime > _frameDuration)
        {
            _frameTime = 0;
            _characterTime = 0;
            _subtitlesTime = 0;

            _frameIndex++;
            _blackBackground.gameObject.SetActive(true);
            _blackBackground.LeanAlpha(1, _blackScreenAnimationTime).setOnComplete(GetNextFrame);

            return;
        }

        if (_characterTime > _characterDuration)
        {
            _characterTime = 0;
            _subtitlesTime = 0;
            _characterIndex++;

            if (_leftCharacterImage.IsActive())
            {
                _leftCharacterImage.transform.LeanMoveLocalY(_hideCharacterPosition, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
                {
                    _leftCharacterImage.gameObject.SetActive(false);
                    GetNextCharacter();
                });
            }
            else if (_rightCharacterImage.IsActive())
            {
                _rightCharacterImage.transform.LeanMoveLocalY(_hideCharacterPosition, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
                {
                    _rightCharacterImage.gameObject.SetActive(false);
                    GetNextCharacter();
                });
            }

            return;
        }

        if (_subtitlesTime > _subtitlesDuration)
        {
            _subtitlesTime = 0;
            ShowSubtitles();
        }

        if (!_isPaused)
        {
            _frameTime += Time.deltaTime;
            _characterTime += Time.deltaTime;
            _subtitlesTime += Time.deltaTime;
        }
    }

    void GetNextCutscene()
    {
        if (_sceneIndex >= _cutscenes.Length)
        {
            _playerDataManager.UpdateStartingCutscenesShown(true);
            SceneManager.LoadScene("Main");
            return;
        }

        _currentCutscene = _cutscenes[_sceneIndex];
        if (!_playerDataManager.IsConditionMet(_currentCutscene.condition))
        {
            _sceneIndex++;
            GetNextCutscene();

            return;
        }

        ResetDefaultFrameSettings();

        _audioSource.clip = Resources.Load<AudioClip>("Audio/Cutscenes/" + _currentCutscene.voiceFileName);
        _audioSource.Play();

        _frameIndex = 0;
        _characterIndex = 0;
        _sceneIndex++;

        GetNextFrame();
    }

    void GetNextFrame()
    {
        if (_frameIndex >= _currentCutscene.frames.Length)
        {
            GetNextCutscene();
            return;
        }

        if (_particles != null)
        {
            _particles.Stop();
            _particles.Clear();
        }
        _frameBackground.transform.localScale = _backgroundDefaultScale;
        _frameBackground.transform.localPosition = _backgroundDefaultPosition;

        _frameDuration = _currentCutscene.frames[_frameIndex].frameDuration;
        _frameBackground.texture = Resources.Load<Texture>("Textures/BackgroundImages/" + _currentCutscene.frames[_frameIndex].backgroundImage);
        _blackBackground.LeanAlpha(0, _blackScreenAnimationTime).setOnComplete(() => { _blackBackground.gameObject.SetActive(false); });

        PlayParticles();
        PlayAnimation();
        GetNextCharacter();
    }

    void GetNextCharacter()
    {
        if (_characterIndex >= _currentCutscene.frames[_frameIndex].characterNames.Length)
        {
            _characterDuration = Int32.MaxValue;
            _subtitlesDuration = Int32.MaxValue;
            _subtitles.text = string.Empty;

            return;
        }

        _characterDuration = _currentCutscene.frames[_frameIndex].characterDurations[_characterIndex];

        if (_currentCutscene.frames[_frameIndex].characterPositions[_characterIndex] == "right")
        {
            _rightCharacterImage.gameObject.SetActive(true);
            _rightCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
            _rightCharacterImage.transform.LeanMoveLocalY(_rightCharacterPosition, _characterAnimationTime).setEaseOutQuart();
        }
        else
        {
            _leftCharacterImage.gameObject.SetActive(true);
            _leftCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
            _leftCharacterImage.transform.LeanMoveLocalY(_leftCharacterPosition, _characterAnimationTime).setEaseOutQuart();
        }


        //subtitles
        string[] words = _currentCutscene.frames[_frameIndex].characterSubtitles[_characterIndex].Split(' ');
        string fragment = string.Empty;
        _subtitlesFragments = new List<string>();

        for (int i = 0; i < words.Length; i++)
        {
            fragment += words[i] + ' ';

            if (fragment.Length >= _minSymbols)
            {
                _subtitlesFragments.Add(fragment);
                fragment = string.Empty;
            }
        }

        if (_subtitlesFragments.Count == 0)
        {
            _subtitlesFragments.Add(fragment);
        }
        else if (fragment.Length + _subtitlesFragments[_subtitlesFragments.Count - 1].Length > _maxSymbols)
        {
            _subtitlesFragments.Add(fragment);
        }
        else
        {
            _subtitlesFragments[_subtitlesFragments.Count - 1] += fragment;
        }

        _subtitlesDuration = _currentCutscene.frames[_frameIndex].characterDurations[_characterIndex] / _subtitlesFragments.Count;
        ShowSubtitles();
    }

    void ShowSubtitles()
    {
        if (_subtitlesFragments.Count == 0)
        {
            _subtitlesDuration = Int32.MaxValue;
            return;
        }

        _subtitles.text = _subtitlesFragments[0];
        _typewriter.StartWriting();

        _subtitlesFragments.RemoveAt(0);
    }

    private void PlayParticles()
    {
        switch (_currentCutscene.frames[_frameIndex].particlesType)
        {
            case "snow":
                _particles = _snowParticles;
                break;
            case "rain":
                _particles = _rainParticles;
                break;
            case "dust":
                _particles = _dustParticles;
                break;
            default:
                return;
        }

        _particles.Play();
    }

    private void PlayAnimation()
    {
        switch (_currentCutscene.frames[_frameIndex].animationType)
        {
            case "zoomIn":
                _frameBackground.transform.LeanScale(_backgroundZoomedScale, _frameDuration);
                break;
            case "zoomOut":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.LeanScale(_backgroundDefaultScale, _frameDuration);
                break;
            case "moveLeft":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostLeftPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostRightPosition, _frameDuration);
                break;
            case "moveRight":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostRightPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostLeftPosition, _frameDuration);
                break;
            case "moveUp":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostUpPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostDownPosition, _frameDuration);
                break;
            case "moveDown":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostDownPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostUpPosition, _frameDuration);
                break;
            default:
                return;
        }
    }

    private void ResetDefaultFrameSettings()
    {
        LeanTween.cancel(_frameBackground.gameObject);
        _frameBackground.transform.localScale = _backgroundDefaultScale;
        _frameBackground.transform.localPosition = _backgroundDefaultPosition;

        if (_particles != null)
        {
            _particles.Stop();
            _particles.Clear();
        }

        _leftCharacterImage.gameObject.SetActive(false);
        _rightCharacterImage.gameObject.SetActive(false);
        _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, _hideCharacterPosition);
        _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, _hideCharacterPosition);

        _typewriter.SkipWriting();
        _subtitles.text = string.Empty;

        _frameTime = 0;
        _characterTime = 0;
        _subtitlesTime = 0;
        _frameDuration = Int32.MaxValue;
        _characterDuration = Int32.MaxValue;
        _subtitlesDuration = Int32.MaxValue;
    }

    public void SkipButtonHandler()
    {
        if (_skipPanel.activeInHierarchy)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _isPaused = true;
        _skipPanel.SetActive(true);
        _pauseBackground.gameObject.SetActive(true);

        _audioSource.Pause();
        _pauseBackground.LeanAlpha(1, _panelAnimationTime);
        _skipPanel.transform.LeanScale(_skipPanelDefaultScale, _panelAnimationTime).setEaseOutQuart().setOnComplete(() => { Time.timeScale = 0; });
    }

    public void ConfirmSkipHandler()
    {
        Time.timeScale = 1;
        _blackBackground.gameObject.SetActive(true);
        LeanTween.cancel(_blackBackground.gameObject);
        AudioManager.Instance.PlaySFX("button");

        _skipPanel.transform.LeanScale(Vector3.zero, _panelAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(1, _blackScreenAnimationTime).setOnComplete(() =>
        {
            _isPaused = false;
            _pauseBackground.alpha = 0;
            _pauseBackground.gameObject.SetActive(false);
            _skipPanel.SetActive(false);

            ResetDefaultFrameSettings();

            GetNextCutscene();
        });
    }

    public void CancelSkipHandler()
    {
        Time.timeScale = 1;
        _isPaused = false;
        _audioSource.Play();
        AudioManager.Instance.PlaySFX("button");

        _pauseBackground.LeanAlpha(0, _panelAnimationTime).setOnComplete(() => { _pauseBackground.gameObject.SetActive(false); });
        _skipPanel.transform.LeanScale(Vector3.zero, _panelAnimationTime).setEaseOutQuart().setOnComplete(() => { _skipPanel.SetActive(false); });
    }
}

/*using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private ParticleSystem _particles;


    *//*------------------------CUTSCENE UI ELEMENTS------------------------*//*
    [SerializeField]
    private CanvasGroup _blackBackground;
    [SerializeField]
    private RawImage _frameBackground;
    [SerializeField]
    private Image _leftCharacterImage;
    [SerializeField]
    private Image _rightCharacterImage;
    [SerializeField]
    private TextMeshProUGUI _subtitles;
    [SerializeField]
    private GameObject _skipPanel;
    [SerializeField]
    private CanvasGroup _pauseBackground;
    [SerializeField]
    private ParticleSystem _snowParticles;
    [SerializeField]
    private ParticleSystem _rainParticles;
    [SerializeField]
    private ParticleSystem _dustParticles;
    *//*---------------------------END UI SECTION---------------------------*/


/*-------------------BACKGROUND TRANSFORM PROPERTIES------------------*//*
private Vector2 _backgroundDefaultScale;
private Vector2 _backgroundDefaultPosition;

private Vector2 _backgroundZoomedScale;
private Vector2 _backgroundMostLeftPosition;
private Vector2 _backgroundMostRightPosition;
private Vector2 _backgroundMostUpPosition;
private Vector2 _backgroundMostDownPosition;

private float _zoomCoeff = 1.15f;
*//*------------------------END TRANSFORM SECTION-----------------------*//*


private Cutscene[] _cutscenes;
private Cutscene _currentCutscene;

private int _sceneIndex = 0;
private int _frameIndex = 0;
private int _characterIndex = 0;


private Vector2 _skipPanelDefaultScale;
private float _leftCharacterPosition;
private float _rightCharacterPosition;
private float _hideCharacterPosition;

private float _blackScreenAnimationTime = 0.8f;
private float _characterAnimationTime = 0.3f;
private float _panelAnimationTime = 0.35f;


private float _frameTime = 0;
private float _characterTime = 0;
private float _subtitlesTime = 0;

private float _frameDuration = int.MaxValue;
private float _characterDuration = int.MaxValue;
private float _subtitlesDuration = int.MaxValue;

private List<string> _subtitlesFragments = new List<string>();
private Typewriter _typewriter;
private int _maxSymbols = 130;
private int _minSymbols = 90;

Player_playerDataManager _player_playerDataManager;
Chapter_playerDataManager _chapter_playerDataManager;


private bool _isPaused;

void Start()
{
    _player_playerDataManager = ServiceLocator.GetService<Player_playerDataManager>();
    _chapter_playerDataManager = ServiceLocator.GetService<Chapter_playerDataManager>();

    //якщо гра програна, і це не останній місяць сюжету - завантажити програшті катсцени, інакше - завантажити звичайні
    if (_player_playerDataManager.GameOver)
    {
        _currentCutscene = null;
    }
    else
    {
        _cutscenes = _chapter_playerDataManager.GetEndingCutscenes();
    }
    *//*if (GameManager.Instance.shouldDisplayStartingCutscenes)
    {
        _cutscenes = _chapter_playerDataManager.GetStartingCutscenes();
        Debug.Log("Starting cutscenes");
    }
    else if (GameManager.Instance.IsGameOver)
    {
        //_cutscenes = GameOverCutscenes();
        _cutscenes = _chapter_playerDataManager.GetStartingCutscenes();
        Debug.Log("Game Over cutscenes");
    }
    else
    {
        _cutscenes = _chapter_playerDataManager.GetEndingCutscenes();
        Debug.Log("Ending cutscenes");
    }*//*

    _typewriter = new Typewriter(_subtitles);
    _audioSource = GetComponent<AudioSource>();

    _leftCharacterPosition = _leftCharacterImage.transform.localPosition.y;
    _rightCharacterPosition = _rightCharacterImage.transform.localPosition.y;
    _backgroundDefaultPosition = _frameBackground.transform.localPosition;
    _backgroundDefaultScale = _frameBackground.transform.localScale;
    _skipPanelDefaultScale = _skipPanel.transform.localScale;

    _backgroundZoomedScale = new Vector2(_frameBackground.transform.localScale.x * _zoomCoeff, _frameBackground.transform.localScale.y * _zoomCoeff);
    _backgroundMostRightPosition = new Vector2((_frameBackground.rectTransform.rect.width * _zoomCoeff - Screen.width) / 2, _frameBackground.transform.localPosition.y);
    _backgroundMostLeftPosition = new Vector2(-_backgroundMostRightPosition.x, _backgroundMostRightPosition.y);
    _backgroundMostUpPosition = new Vector2(_frameBackground.transform.localPosition.x, (_frameBackground.rectTransform.rect.height * _zoomCoeff - Screen.height) / 2);
    _backgroundMostDownPosition = new Vector2(_backgroundMostUpPosition.x, -_backgroundMostUpPosition.y);
    _hideCharacterPosition = -_leftCharacterImage.rectTransform.rect.height;

    _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, _hideCharacterPosition);
    _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, _hideCharacterPosition);
    _skipPanel.transform.localScale = Vector2.zero;

    GetNextCutscene();
}

private void Update()
{
    if (_frameTime > _frameDuration)
    {
        _frameTime = 0;
        _characterTime = 0;
        _subtitlesTime = 0;

        _frameIndex++;
        _blackBackground.gameObject.SetActive(true);
        _blackBackground.LeanAlpha(1, _blackScreenAnimationTime).setOnComplete(GetNextFrame);

        return;
    }

    if (_characterTime > _characterDuration)
    {
        _characterTime = 0;
        _subtitlesTime = 0;
        _characterIndex++;

        if (_leftCharacterImage.IsActive())
        {
            _leftCharacterImage.transform.LeanMoveLocalY(_hideCharacterPosition, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
            {
                _leftCharacterImage.gameObject.SetActive(false);
                GetNextCharacter();
            });
        }
        else if (_rightCharacterImage.IsActive())
        {
            _rightCharacterImage.transform.LeanMoveLocalY(_hideCharacterPosition, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
            {
                _rightCharacterImage.gameObject.SetActive(false);
                GetNextCharacter();
            });
        }

        return;
    }

    if (_subtitlesTime > _subtitlesDuration)
    {
        _subtitlesTime = 0;
        ShowSubtitles();
    }

    if (!_isPaused)
    {
        _frameTime += Time.deltaTime;
        _characterTime += Time.deltaTime;
        _subtitlesTime += Time.deltaTime;
    }
}

void GetNextCutscene()
{
    if (_sceneIndex >= _cutscenes.Length)
    {
        *//*if (GameManager.Instance.IsGameOver)
        {
            //!!!
            //Destroy(GameManager.Instance.gameObject);

            //System.IO.File.Delete(System.IO.Directory.GetCurrentDirectory() + "/Assets/Story/" + _playerDataManager.CurrentSelectedPresident + "/PlayerData.json");

            SceneManager.LoadScene(0);

            return;
        }*/

/* if (GameManager.Instance.shouldDisplayStartingCutscenes)
 {
     GameManager.Instance.shouldDisplayStartingCutscenes = false;
     SceneManager.LoadScene(1);
     return;
 }*/

/*            if (_playerDataManager.PlayerData.chapterID >= _playerDataManager.ChaptersAmount)
            {
                SceneManager.LoadScene(0);
                return;
            }*//*

            SceneManager.LoadScene("Menu");

            return;
        }

        _currentCutscene = _cutscenes[_sceneIndex];
        if (!_player_playerDataManager.IsConditionMet(_currentCutscene.condition))
        {
            _sceneIndex++;
            GetNextCutscene();

            return;
        }

        ResetDefaultFrameSettings();

        _audioSource.clip = Resources.Load<AudioClip>("Audio/Cutscenes/" + _currentCutscene.voiceFileName);
        _audioSource.Play();

        _frameIndex = 0;
        _characterIndex = 0;
        _sceneIndex++;

        GetNextFrame();
    }

    void GetNextFrame()
    {
        if (_frameIndex >= _currentCutscene.frames.Length)
        {
            GetNextCutscene();
            return;
        }

        if (_particles != null)
        {
            _particles.Stop();
            _particles.Clear();
        }
        _frameBackground.transform.localScale = _backgroundDefaultScale;
        _frameBackground.transform.localPosition = _backgroundDefaultPosition;

        _frameDuration = _currentCutscene.frames[_frameIndex].frameDuration;
        _frameBackground.texture = Resources.Load<Texture>("Textures/BackgroundImages/" + _currentCutscene.frames[_frameIndex].backgroundImage);
        _blackBackground.LeanAlpha(0, _blackScreenAnimationTime).setOnComplete(() => { _blackBackground.gameObject.SetActive(false); });

        PlayParticles();
        PlayAnimation();
        GetNextCharacter();
    }

    void GetNextCharacter()
    {
        if (_characterIndex >= _currentCutscene.frames[_frameIndex].characterNames.Length)
        {
            _characterDuration = Int32.MaxValue;
            _subtitlesDuration = Int32.MaxValue;
            _subtitles.text = string.Empty;

            return;
        }

        _characterDuration = _currentCutscene.frames[_frameIndex].characterDurations[_characterIndex];

        //якщо позиція персонажа справа - активувати правий геймобджект, інакше - лівий
        if (_currentCutscene.frames[_frameIndex].characterPositions[_characterIndex] == "right")
        {
            _rightCharacterImage.gameObject.SetActive(true);
            _rightCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
            _rightCharacterImage.transform.LeanMoveLocalY(_rightCharacterPosition, _characterAnimationTime).setEaseOutQuart();
        }
        else
        {
            _leftCharacterImage.gameObject.SetActive(true);
            _leftCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
            _leftCharacterImage.transform.LeanMoveLocalY(_leftCharacterPosition, _characterAnimationTime).setEaseOutQuart();
        }


        //subtitles
        string[] words = _currentCutscene.frames[_frameIndex].characterSubtitles[_characterIndex].Split(' ');
        string fragment = string.Empty;
        _subtitlesFragments = new List<string>();

        //поділ субтитрів на фрагменти
        for (int i = 0; i < words.Length; i++)
        {
            fragment += words[i] + ' ';

            if (fragment.Length >= _minSymbols)
            {
                _subtitlesFragments.Add(fragment);
                fragment = string.Empty;
            }
        }
        //додавання останнього фрагменту до вже існуючого, або створення нового
        if (_subtitlesFragments.Count == 0)
        {
            _subtitlesFragments.Add(fragment);
        }
        else if (fragment.Length + _subtitlesFragments[_subtitlesFragments.Count - 1].Length > _maxSymbols)
        {
            _subtitlesFragments.Add(fragment);
        }
        else
        {
            _subtitlesFragments[_subtitlesFragments.Count - 1] += fragment;
        }

        _subtitlesDuration = _currentCutscene.frames[_frameIndex].characterDurations[_characterIndex] / _subtitlesFragments.Count;
        ShowSubtitles();
    }

    void ShowSubtitles()
    {
        if (_subtitlesFragments.Count == 0)
        {
            _subtitlesDuration = Int32.MaxValue;
            return;
        }

        _subtitles.text = _subtitlesFragments[0];
        _typewriter.StartWriting();

        _subtitlesFragments.RemoveAt(0);
    }

    private void PlayParticles()
    {
        switch (_currentCutscene.frames[_frameIndex].particlesType)
        {
            case "snow":
                _particles = _snowParticles;
                break;
            case "rain":
                _particles = _rainParticles;
                break;
            case "dust":
                _particles = _dustParticles;
                break;
            default:
                return;
        }

        _particles.Play();
    }

    private void PlayAnimation()
    {
        switch (_currentCutscene.frames[_frameIndex].animationType)
        {
            case "zoomIn":
                _frameBackground.transform.LeanScale(_backgroundZoomedScale, _frameDuration);
                break;
            case "zoomOut":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.LeanScale(_backgroundDefaultScale, _frameDuration);
                break;
            case "moveLeft":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostLeftPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostRightPosition, _frameDuration);
                break;
            case "moveRight":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostRightPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostLeftPosition, _frameDuration);
                break;
            case "moveUp":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostUpPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostDownPosition, _frameDuration);
                break;
            case "moveDown":
                _frameBackground.transform.localScale = _backgroundZoomedScale;
                _frameBackground.transform.localPosition = _backgroundMostDownPosition;
                _frameBackground.transform.LeanMoveLocal(_backgroundMostUpPosition, _frameDuration);
                break;
            default:
                return;
        }
    }

    private void ResetDefaultFrameSettings()
    {
        LeanTween.cancel(_frameBackground.gameObject);
        _frameBackground.transform.localScale = _backgroundDefaultScale;
        _frameBackground.transform.localPosition = _backgroundDefaultPosition;

        if (_particles != null)
        {
            _particles.Stop();
            _particles.Clear();
        }

        _leftCharacterImage.gameObject.SetActive(false);
        _rightCharacterImage.gameObject.SetActive(false);
        _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, _hideCharacterPosition);
        _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, _hideCharacterPosition);

        _typewriter.SkipWriting();
        _subtitles.text = string.Empty;

        _frameTime = 0;
        _characterTime = 0;
        _subtitlesTime = 0;
        _frameDuration = Int32.MaxValue;
        _characterDuration = Int32.MaxValue;
        _subtitlesDuration = Int32.MaxValue;
    }

    public void SkipButtonHandler()
    {
        if (_skipPanel.activeInHierarchy)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("button");

        _isPaused = true;
        _skipPanel.SetActive(true);
        _pauseBackground.gameObject.SetActive(true);

        _audioSource.Pause();
        _pauseBackground.LeanAlpha(1, _panelAnimationTime);
        _skipPanel.transform.LeanScale(_skipPanelDefaultScale, _panelAnimationTime).setEaseOutQuart().setOnComplete(() => { Time.timeScale = 0; });
    }

    public void ConfirmSkipHandler()
    {
        Time.timeScale = 1;
        _blackBackground.gameObject.SetActive(true);
        LeanTween.cancel(_blackBackground.gameObject);
        AudioManager.Instance.PlaySFX("button");

        _skipPanel.transform.LeanScale(Vector3.zero, _panelAnimationTime).setEaseOutQuart();
        _blackBackground.LeanAlpha(1, _blackScreenAnimationTime).setOnComplete(() =>
        {
            _isPaused = false;
            _pauseBackground.alpha = 0;
            _pauseBackground.gameObject.SetActive(false);
            _skipPanel.SetActive(false);

            ResetDefaultFrameSettings();

            GetNextCutscene();
        });
    }

    public void CancelSkipHandler()
    {
        Time.timeScale = 1;
        _isPaused = false;
        _audioSource.Play();
        AudioManager.Instance.PlaySFX("button");

        _pauseBackground.LeanAlpha(0, _panelAnimationTime).setOnComplete(() => { _pauseBackground.gameObject.SetActive(false); });
        _skipPanel.transform.LeanScale(Vector3.zero, _panelAnimationTime).setEaseOutQuart().setOnComplete(() => { _skipPanel.SetActive(false); });
    }
}*/