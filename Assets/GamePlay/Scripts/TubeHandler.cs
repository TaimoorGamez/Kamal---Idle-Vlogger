using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Collections;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class TubeHandler : MonoBehaviour
    {
        public bool IsBussy = false;
        public List<Color> WaterColors = new List<Color>();
        public Color CurrentColor;
        public CapHandler TubeCap;

        [SerializeField] ParticleSystem WaveParticle, DropsParticle, CompleteParticle;
        [SerializeField] LineRenderer WaterLine;
        [SerializeField] Transform AnchorPos1, AnchorPos2;
        [SerializeField] Vector3[] ParticlePos;
        [SerializeField] CapsuleCollider TubeCollider;
        [SerializeField] Liquid[] MyLiquid;
        [SerializeField] GameObject[] HidenMarks;
        [SerializeField] Animation TubeAnimation;

        List<Coroutine> _drinkingRotine = new List<Coroutine>();
        TubeHandler _senderTube;
        Vector3 _orignalPos;
        int _totalLiquidLayers = 4, _colorsToUndo = 1;
        bool _isDrinkingWater = false, _tubeCompleted = false, _alreadyAddedToCompleted = false;
        Coroutine _celebrationRotine, _addingRotine, _removingRotine;
        MaterialPropertyBlock _propertyBlock;
        ParticleSystem.MainModule _pM;

        private void Start()
        {
            _orignalPos = transform.position;
            _propertyBlock = new MaterialPropertyBlock();
            _pM = DropsParticle.main;
        }

        public void SetHidenColour(Color currentColor)
        {
            if (WaterColors.Count < _totalLiquidLayers - 1)
            {
                MyLiquid[WaterColors.Count].SetHidenColour();
                HidenMarks[WaterColors.Count].gameObject.SetActive(true);
            }
            SetColor(currentColor);
        }

        public void SetColor(Color currentColor)
        {
            MyLiquid[WaterColors.Count].SetColor(currentColor);
            WaveParticle.transform.localPosition = ParticlePos[WaterColors.Count];
            DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count];
            WaterColors.Add(currentColor);
            CurrentColor = currentColor;
        }

        private void OnMouseDown()
        {
            if (!LevelsManager.I.DoingUndo && !LevelsManager.I.IsSwaping && LevelsManager.I.CanPlay)
            {
                if (LevelsManager.I.Tube == null && !IsBussy && !_isDrinkingWater)
                {
                    if (WaterColors.Count > 0)
                    {
                        TubeState(true);
                        LevelsManager.I.Tube = this;
                    }
                    else
                    {
                        SingleIntegerEventsHolder.ShowToastEvent?.Invoke(0);
                    }
                }
                else
                {
                    if (LevelsManager.I.Tube == this && !IsBussy)
                    {
                        LevelsManager.I.Tube = null;
                        MoveBackIn();
                    }
                    else
                    {
                        if (!IsBussy && LevelsManager.I.Tube != null)
                        {
                            _senderTube = LevelsManager.I.Tube;
                            if (WaterColors.Count < 1)
                            {
                                DrinkWater();
                            }
                            else if (WaterColors.Count < _totalLiquidLayers)
                            {
                                if (_senderTube.CurrentColor == CurrentColor)
                                {
                                    DrinkWater();
                                }
                                else
                                {
                                    SingleIntegerEventsHolder.ShowToastEvent?.Invoke(6);
                                    LevelsManager.I.Tube.MoveBackIn();
                                    LevelsManager.I.Tube = null;
                                }
                            }
                            else
                            {
                                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(7);
                                LevelsManager.I.Tube.MoveBackIn();
                                LevelsManager.I.Tube = null;
                            }
                        }
                    }
                }
            }
            else if (LevelsManager.I.IsSwaping && LevelsManager.I.CanPlay && WaterColors.Count > 0)
            {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(0);
                LevelsManager.I.ColorSwaper.AddTubeForSwaping(this);
            }
        }

        public void MoveBackIn()
        {
            IsBussy = false;
            TubeState(false);
        }

        public void DrinkWater()
        {
            IsBussy = true;
            _isDrinkingWater = true;
            _senderTube.IsBussy = true;
            LevelsManager.I.Tube = null;
            _drinkingRotine.Add(StartCoroutine(ChangingWater(_senderTube)));
        }

        public void UndoWater(TubeHandler senderTube, int liquidLayers)
        {
            IsBussy = true;
            _isDrinkingWater = true;
            senderTube.IsBussy = true;
            _colorsToUndo = liquidLayers;
            LevelsManager.I.Tube = null;
            _drinkingRotine.Add(StartCoroutine(ChangingWater(senderTube)));
        }

        private IEnumerator ChangingWater(TubeHandler senderTube)
        {
            int randomDirection = Random.Range(0, 2);
            Transform anchorPos = AnchorPos1;
            if (randomDirection == 0)
            {
                anchorPos = AnchorPos2;
            }
            int colorsToAdd = 1;
            senderTube.transform.DOMove(anchorPos.position, 0.1f);
            senderTube.transform.DORotate(anchorPos.eulerAngles, 0.1f).OnComplete(() =>
            {
                if (WaterColors.Count < _totalLiquidLayers - 1 && !LevelsManager.I.DoingUndo)
                {
                    int sameColors = 1;
                    for (int i = senderTube.WaterColors.Count - 1; i > 0; i--)
                    {
                        if (senderTube.WaterColors[i] == senderTube.WaterColors[i - 1])
                        {
                            if (LevelsManager.I.IsHiddenLevel)
                            {
                                if (!senderTube.GetHidenColor(i - 1))
                                    sameColors++;
                            }
                            else
                            {
                                sameColors++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (sameColors > 1)
                    {
                        int remaingLayers = _totalLiquidLayers - WaterColors.Count;
                        if (sameColors <= remaingLayers)
                        {
                            colorsToAdd = sameColors;
                        }
                        else
                        {
                            colorsToAdd = remaingLayers;
                        }
                    }
                }

                if (LevelsManager.I.DoingUndo)
                {
                    colorsToAdd = _colorsToUndo;
                }
                AddColor(senderTube.CurrentColor, colorsToAdd);
                senderTube.RemoveColor(colorsToAdd);
                if (!LevelsManager.I.DoingUndo)
                {
                    LevelsManager.I.UndoManager.AddUndo(senderTube, this, colorsToAdd);
                }
                IsBussy = false;
            });
            yield return new WaitForSeconds(1f);
            WaterLine.gameObject.SetActive(false);
            senderTube.TubeState(false);
            yield return new WaitForSeconds(0.25f);
            if (LevelsManager.I.IsHiddenLevel)
            {
                senderTube.RevelColour();
            }
            WaterAdded();
            yield return new WaitForSeconds(0.1f);
            senderTube.IsBussy = false;
            if (LevelsManager.I.DoingUndo)
            {
                yield return new WaitForSeconds(0.05f);
                LevelsManager.I.DoingUndo = false;
                LevelsManager.I.UsingAnyFeature = false;
            }
        }

        public void WaterAdded()
        {
            _isDrinkingWater = false;
            if (WaterColors.Count == _totalLiquidLayers)
            {
                _tubeCompleted = true;
                for (int i = _totalLiquidLayers - 1; i > 0; i--)
                {
                    if (WaterColors[i] != WaterColors[i - 1])
                    {
                        _tubeCompleted = false;
                        break;
                    }
                }
                if (!_alreadyAddedToCompleted && _tubeCompleted)
                {
                    _alreadyAddedToCompleted = true;
                    TubeCollider.enabled = false;
                    if (LevelsManager.I.IsHiddenLevel)
                    {
                        RevelFullTube();
                    }
                    _celebrationRotine = StartCoroutine(CelebrationOnComplete());
                }
            }
        }

        IEnumerator CelebrationOnComplete()
        {
            yield return new WaitForSeconds(0.5f);
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(4);
            CompleteParticle.Play();
            TubeCap.PlayCelebration(CurrentColor);
            yield return new WaitForSeconds(2f);
            LevelsManager.I.CompletedTubes++;
            SimpleEventsHolder.CheckCompleteEvent?.Invoke();
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
        }

        public void RemoveFromCompleted()
        {
            if (_tubeCompleted && _alreadyAddedToCompleted)
            {
                _tubeCompleted = false;
                LevelsManager.I.CompletedTubes--;
                _alreadyAddedToCompleted = false;
                TubeCollider.enabled = true;
                TubeCap.HideCap();
            }
        }
        public void RemoveColor(int layers)
        {
            _removingRotine = StartCoroutine(ColorRemovingCorotine(layers));
        }

        IEnumerator ColorRemovingCorotine(int layers)
        {
            WaveParticle.Stop();
            TubeAnimation.Play("Liquid " + WaterColors.Count + "" + layers);
            for (int c = 1; c <= layers; c++)
            {
                yield return new WaitForSeconds((float)1 / layers);
                MyLiquid[WaterColors.Count - 1].HideColor();
                WaterColors.RemoveAt(WaterColors.Count - 1);
            }
            TubeAnimation.Play("LiquidDefault");
            if (WaterColors.Count > 0)
            {
                DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count - 1];
                CurrentColor = WaterColors[WaterColors.Count - 1];
            }
            yield return new WaitForSeconds(0.1f);
            if (_removingRotine != null)
            {
                StopCoroutine(_removingRotine);
            }
        }

        public void SwapeColor(Color currentColor)
        {
            WaterColors[WaterColors.Count - 1] = currentColor;
            CurrentColor = currentColor;
            MyLiquid[WaterColors.Count-1].ChangeColor(currentColor);
        }

        void TubeState(bool state)
        {
            if (state)
            {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(0);
                MyLiquid[WaterColors.Count-1].SetGlow(true);
                if (!LevelsManager.I.IsHiddenLevel)
                {
                    for (int c = 1; c < WaterColors.Count; c++)
                    {
                        if (WaterColors[c] == WaterColors[c - 1])
                        {
                            MyLiquid[c - 1].SetGlow(true);
                        }
                    }
                }
                _propertyBlock.SetColor("_BaseColor", CurrentColor);
                _pM.startColor = CurrentColor;
                WaveParticle.Play();
                transform.DOLocalMoveY(_orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                WaveParticle.Stop();
                for (int c = 0; c < MyLiquid.Length; c++)
                {
                    MyLiquid[c].SetGlow(false);
                }
                transform.DOLocalMove(_orignalPos, 0.05f);
                transform.DOLocalRotate(Vector3.zero, 0.05f).OnComplete(() => transform.position = _orignalPos);
            }
        }

        void AddColor(Color currentColor, int layers)
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
            _propertyBlock.SetColor("_BaseColor", currentColor);
            WaterLine.SetPropertyBlock(_propertyBlock);
            _pM.startColor = currentColor;
            CurrentColor = currentColor;
            _addingRotine = StartCoroutine(ColorAdddingCorotine(currentColor,layers));
        }

        IEnumerator ColorAdddingCorotine(Color currentColor, int layers)
        {
            float duration = (float)1 / layers;
            Vector3 startPosition = DropsParticle.transform.localPosition;
            WaterLine.gameObject.SetActive(true);
            DropsParticle.Play();
            for (int c = 1; c <= layers; c++)
            {
                MyLiquid[WaterColors.Count].SmoothlyAddColor(currentColor, duration);
                DropsParticle.transform.DOLocalMove(ParticlePos[WaterColors.Count], duration);
                yield return duration;
                WaterColors.Add(currentColor);
            }
            DropsParticle.Stop();
            DropsParticle.transform.localPosition = ParticlePos[WaterColors.Count-1];

            if (_addingRotine != null)
            {
                StopCoroutine( _addingRotine );
            }
        }

        public void RevelColour()
        {
            if (WaterColors.Count > 0)
            {
                HidenMarks[WaterColors.Count - 1].SetActive(false);
                MyLiquid[WaterColors.Count - 1].RevelColour();
            }
        }

        void RevelFullTube()
        {
            for (int h = 0; h < HidenMarks.Length; h++ )
            {
                HidenMarks[h].SetActive(false);
                MyLiquid[h].RevelColour();
            }
        }

        bool GetHidenColor(int i)
        {
            return HidenMarks[i].gameObject.activeInHierarchy;
        }

        private void OnDisable()
        {
            foreach (Coroutine rotine in _drinkingRotine)
            {
                if (rotine != null)
                {
                    StopCoroutine(rotine);
                }
            }
            if (_celebrationRotine != null)
            {
                StopCoroutine(_celebrationRotine);
            }
            if (_removingRotine != null)
            {
                StopCoroutine(_removingRotine);
            }
        }
    }
}
