using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.States;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.Coloring
{
    public class DetailsHandler : MonoBehaviour
    {
        [SerializeField] RectTransform SprayCan, FlameThrower, ColoringParts;
        [SerializeField] AudioSource SpraySound, FlameSound;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject NextBtn, TouchProtector, BubbleParticle, FlameParticles, TextHolder;
        [SerializeField] Image Details;
        [SerializeField] RawImage CompoundImg;
        [SerializeField] Texture2D CloudTexture;
        [SerializeField] ParticleSystem StarParticles;

        float _speed = 5, _brushSize = 25, _preparationTime = 0.5f, _finalPos = 175, _detailFillingTime = 0.1f;
        bool _canSpray = false, _effectCheck = false, _canShowNextBtn = true, _onceClicked = true, _canThrowFlame = false, 
             _detailsApplied = false, _sprayFlameReleaseInProgress = false;
        Coroutine _movingRoutine;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _totalColorPixles = 1, _flamePixlesCounter = 0, _compoundTextureLength = 500, _totalBubbles = 25, _bubbleCounter = 0, _lastAppliedCenterX = -1, _lastAppliedCenterY = -1;
        Color32[] _cloudPixles, _compoundPixels;
        const int _bubbleThreshold = 32;
        Color32 _fillColor = new Color32(0, 0, 0, 0);
        string _sprayPath = "GamePlay/Spray/", _flamePath = "GamePlayFlame/";
        Animation _sprayAnimation;
        AsyncOperationHandle _sprayHandle, _flameHandle;

        private async void OnDisable()
        {
            _canSpray = false;
            _canThrowFlame = false;
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }
            await ReleaseSprayAndFlameHandlesSafelyAsync();
        }

        async Task ReleaseSprayAndFlameHandlesSafelyAsync()
        {
            if (_sprayFlameReleaseInProgress)
                return;

            _sprayFlameReleaseInProgress = true;

            // wait for next frame (Unity safe point)
            await Task.Yield();

            if (_sprayHandle.IsValid())
            {
                Addressables.Release(_sprayHandle);
                _sprayHandle = default;
            }

            if (_flameHandle.IsValid())
            {
                Addressables.Release(_flameHandle);
                _flameHandle = default;
            }

            // optional extra frame (Android / scene safety)
            await Task.Yield();

            _sprayFlameReleaseInProgress = false;
        }


        private void Start()
        {
            _currentCamera = Camera.main;
            LoadSprayObj(_sprayPath + DBVariablesHolder.CurrentActiveSpray.Value);
            Invoke(nameof(PrepareCompounD),0.1f);
        }

        async void LoadSprayObj(string path)
        {
            _sprayHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _sprayHandle.Task;

            if (_sprayHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject obj = Instantiate(_sprayHandle.Result as GameObject, SprayCan);
            await Task.Yield();
            await Task.Yield();
            _sprayAnimation = obj.GetComponent<Animation>();

            await Task.Yield();
        }

        public void OnNextBtnClick()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                TouchProtector.SetActive(true);
                SpraySound.Stop();
                FlameSound.Stop();
                if (_movingRoutine != null)
                {
                    StopCoroutine(_movingRoutine);
                    _movingRoutine = null;
                }
                TextHolder.SetActive(false);
                NextBtn.SetActive(false);

                if (!_detailsApplied)
                {
                    _canSpray = false;
                    SprayCan.gameObject.SetActive(false);
                    Details.gameObject.SetActive(true);
                    PrepareFlames();
                }
                else if (_detailsApplied)
                {
                    _canThrowFlame = false;
                    FlameThrower.gameObject.SetActive(false);
                    Details.DOFillAmount(1, _detailFillingTime);
                    HideRemaingPixles();
                    SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(6);
                    StarParticles.Play();
                    ColoringParts.DOScale(1.35f, _preparationTime).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        Invoke(nameof(LevelComplete), 2);
                    });
                }
            }
        }

        void LevelComplete()
        {
            ColoringParts.DOScale(0.75f, _preparationTime);
            ColoringParts.DOAnchorPosY(_finalPos, _preparationTime).OnComplete(() =>
            {
                StateManager.I.ActiveState(StateManager.I.LevelCompleteStatePath);
            });
        }

        void PrepareCompounD()
        {
            _partTexture = new Texture2D(_compoundTextureLength, _compoundTextureLength, TextureFormat.RGBA32, false);
            _compoundPixels = _partTexture.GetPixels32();

            // Apply the color to all pixels
            for (int i = 0; i < _compoundPixels.Length; i++)
            {
                _compoundPixels[i] = _fillColor;
                _totalColorPixles++;

            }
            // Apply the updated pixels to the texture
            _partTexture.SetPixels32(_compoundPixels);
            _partTexture.Apply();
            CompoundImg.texture = _partTexture;
            _cloudPixles = CloudTexture.GetPixels32();
            SprayCan.gameObject.SetActive(true);
            _canSpray = true;
            _movingRoutine = StartCoroutine(SprayMovingRoutine());
            InfoText.text = InfoMsgs[0];
            CompoundImg.enabled = true;
        }

        IEnumerator SprayMovingRoutine()
        {
            Vector2 initialOffset = Vector2.zero;
            RectTransform coloringParTransform = CompoundImg.rectTransform;

            while (_canSpray)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = SprayCan.anchoredPosition - initialPoint;
                    TextHolder.SetActive(false);
                    if (!_effectCheck)
                    {
                        _effectCheck = true;
                        BubbleParticle.SetActive(true);
                        _sprayAnimation.Play("SprayOn");
                        if (DBVariablesHolder.Sound.Value == 1)
                            SpraySound.Play();
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                    // Calculate and clamp target position
                    Vector2 targetPosition = localPoint + initialOffset;
                    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Smoothly move BrushTransform to the target position
                    SprayCan.anchoredPosition = Vector2.Lerp(
                        SprayCan.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                    Vector2 brushUV = new Vector2(
                    (SprayCan.anchoredPosition.x - coloringParTransform.rect.x) / coloringParTransform.rect.width,
                    (SprayCan.anchoredPosition.y - coloringParTransform.rect.y) / coloringParTransform.rect.height);

                    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);

                    ApplyClouds(texX, texY);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    initialOffset = Vector2.zero;
                    BubbleParticle.SetActive(false);
                    _sprayAnimation.Play("SprayOff");
                    SpraySound.Stop();
                    _effectCheck = false;
                    if (_canShowNextBtn)
                    { TextHolder.SetActive(true); }
                }
                yield return null; // Yield until the next frame
            }
        }

        void ApplyClouds(int centerX, int centerY)
        {
            // Check if the position has changed significantly
            if (Mathf.Abs(centerX - _lastAppliedCenterX) <= _bubbleThreshold && Mathf.Abs(centerY - _lastAppliedCenterY) <= _bubbleThreshold)
            {
                return; // Skip applying the bubble if the position hasn't changed enough
            }

            _bubbleCounter++;
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(5);
            // Save the new position as the last applied position
            _lastAppliedCenterX = centerX;
            _lastAppliedCenterY = centerY;

            int bubbleWidth = CloudTexture.width;
            int bubbleHeight = CloudTexture.height;
            int compoundWidth = _partTexture.width;
            int compoundHeight = _partTexture.height;

            // Calculate bounds of the bubble on the compound texture
            int startX = centerX - bubbleWidth / 2;
            int startY = centerY - bubbleHeight / 2;

            // Loop through bubble texture pixels
            for (int y = 0; y < bubbleHeight; y++)
            {
                // Calculate the target Y index once per row
                int targetY = startY + y;

                // Only process if the targetY is within the compound texture bounds
                if (targetY >= 0 && targetY < compoundHeight)
                {
                    for (int x = 0; x < bubbleWidth; x++)
                    {
                        int targetX = startX + x;

                        // Only process if the targetX is within the compound texture bounds
                        if (targetX >= 0 && targetX < compoundWidth)
                        {
                            int bubbleIndex = y * bubbleWidth + x;
                            int compoundIndex = targetY * compoundWidth + targetX;

                            // Blend only if the bubble pixel is not fully transparent
                            if (_cloudPixles[bubbleIndex].a > 0.5f)
                            {
                                _compoundPixels[compoundIndex] = _cloudPixles[bubbleIndex];
                            }
                        }
                    }
                }
            }

            // Set all the updated pixels at once
            _partTexture.SetPixels32(_compoundPixels);
            _partTexture.Apply();
            if (_bubbleCounter >= _totalBubbles && _canShowNextBtn)
            {
                _canShowNextBtn = false;
                _onceClicked = false;
                NextBtn.SetActive(true);
            }
        }

        void PrepareFlames()
        {
            _totalColorPixles = 0;
            for (int i = 0; i < _compoundPixels.Length; i++)
            {
                if (_compoundPixels[i].a > 0)
                _totalColorPixles++;

            }
            FlameThrower.gameObject.SetActive(true);
            InfoText.text = InfoMsgs[1];
            TouchProtector.SetActive(false);
            _canShowNextBtn = true;
            _canThrowFlame = true;
            _movingRoutine = StartCoroutine(FlamesThrowingRoutine());
            LoadFlameObj(_flamePath + DBVariablesHolder.CurrentActiveFlameThrower.Value);
        }

        async void LoadFlameObj(string path)
        {
            _flameHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _flameHandle.Task;

            if (_flameHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject flameObj = Instantiate(_flameHandle.Result as GameObject, FlameThrower);

            await Task.Yield();
            await Task.Yield();
        }

        IEnumerator FlamesThrowingRoutine()
        {
            Vector2 initialOffset = Vector2.zero;
            RectTransform coloringParTransform = CompoundImg.rectTransform;
            while (_canThrowFlame)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = FlameThrower.anchoredPosition - initialPoint;
                    TextHolder.SetActive(false);
                    if (!_effectCheck)
                    {
                        _effectCheck = true;
                        FlameParticles.SetActive(true);
                        if (DBVariablesHolder.Sound.Value == 1)
                            FlameSound.Play();
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                    // Calculate and clamp target position
                    Vector2 targetPosition = localPoint + initialOffset;
                    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Smoothly move BrushTransform to the target position
                    FlameThrower.anchoredPosition = Vector2.Lerp(
                        FlameThrower.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                    Vector2 brushUV = new Vector2(
                    (FlameThrower.anchoredPosition.x - coloringParTransform.rect.x) / coloringParTransform.rect.width,
                    (FlameThrower.anchoredPosition.y - coloringParTransform.rect.y) / coloringParTransform.rect.height);

                    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);

                    ApplyMask(texX, texY);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    initialOffset = Vector2.zero;
                    FlameParticles.SetActive(false);
                    FlameSound.Stop();
                    _effectCheck = false;
                    if (_canShowNextBtn)
                    { TextHolder.SetActive(true); }
                }
                yield return null; // Yield until the next frame
            }
        }

        void ApplyMask(int centerX, int centerY)
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
                            int brushIndex = y * brushWidth + x;
                            int compoundIndex = targetY * compoundWidth + targetX;

                            // Blend only if the brush pixel is within the circular area
                            float distSquared = (x - brushWidth / 2) * (x - brushWidth / 2) + (y - brushHeight / 2) * (y - brushHeight / 2);
                            if (distSquared <= _brushSize * _brushSize && _compoundPixels[compoundIndex].a > 0)
                            {
                                _flamePixlesCounter++;
                                _compoundPixels[compoundIndex] = _fillColor;
                            }
                        }
                    }
                }
            }
            _partTexture.SetPixels32(_compoundPixels);
            _partTexture.Apply();
            Details.fillAmount += 0.025f;
            float totalFilling = ((float)_flamePixlesCounter / _totalColorPixles) * 100;
            if (totalFilling >= 75 && _canShowNextBtn)
            {
                _canShowNextBtn = false;
                _onceClicked = false;
                _detailsApplied = true;
                NextBtn.SetActive(true);
            }
        }
        void HideRemaingPixles()
        {
            if (_flamePixlesCounter < _totalColorPixles)
            {
                for (int p = 0; p < _compoundPixels.Length; p++)
                {
                    if (_compoundPixels[p].a > 0)
                    {
                        _compoundPixels[p] = _fillColor;
                    }
                }
                _partTexture.SetPixels32(_compoundPixels);
                _partTexture.Apply();
            }
        }
    }
}
