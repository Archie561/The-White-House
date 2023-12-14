using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    private AudioSource _audioSource;
    /*---------------------------Cutscene UI Elements---------------------------*/
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
    /*---------------------------END UI SECTION---------------------------*/

    private Cutscene[] _cutscenes;
    private Cutscene _currentCutscene;
    private int _sceneIndex;
    private int _frameIndex;
    private int _characterIndex;

    private float _blackScreenDelay = 0.8f;
    private float _characterAnimationTime = 0.3f;

    private float _leftCharacterPosition;
    private float _rightCharacterPosition;

    private Typewriter _typewriter;

    //тест анімації
    private Vector3 _backgroundScale;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _sceneIndex = 0;
        _frameIndex = 0;
        _characterIndex = 0;

        _leftCharacterPosition = _leftCharacterImage.transform.localPosition.y;
        _rightCharacterPosition = _rightCharacterImage.transform.localPosition.y;

        _backgroundScale = _frameBackground.transform.localScale;

        _typewriter = new Typewriter(_subtitles);

        //перевірку добавити в геймменеджер, перед загрузкою сцени
        _cutscenes = GameManager.Instance.GetCutscenes();
        if (_sceneIndex >= _cutscenes.Length)
        {
            GameManager.Instance.LoadMainScene();
        }

        StartCoroutine(ShowFrame());
    }

    IEnumerator ShowFrame()
    {
        //перевірити залежність від умов, якщо не проходять, збільшити sceneIndex на 1 і викликати функцію заново
        _currentCutscene = _cutscenes[_sceneIndex];

        _audioSource.clip = Resources.Load<AudioClip>("Music/Cutscenes/" + _currentCutscene.voiceFileName);
        _audioSource.Play();

        while (_frameIndex < _currentCutscene.frames.Length)
        {
            yield return new WaitForSeconds(_blackScreenDelay);
            _blackBackground.LeanAlpha(0, _blackScreenDelay).setOnComplete(() => { _blackBackground.gameObject.SetActive(false); });

            _frameBackground.texture = Resources.Load<Texture>("Textures/Backgrounds/" + _currentCutscene.frames[_frameIndex].backgroundImage);

            // тест анімації
            _frameBackground.transform.localScale = _backgroundScale;
            if (_currentCutscene.frames[_frameIndex].animationType == "2")
            {
                _frameBackground.transform.LeanScale(new Vector3(_frameBackground.transform.localScale.x + _frameBackground.transform.localScale.x * 0.1f, _frameBackground.transform.localScale.y + _frameBackground.transform.localScale.y * 0.15f, _frameBackground.transform.localScale.z), _currentCutscene.frames[_frameIndex].frameDuration);
            }

            StartCoroutine(ShowCharacter());

            yield return new WaitForSeconds(_currentCutscene.frames[_frameIndex].frameDuration);

            _subtitles.text = "";
            _blackBackground.gameObject.SetActive(true);
            _blackBackground.LeanAlpha(1, _blackScreenDelay);
            _frameIndex++;
        }

        GameManager.Instance.LoadMainScene();
        //функція затемнення екрану
    }

    IEnumerator ShowCharacter()
    {
        while (_characterIndex < _currentCutscene.frames[_frameIndex].characterNames.Length)
        {
            PlayAnimation();

            //пофіксити баг коли анімація персонажа закінчується швидше ніж субтитри

            _subtitles.text = _currentCutscene.frames[_frameIndex].characterSubtitles[_characterIndex];
            _typewriter.StartWriting();

            yield return new WaitForSeconds(_currentCutscene.frames[_frameIndex].characterDurations[_characterIndex]);

            _characterIndex++;
        }

        if (_leftCharacterImage.IsActive())
        {
            _leftCharacterImage.transform.LeanMoveLocalY(-Screen.height, _characterAnimationTime).setEaseOutQuart().setOnComplete(() => { _leftCharacterImage.gameObject.SetActive(false); });
        }
        else
        {
            _rightCharacterImage.transform.LeanMoveLocalY(-Screen.height, _characterAnimationTime).setEaseOutQuart().setOnComplete(() => { _rightCharacterImage.gameObject.SetActive(false); });
        }
        _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, _leftCharacterPosition);
        _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, _rightCharacterPosition);

        //анімація приховування персонажів
        _characterIndex = 0;
    }

    private void PlayAnimation()
    {
        //якщо це перший персонаж у фреймі
        if (!_leftCharacterImage.IsActive() && !_rightCharacterImage.IsActive())
        {
            _leftCharacterImage.transform.localPosition = new Vector2(_leftCharacterImage.transform.localPosition.x, -Screen.height);
            _rightCharacterImage.transform.localPosition = new Vector2(_rightCharacterImage.transform.localPosition.x, -Screen.height);

            //якщо позиція персонажа зліва
            if (_currentCutscene.frames[_frameIndex].characterPositions[_characterIndex] == "left")
            {
                _leftCharacterImage.gameObject.SetActive(true);
                _leftCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
                _leftCharacterImage.transform.LeanMoveLocalY(_leftCharacterPosition, _characterAnimationTime).setEaseOutQuart();
            }
            //якщо позиція персонажа справа
            else
            {
                _rightCharacterImage.gameObject.SetActive(true);
                _rightCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
                _rightCharacterImage.transform.LeanMoveLocalY(_rightCharacterPosition, _characterAnimationTime).setEaseOutQuart();
            }
        }
        //якщо потрібно просто поміняти персонажів
        else
        {
            //якщо позиція персонажа зліва
            if (_currentCutscene.frames[_frameIndex].characterPositions[_characterIndex] == "left")
            {
                _rightCharacterImage.transform.LeanMoveLocalY(-Screen.height, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
                {
                    _rightCharacterImage.gameObject.SetActive(false);
                    _leftCharacterImage.gameObject.SetActive(true);
                    _leftCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
                    _leftCharacterImage.transform.LeanMoveLocalY(_leftCharacterPosition, _characterAnimationTime).setEaseOutQuart();
                });
            }
            //якщо позиція персонажа справа
            else
            {
                _leftCharacterImage.transform.LeanMoveLocalY(-Screen.height, _characterAnimationTime).setEaseOutQuart().setOnComplete(() =>
                {
                    _leftCharacterImage.gameObject.SetActive(false);
                    _rightCharacterImage.gameObject.SetActive(true);
                    _rightCharacterImage.sprite = Resources.Load<Sprite>("Textures/Characters/" + _currentCutscene.frames[_frameIndex].characterNames[_characterIndex]);
                    _rightCharacterImage.transform.LeanMoveLocalY(_rightCharacterPosition, _characterAnimationTime).setEaseOutQuart();
                });
            }
        }
    }

}
