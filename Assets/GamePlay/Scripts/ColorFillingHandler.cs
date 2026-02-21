using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.DB.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColorFillingHandler : MonoBehaviour
    {
        [SerializeField] ColorFilling[] ColoringPart;
        [SerializeField] RectTransform BrushTransform;
        [SerializeField] AudioSource BurshSound;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject NextBtn, TouchProtector, DetailsHandler, ResetButton, TextHolder;
        [SerializeField] Animation BrushAnimtion;
        [SerializeField] ParticleSystem BrushParitcle;

        float _speed = 5, _brushSize = 20, _preparationTime = 1, _fillingCOmpletePercentage = 75;
        bool _canColor = false, _coloringSound = false, _canShowNextBtn = true, _onceClicked = false, _isReseting = false, _startCutforthisPart = false;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalColorPixles = 1, _coloredPixlesCounter = 0, _previousColoredPixel = 0;
        Color32[] _partPixles;
        Color _whiteColor = Color.white;
        ParticleSystem.MainModule _pm;

        private void OnEnable()
        {
            SimpleEventsHolder.StartColoringEvent += StartColoring;
            SimpleEventsHolder.ColorSelectedEvent += ColorSelected;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.StartColoringEvent -= StartColoring;
            SimpleEventsHolder.ColorSelectedEvent -= ColorSelected;
            _canColor = false;
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }
        }

        private void Start()
        {
            LevelsManager.I.LevelStars = 3;
            _currentCamera = Camera.main;
            _coloringParTransform = ColoringPart[_paintingCounter].transform as RectTransform;
            _pm = BrushParitcle.main;
        }

        void StartColoring()
        {
            TouchProtector.SetActive(true);
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _totalColorPixles = ColoringPart[_paintingCounter].GetColoredPixlesCount();
            _partPixles = _partTexture.GetPixels32();
            BrushTransform.gameObject.SetActive(true);
            BrushTransform.DOAnchorPosX(175, _preparationTime).SetEase(Ease.OutBack).OnComplete(() =>
            {
                TextHolder.SetActive(true);
            });
            ColoringPart[_paintingCounter].MyAnimation.Play("CurrentColoringPart");
        }

        void ColorSelected()
        {
            if (!_isReseting && _paintingCounter < ColoringPart.Length)
            {
                InfoText.text = InfoMsgs[1];
                Color newColor = LevelsManager.I.CurrentColor;
                _pm.startColor = newColor;
                if (_movingRoutine == null)
                {
                    _canColor = true;
                    _movingRoutine = StartCoroutine(BrushMovingRoutine());
                    TouchProtector.SetActive(false);
                    BrushParitcle.gameObject.SetActive(true);
                }
            }
            else
            {
                LevelsManager.I.CurrentBowl.BowlState(false);
            }
        }

        IEnumerator BrushMovingRoutine()
        {
            BrushTransform.DOAnchorPosX(0, _preparationTime/2).SetEase(Ease.InBack);
            Vector2 initialOffset = Vector2.zero;
            while (_canColor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = BrushTransform.anchoredPosition - initialPoint;
                    TextHolder.SetActive(false);
                    if (!_coloringSound)
                    {
                        _coloringSound = true;
                        BrushAnimtion.Play();
                        ResetButton.SetActive(true);
                        if (DBVariablesHolder.Sound.Value == 1)
                            BurshSound.Play();
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                    // Calculate and clamp target position
                    Vector2 targetPosition = localPoint + initialOffset;
                    
                    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Smoothly move BrushTransform to the target position
                    BrushTransform.anchoredPosition = Vector2.Lerp(
                        BrushTransform.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                    // Convert to UV coordinates and apply the brush
                    Vector2 brushUV = new Vector2(
                        (BrushTransform.anchoredPosition.x - _coloringParTransform.rect.x) / _coloringParTransform.rect.width,
                        (BrushTransform.anchoredPosition.y - _coloringParTransform.rect.y) / _coloringParTransform.rect.height);

                    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);
                    ApplyBrush(texX, texY);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    BrushAnimtion.Stop();
                    BurshSound.Stop();
                    _coloringSound = false;
                    initialOffset = Vector2.zero;
                    if (_canShowNextBtn)
                       TextHolder.SetActive(true);

                    if (_coloredPixlesCounter > _previousColoredPixel && LevelsManager.I.LevelStars > 1 && !_startCutforthisPart
                        && !LevelsManager.I.CurrentColor.Equals(ColoringPart[_paintingCounter].DefaultColor))
                    {
                        LevelsManager.I.LevelStars--;
                        _startCutforthisPart = true;
                    }
                    _previousColoredPixel = _coloredPixlesCounter;
                }
                yield return null; // Yield until the next frame
            }
        }


        void ApplyBrush(int centerX, int centerY)
        {
            int brushWidth = Mathf.RoundToInt(_brushSize * 2);
            int brushHeight = Mathf.RoundToInt(_brushSize * 2);
            int compoundWidth = _partTexture.width;
            int compoundHeight = _partTexture.height;

            // Calculate bounds of the brush area on the compound texture
            int startX = centerX - brushWidth / 2;
            int startY = centerY - brushHeight / 2;

            // Loop through the brush area
            for (int y = 0; y < brushHeight; y++)
            {
                // Calculate the target Y index once per row
                int targetY = startY + y;

                // Only process if the targetY is within the compound texture bounds
                if (targetY >= 0 && targetY < compoundHeight)
                {
                    for (int x = 0; x < brushWidth; x++)
                    {
                        int targetX = startX + x;

                        // Only process if the targetX is within the compound texture bounds
                        if (targetX >= 0 && targetX < compoundWidth)
                        {
                            int compoundIndex = targetY * compoundWidth + targetX;

                            // Blend only if the brush pixel is within the circular area
                            float distSquared = (x - brushWidth / 2) * (x - brushWidth / 2) + (y - brushHeight / 2) * (y - brushHeight / 2);
                            if (distSquared <= _brushSize * _brushSize && _partPixles[compoundIndex] == _whiteColor)
                            {
                                _partPixles[compoundIndex] = LevelsManager.I.CurrentColor;
                                _coloredPixlesCounter++;
                            }
                        }
                    }
                }
            }

            // Apply the updated pixels back to the texture
            _partTexture.SetPixels32(_partPixles);
            _partTexture.Apply();
            float totalFilling = ((float)_coloredPixlesCounter / _totalColorPixles) * 100;
            if (totalFilling >= _fillingCOmpletePercentage && _canShowNextBtn)
            {
                _canShowNextBtn = false;
                _onceClicked = false;
                ColoringPart[_paintingCounter].MyAnimation.Play("DefaultColoringPart");
                NextBtn.SetActive(true);
            }
        }

        void FillRemaingPixles()
        {
            if (_coloredPixlesCounter < _totalColorPixles)
            {
                if (LevelsManager.I.LevelStars > 1 && !_startCutforthisPart && !LevelsManager.I.CurrentColor.Equals(ColoringPart[_paintingCounter].DefaultColor))
                {
                    LevelsManager.I.LevelStars--;
                    _startCutforthisPart = true;
                }
                for (int p = 0; p < _partPixles.Length; p++)
                {
                    if (_partPixles[p] == _whiteColor)
                    {
                        _partPixles[p] = LevelsManager.I.CurrentColor;
                    }
                }
                _partTexture.SetPixels32(_partPixles);
                _partTexture.Apply();
            }
            _paintingCounter++;
        }

        public void OnNextBtnClick()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                _canColor = false;
                TouchProtector.SetActive(true);
                BrushTransform.gameObject.SetActive(false);
                if (_movingRoutine != null)
                {
                    StopCoroutine(_movingRoutine);
                    _movingRoutine = null;
                }
                TextHolder.SetActive(false);
                NextBtn.SetActive(false);
                ResetButton.SetActive(false);
                FillRemaingPixles();
                LevelsManager.I.CurrentBowl.BowlState(false);
                _coloredPixlesCounter = 0;
                BurshSound.Stop();

                if (_paintingCounter < ColoringPart.Length)
                {
                    BrushTransform.DOAnchorPosX(175, 0.2f).SetEase(Ease.InBack);
                    _startCutforthisPart = false;
                    _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
                    _totalColorPixles = ColoringPart[_paintingCounter].GetColoredPixlesCount();
                    _partPixles = _partTexture.GetPixels32();
                    InfoText.text = InfoMsgs[0];
                    BrushTransform.gameObject.SetActive(true);
                    TextHolder.SetActive(true);
                    TouchProtector.SetActive(false);
                    ColoringPart[_paintingCounter].MyAnimation.Play("CurrentColoringPart");
                    _canShowNextBtn = true;
                    _onceClicked = false;
                }
                else 
                {
                    SimpleEventsHolder.HideColorBowlEvent?.Invoke();
                    DetailsHandler.SetActive(true);
                }
            }
        }

        public void ResetColor()
        {
            if (!_onceClicked && !_isReseting)
            {
                _isReseting = true;
                _onceClicked = true;
                _canColor = false;
                TouchProtector.SetActive(true);
                BrushTransform.DOAnchorPosX(175, 0.2f).SetEase(Ease.InBack);
                if (_movingRoutine != null)
                {
                    StopCoroutine(_movingRoutine);
                    _movingRoutine = null;
                }
                TextHolder.SetActive(false);
                NextBtn.SetActive(false);
                ResetButton.SetActive(false);
                LevelsManager.I.CurrentBowl.BowlState(false);
                _coloredPixlesCounter = 0;
                BurshSound.Stop();
                byte alphaThreshold = 50;
                _partPixles = _partTexture.GetPixels32();
                for (int i = 0; i < _partPixles.Length; i++)
                {
                    if (_partPixles[i].a > alphaThreshold)
                    {
                        _partPixles[i] = _whiteColor;
                    }
                }
                _partTexture.SetPixels32(_partPixles);
                _partTexture.Apply();
                Invoke(nameof(StartAgain), 0.1f);
            }
        }

        void StartAgain()
        {
            _isReseting = false;
            _onceClicked = false;
            _canShowNextBtn = true;
            InfoText.text = InfoMsgs[0];
            BrushTransform.gameObject.SetActive(true);
            TextHolder.SetActive(true);
            ColoringPart[_paintingCounter].MyAnimation.Play("CurrentColoringPart");
            if (_startCutforthisPart && LevelsManager.I.LevelStars < 3)
            {
                LevelsManager.I.LevelStars++;
                _startCutforthisPart = false;
            }
        }
    }
}