using TMPro;
using System.IO;
using UnityEngine;
using Core.States;
using DG.Tweening;
using Core.Events;
using Core.Economy;
using Core.GamePlay;
using UnityEngine.UI;
using Core.DB.Variables;
using System.Collections;
using Core.Plugins.Firebase;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class CompleteScreen : UiScreens
    {
        [SerializeField] TextMeshProUGUI LevelBonusText, StarsBonusText, MovesBonusText, TotalBonusText;
        [SerializeField] Camera ScreenshotCamera;
        [SerializeField] RawImage DisplayImage;
        [SerializeField] Image[] StarsImg;
        [SerializeField] RectTransform StarsObj, NextBtn;
        [SerializeField] GameObject RvBtn;
        [SerializeField] MultiplierBar CurrentMultiplayerBar;


        float _starsPos = 100, _durationTweeing = 1f;
        bool _onceClicked = true, _txtReleaseInProgress = false;
        int _levelBonus = 15, _starsBonus = 10, _totalBonus = 0; 
        Coroutine _screenShotRotine;
        string _starsDataPath;
        Vector2 _nextBtnPosition = new Vector2(-135, -430);
        string _textPath = "Appreation/Text/";
        GameObject _textObj;
        AsyncOperationHandle _txtHandle;
        RenderTexture _tempRT;
        Color _starColor = Color.white;

        private void OnEnable()
        {
            SimpleEventsHolder.MultiplayRewardEvent += OnMultiplyReward;
        }

        private async void OnDisable()
        {
            SimpleEventsHolder.MultiplayRewardEvent -= OnMultiplyReward;

            if (_screenShotRotine != null)
                StopCoroutine(_screenShotRotine);

            await ReleaseTextHandleSafelyAsync();
        }

        void Start()
        {
            int textNum = 0;
            if (LevelsManager.I.LevelStars > 2)
            { 
                textNum = Random.Range(1,9);
            }
            LoadAppreationText(_textPath + textNum);
            LevelsManager.I.SortingCompleted = false;
            _starsDataPath = Path.Combine(Application.persistentDataPath, "starsData.json");
            _screenShotRotine = StartCoroutine(CaptureColoredArea());
            DoubleIntegerEventHolder.TaskEvent?.Invoke(1,1);
            FirebaseHandler.I?.LogEvent($"Lvl_cmp_{DBVariablesHolder.LvlIndex.Value}_Star_{LevelsManager.I.LevelStars}");
        }


        async void LoadAppreationText(string path)
        {
            _txtHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _txtHandle.Task;

            if (_txtHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            _textObj = null;
            _textObj = Instantiate(_txtHandle.Result as GameObject, transform);

            await Task.Yield();
            await Task.Yield();
        }

        IEnumerator CaptureColoredArea()
        {
            _tempRT = RenderTexture.GetTemporary(
                150,
                150,
                24
            );
            _tempRT.filterMode = FilterMode.Bilinear;
            _tempRT.wrapMode = TextureWrapMode.Clamp;
            ScreenshotCamera.targetTexture = _tempRT;
            ScreenshotCamera.Render(); 
            int currentLvlNum = LevelsManager.I.TempLvlNum != -1 ? LevelsManager.I.TempLvlNum : DBVariablesHolder.LvlNum.Value;
            int currentLvlIndex = LevelsManager.I.TempLvlIndex != -1 ? LevelsManager.I.TempLvlIndex : DBVariablesHolder.LvlIndex.Value;
            yield return new WaitForSeconds(0.01f);
            // Create a new Texture2D
            Texture2D screenshot = new Texture2D(_tempRT.width, _tempRT.height, TextureFormat.RGB24, false);
            yield return new WaitForSeconds(0.01f);
            // Read the pixels from the RenderTexture
            RenderTexture.active = _tempRT;
            screenshot.ReadPixels(new Rect(0, 0, _tempRT.width, _tempRT.height), 0, 0);
            screenshot.Apply();
            yield return new WaitForSeconds(0.25f);

            DisplayImage.texture = screenshot;
            string directoryPath = Path.Combine(Application.persistentDataPath, "Paintings");
            if (!Directory.Exists(directoryPath))
            { Directory.CreateDirectory(directoryPath); }

            string filePath = Path.Combine(directoryPath, $"Painting_{currentLvlNum}.png");
            File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
            yield return new WaitForSeconds(2.5f);

            SimpleEventsHolder.DestroyLevelEvent?.Invoke();
            OnOpen();
            Invoke(nameof(OnPanelVisible), _durationTweeing);
            for (int s = 0; s < LevelsManager.I.LevelStars; s++)
            {
                StarsImg[s].color = _starColor;
            }
            yield return new WaitForSeconds(0.1f);

            GameData starsData = LoadStars();
            yield return new WaitForSeconds(0.1f);
            LevelData existingLevel = starsData.Levels.Find(l => l.LevelNumber == currentLvlNum);
            if (existingLevel != null)
            {
                existingLevel.Stars = LevelsManager.I.LevelStars; // Save max stars
            }
            else
            {
                starsData.Levels.Add(new LevelData { LevelNumber = currentLvlNum, LevelIndex = currentLvlIndex, Stars = LevelsManager.I.LevelStars });
            }

            string json = JsonUtility.ToJson(starsData, true);
            File.WriteAllText(_starsDataPath, json);
            yield return new WaitForSeconds(0.1f);
            ScreenshotCamera.enabled = false;
            _onceClicked = false;
        }

        void OnPanelVisible()
        {
            StateManager.I.DestroyState(StateManager.I.GamePlayStatePath);
            if (DBVariablesHolder.LvlNum.Value > LevelsManager.I.MinLvlCount)
            {
                NextBtn.DOAnchorPos(_nextBtnPosition, _durationTweeing).SetEase(Ease.InOutBack).OnComplete(() =>
                {
                    CurrentMultiplayerBar.gameObject.SetActive(true);
                    RvBtn.SetActive(true);
                });
            }
            StarsObj.DOAnchorPosY(_starsPos, _durationTweeing).SetEase(Ease.OutBack);
            StarsObj.DOScale(1, _durationTweeing).SetEase(Ease.OutBack).OnComplete(() => {
                DisplayImage.gameObject.SetActive(true);
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(7);
                DisplayImage.transform.DOScale(1, _durationTweeing / 2).SetEase(Ease.OutBack);

            });
            DOTween.To(() => 0, x => LevelBonusText.text = x.ToString(), _levelBonus, _durationTweeing);
            if (LevelsManager.I.TotalMoves > 0)
            { DOTween.To(() => 0, x => MovesBonusText.text = x.ToString(), LevelsManager.I.TotalMoves, _durationTweeing); }
            _starsBonus *= LevelsManager.I.LevelStars;
            DOTween.To(() => 0, x => StarsBonusText.text = x.ToString(), _starsBonus, _durationTweeing);
            _totalBonus = (_levelBonus + LevelsManager.I.TotalMoves + _starsBonus);
            DOTween.To(() => 0, x => TotalBonusText.text = x.ToString(), _totalBonus, _durationTweeing).SetDelay(_durationTweeing);
            TotalBonusText.rectTransform.DOScale(1, _durationTweeing).SetEase(Ease.OutBack).SetDelay(_durationTweeing);
        }

        GameData LoadStars()
        {
            if (File.Exists(_starsDataPath))
            {
                string json = File.ReadAllText(_starsDataPath);
                return JsonUtility.FromJson<GameData>(json);
            }
            return new GameData();
        }

        void OnMultiplyReward()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                LevelsManager.I.CanPlay = false;
                CurrenciesHolder.CashCurrency.Amount += (_totalBonus * CurrentMultiplayerBar.CurrentMultiplier);
                if (LevelsManager.I.TempLvlIndex == -1)
                {
                    DBVariablesHolder.LvlNum.Value++;
                    DBVariablesHolder.LvlIndex.Value++;
                }
                Invoke(nameof(OnClose), 2);
            }
        }

        public void OnClickNext()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                LevelsManager.I.CanPlay = false;
                CurrenciesHolder.CashCurrency.Amount += _totalBonus;
                if (LevelsManager.I.TempLvlIndex == -1)
                {
                    DBVariablesHolder.LvlNum.Value++;
                    DBVariablesHolder.LvlIndex.Value++;
                }
                Invoke(nameof(OnClose), 2);
            }
        }

        public override void OnOpen()
        {
            if (_textObj != null)
                Destroy(_textObj);

           SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
           Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            KillAllTweens();
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => {
                if (DBVariablesHolder.LvlIndex.Value > DBVariablesHolder.MaxLvlCount.Value)
                {
                    DBVariablesHolder.LvlIndex.Value = LevelsManager.I.MinLvlCount;
                }

                if (DBVariablesHolder.LvlNum.Value <= LevelsManager.I.MinLvlCount)
                {
                    SimpleEventsHolder.InitLvlEvent?.Invoke();        
                    StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
                }
                else
                {
                    StateManager.I.ActiveState(StateManager.I.MainMenuStatePath);
                }
                StateManager.I.DestroyState(StateManager.I.LevelCompleteStatePath);
            });
        }

        void KillAllTweens()
        {
            DOTween.Kill(NextBtn.transform);
            DOTween.Kill(StarsObj.transform);
            DOTween.Kill(DisplayImage.transform);
            DOTween.Kill(LevelBonusText.text);
            DOTween.Kill(StarsBonusText.text);
            DOTween.Kill(MovesBonusText.text);
            DOTween.Kill(TotalBonusText.text);
        }
        async Task ReleaseTextHandleSafelyAsync()
        {
            if (_txtReleaseInProgress)
                return;

            _txtReleaseInProgress = true;

            // wait one frame – Unity safe point
            await Task.Yield();

            if (_txtHandle.IsValid())
            {
                Addressables.Release(_txtHandle);
                _txtHandle = default;
            }

            // optional extra frame (Android / scene safety)
            await Task.Yield();

            _txtReleaseInProgress = false;
        }

    }
}