using Core.Events;
using UnityEngine;
using Core.DB.Variables;
using Core.Plugins.Firebase;
using System.Collections.Generic;

namespace Core.GamePlay.WaterSort
{
    public class SortUndoManager : MonoBehaviour
    {
        Stack<UndoData> _undoMoves = new Stack<UndoData>();

        public void AddUndo(TubeHandler senderTube, TubeHandler getterTube, int liquidLayers)
        {
            SimpleEventsHolder.RegisterMoveEvent?.Invoke();
            UndoData newUndo = new UndoData();
            newUndo.SenderTube = senderTube;
            newUndo.GetterTube = getterTube;
            newUndo.LiquidLayers = liquidLayers;
            _undoMoves.Push(newUndo);
        }

        private void OnEnable()
        {
            SimpleEventsHolder.UndoEvent += OnUndoBtnClick;
            SimpleEventsHolder.RestartLevelEvent += RestForNewLevel;
            SimpleEventsHolder.InitLvlEvent += RestForNewLevel;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.UndoEvent -= OnUndoBtnClick;
            SimpleEventsHolder.RestartLevelEvent -= RestForNewLevel;
            SimpleEventsHolder.InitLvlEvent -= RestForNewLevel;
        }

        void RestForNewLevel()
        {
            _undoMoves.Clear();
        }

        void OnUndoBtnClick()
        {
            if (_undoMoves.Count > 0 && !LevelsManager.I.DoingUndo && !LevelsManager.I.UsingAnyFeature && LevelsManager.I.CanPlay)
            {
                LevelsManager.I.DoingUndo = true;
                LevelsManager.I.UsingAnyFeature = true;
                UndoData lastMove = new UndoData();
                lastMove = _undoMoves.Pop();
                if (!lastMove.GetterTube.IsBussy && !lastMove.SenderTube.IsBussy)
                {
                    if (LevelsManager.I.Tube != null)
                    {
                        LevelsManager.I.Tube.MoveBackIn();
                    }
                    LevelsManager.I.Tube = lastMove.GetterTube;
                    LevelsManager.I.Tube.RemoveFromCompleted();
                    lastMove.SenderTube.UndoWater(lastMove.GetterTube, lastMove.LiquidLayers);
                    if (DBVariablesHolder.LvlNum.Value >= LevelsManager.I.MinLvlCount)
                    {
                        SimpleEventsHolder.UpdateUndoStatusEvent?.Invoke();
                    }
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(3, 1);
                    FirebaseHandler.I?.LogEvent($"undo_lvl_{DBVariablesHolder.LvlIndex.Value}");
                }
                else
                {
                    _undoMoves.Push(lastMove);
                }
            }
            else if (_undoMoves.Count < 1)
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(2);
            }
            else if (LevelsManager.I.UsingAnyFeature)
            {
                SingleIntegerEventsHolder.ShowToastEvent?.Invoke(4);
            }
        }
    }
}
