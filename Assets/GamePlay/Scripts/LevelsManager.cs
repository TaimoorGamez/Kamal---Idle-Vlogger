using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DB.Variables;
using System.Collections;
using Core.Plugins.Firebase;
using System.Threading.Tasks;
using Core.GamePlay.Coloring;
using Core.GamePlay.WaterSort;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay
{
    public class LevelsManager : MonoBehaviour
    {
        public static LevelsManager I { get; private set; }

        public SortUndoManager UndoManager;
        public ColorSwapManager ColorSwaper;
        public Color32 CurrentColor;
        public TubeHandler Tube;
        public BowlColorHandler CurrentBowl;
        public int MinLvlCount = 5, LevelStars = 3, TotalMoves, CurrrentLvl, TempLvlIndex = -1, TempLvlNum = -1, CompletedTubes;
        public bool IsHiddenLevel = false, CanPlay = false, BtnOnceClicked = false, SortingCompleted, UsingAnyFeature = false, 
                    IsSwaping  = false, DoingUndo = false;

        [SerializeField] Vector3[] TubePositions, BowlPositions;
        [SerializeField] BowlColorHandler BowlObj;

        string _sortingLvlPath = "Level/Sort/";
        string _tubePath = "GamePlay/WaterSort/Tube";
        TubeHandler _tubePrefab;
        List<TubeHandler> _colorTubes = new List<TubeHandler>();
        List<TubeHandler> _totalTubes = new List<TubeHandler>();
        Coroutine lvlMakingRotine;
        int _totalTubesCount = 0, _maxColorsInTube = 4;
        SOColors _levelColors;
        Dictionary<int, List<Color32>> _currentLevelColors = new Dictionary<int, List<Color32>>();
        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        AsyncOperationHandle _lvlHandle, _tubeHandle;
        bool _releaseInProgress = false;

        private void OnEnable()
        {
            SimpleEventsHolder.InitLvlEvent += InitNewLevel;
            SimpleEventsHolder.ExtraTubeEvent += OnAddTubeClick;
            SimpleEventsHolder.RestartLevelEvent += RestartLevel;
            SimpleEventsHolder.DestroyLevelEvent += DestroyLevel;
            SimpleEventsHolder.StartColoringEvent += ColoringPreparation;
            SimpleEventsHolder.CheckCompleteEvent = CheckComplete;
            SimpleEventsHolder.RegisterMoveEvent = CheckMoves;
        }

        private async void OnDisable()
        {
            SimpleEventsHolder.InitLvlEvent -= InitNewLevel;
            SimpleEventsHolder.ExtraTubeEvent -= OnAddTubeClick;
            SimpleEventsHolder.RestartLevelEvent -= RestartLevel;
            SimpleEventsHolder.DestroyLevelEvent -= DestroyLevel;
            SimpleEventsHolder.StartColoringEvent -= ColoringPreparation;
            await ReleaseAllHandlesSafelyAsync();
        }

        private void Start()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            LoadWaterSortTube();
        }

        void InitNewLevel()
        {
            CanPlay = false;
            _colorTubes.Clear();
            _totalTubes.Clear();
            DestroyLevel();
            if (DBVariablesHolder.LvlNum.Value % 5 == 0)
            {
                IsHiddenLevel = true;
            }
            else if (IsHiddenLevel)
            {
                IsHiddenLevel = false;
            }

            if (TempLvlIndex != -1 && TempLvlNum != -1)
            {
                LoadAddressableLevels<SOColors>(_sortingLvlPath + TempLvlIndex);
            }
            else
            {
                if (DBVariablesHolder.LvlIndex.Value < 5)
                    LoadAddressableLevels<GameObject>(_sortingLvlPath + DBVariablesHolder.LvlIndex.Value);
                else
                    LoadAddressableLevels<SOColors>(_sortingLvlPath + DBVariablesHolder.LvlIndex.Value);
            }
        }

        async void LoadWaterSortTube()
        {
            _tubeHandle = Addressables.LoadAssetAsync<GameObject>(_tubePath);
            await _tubeHandle.Task;

            if (_tubeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {_tubePath}");
                return;
            }
            _tubePrefab = null;
            GameObject tubeObj = _tubeHandle.Result as GameObject;
            _tubePrefab = tubeObj.GetComponent<TubeHandler>();

            await Task.Yield();  // frame 1

            while (_tubePrefab == null)
                await Task.Yield();
        }

        async void LoadAddressableLevels<T>(string path)
        {
            if (_lvlHandle.IsValid())
            {
                Addressables.Release(_lvlHandle);
                _lvlHandle = default;
            }
            await Task.Yield();

            _lvlHandle = Addressables.LoadAssetAsync<T>(path);
            await _lvlHandle.Task;

            if (_lvlHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Failed to load Addressable prefab at: {path}");
                return;
            }

            if (DBVariablesHolder.LvlIndex.Value < 5)
            {
                GameObject lvlObj = Instantiate(_lvlHandle.Result as GameObject, transform);
                CurrrentLvl = DBVariablesHolder.LvlIndex.Value;

                await Task.Yield();  // frame 1
                await Task.Yield();  // frame 2

                while (lvlObj == null)
                    await Task.Yield();
            }
            else 
            {
                _levelColors = null;
                _levelColors = _lvlHandle.Result as SOColors;
                CurrrentLvl = _levelColors.Colors.Length;
                _totalTubesCount = CurrrentLvl + 2;
                lvlMakingRotine = StartCoroutine(GenerateLvl());
                TotalMoves = CurrrentLvl * _maxColorsInTube;

                await Task.Yield();  // frame 1
                await Task.Yield();  // frame 2

                while (_levelColors == null)
                    await Task.Yield();
            }
            await Task.Yield();

            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
        }

        IEnumerator GenerateLvl()
        {
            FirebaseHandler.I?.LogEvent($"WLvl_str_{DBVariablesHolder.LvlIndex.Value}");
            for (int t = 0; t < _totalTubesCount; t++)
            {
                TubeHandler newTube = Instantiate(_tubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                _totalTubes.Add(newTube);
                if (t < CurrrentLvl)
                {
                    _colorTubes.Add(newTube);
                }
            }
            List<TubeHandler> tempTubes = new List<TubeHandler>();
            foreach (TubeHandler tube in _colorTubes)
            {
                tempTubes.Add(tube);
            }
            yield return new WaitForSeconds(0.25f);
            for (int l = 0; l < CurrrentLvl; l++)
            {
                for (int b = 0; b < _maxColorsInTube; b++)
                {
                    int tubeNum = Random.Range(0, _colorTubes.Count);

                    if (_colorTubes[tubeNum].WaterColors.Count < _maxColorsInTube)
                    {
                        if (IsHiddenLevel)
                        {
                            _colorTubes[tubeNum].SetHidenColour(_levelColors.Colors[l]);
                        }
                        else
                        {
                            _colorTubes[tubeNum].SetColor(_levelColors.Colors[l]);
                        }

                    }
                    yield return new WaitForSeconds(0.01f);
                    if (_colorTubes[tubeNum].WaterColors.Count == _maxColorsInTube)
                    {
                        _colorTubes[tubeNum].WaterAdded();
                        _colorTubes.RemoveAt(tubeNum);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            for (int c = 0; c < tempTubes.Count; c++)
            {
                _currentLevelColors[c] = new List<Color32>();
                foreach (Color col in tempTubes[c].WaterColors)
                {
                    _currentLevelColors[c].Add(col);
                }
            }
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            CanPlay = true;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

        void RestartLevel()
        {
            if (!BtnOnceClicked)
            {
                BtnOnceClicked = true;
                CanPlay = false;
                lvlMakingRotine = StartCoroutine(ReGenerateLevel());
                FirebaseHandler.I?.LogEvent($"WLvl_rst_{DBVariablesHolder.LvlIndex.Value}");
            }
        }

        IEnumerator ReGenerateLevel()
        {
            DestroyLevel();
            yield return new WaitForSeconds(0.5f); 
            _totalTubesCount = CurrrentLvl + 2;
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(1);
            for (int t = 0; t < _totalTubesCount; t++)
            {
                TubeHandler newTube = Instantiate(_tubePrefab, transform);
                newTube.transform.position = TubePositions[t];
                if (t < CurrrentLvl)
                {
                    List<Color32> tubeColors = _currentLevelColors[t];
                    yield return new WaitForSeconds(0.015f);
                    for (int c = 0; c < _maxColorsInTube; c++)
                    {
                        if (IsHiddenLevel)
                        {
                            newTube.SetHidenColour(tubeColors[c]);
                        }
                        else
                        {
                            newTube.SetColor(tubeColors[c]);
                        }
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            SingleIntegerEventsHolder.SwitchProtectorEvent?.Invoke(0);
            CanPlay = true;
            BtnOnceClicked = false;
            if (lvlMakingRotine != null)
            {
                StopCoroutine(lvlMakingRotine);
            }
        }

        void OnAddTubeClick()
        {
            if (_totalTubesCount < 10 && CanPlay)
            {
                TubeHandler newTube = Instantiate(_tubePrefab, transform);
                newTube.transform.position = TubePositions[_totalTubesCount];
                _totalTubesCount++;
                _totalTubes.Add(newTube);
                SimpleEventsHolder.UpdateExtraTubeStatusEvent?.Invoke();
                DoubleIntegerEventHolder.TaskEvent?.Invoke(5, 1);
                FirebaseHandler.I?.LogEvent($"ExTube_lvl_{DBVariablesHolder.LvlIndex.Value}");
            }
            else if(_totalTubes.Count >= 10)
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(5);
            }
        }

        void CheckComplete()
        {
            if (CompletedTubes == CurrrentLvl)
            {
                CompletedTubes = 0;
                SortingCompleted = true;
                SimpleEventsHolder.StartColoringEvent?.Invoke();
            }
        }

        void CheckMoves()
        {
            TotalMoves--;
            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            for (int t = 0; t < _totalTubes.Count; t++)
            {
                TubeHandler currentTube = _totalTubes[t];
                if (currentTube.WaterColors.Count > 0)
                {
                    currentTube.TubeCap.gameObject.SetActive(false);
                    currentTube.transform.DOKill();
                    currentTube.transform.DOScale(_bowlScale, tweenTime);
                    currentTube.transform.DOLocalMove(BowlPositions[t], tweenTime).OnKill(() =>
                    {
                        BowlColorHandler colorBowl = Instantiate(BowlObj, transform);
                        colorBowl.transform.position = currentTube.transform.position;
                        colorBowl.SetColor(currentTube.CurrentColor);
                        Destroy(currentTube.gameObject, 0.15f);
                    });
                }
                else
                {
                        Destroy(currentTube.gameObject);
                }
            }
            FirebaseHandler.I?.LogEvent($"color_lvl_str_{DBVariablesHolder.LvlIndex.Value}");
        }

        void DestroyLevel()
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        async Task ReleaseAllHandlesSafelyAsync()
        {
            // prevent double release
            if (_releaseInProgress)
                return;

            _releaseInProgress = true;


            // STEP 2: wait one frame (Unity safe point)
            await Task.Yield();

            // STEP 3: release level handle
            if (_lvlHandle.IsValid())
            {
                Addressables.Release(_lvlHandle);
                _lvlHandle = default;
            }

            // STEP 4: release tube handle
            if (_tubeHandle.IsValid())
            {
                Addressables.Release(_tubeHandle);
                _tubeHandle = default;
            }

            // STEP 5: optional extra frame (Android lifecycle safety)
            await Task.Yield();

            _releaseInProgress = false;
        }

    }
}