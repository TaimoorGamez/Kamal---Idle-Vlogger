using System;
using System.Collections.Generic;

namespace Core.Events
{
    public delegate void GameEvent();

    public delegate void GameEventInteger(int val);

    public delegate void GameEventWith2Ints(int index, int val);

    public static class SimpleEventsHolder
    {
        public static GameEvent

        //-------------------Game Flow Events-------------------
        SelfDestructionEvent, HideColorBowlEvent, CheckCompleteEvent,
        StartColoringEvent, ColorSelectedEvent, DestroyLevelEvent, InitLvlEvent,
        UpdateMovesEvent, RestartLevelEvent, ExtraTubeEvent,
        UpdateExtraTubeStatusEvent, RegisterMoveEvent, UndoEvent, UpdateUndoStatusEvent,
        SwapColorsEvent, UpdateSwapStatusEvent, UpdatePowerBtnsUIEvent, CheckPluginStatus,

        //-------------------Sound Events-------------------
        BtnPressSfxEvent, UpdateMusicStateEvent, UpdateSoundStateEvent,

        //-------------------Spin Wheel Events-------------------
        ResetSpinWheelEvent,

        //-------------------Daily Reward Events-------------------
        UpDateDailyRewardState,

        //-------------------Daily Tasks Events-------------------
        GenerateDailyTasksEvent,

        //------------------Ads Events------------------
        RemoveAds, StartCountingAdBreak, GrantRewardEvent,
        MultiplayRewardEvent, AddMovesEvent, DoubleDailyRewardEvent,
        RewardSpinWheelEvent, BuyCaps, BuySprays, BuyFlames, AdsBlockerEvent,
        RewardUndoEvent, RewardExtraTubeEvent, RewardSwapColor;
    }

    public static class SingleIntegerEventsHolder
    {
        public static GameEventInteger

        //-------------------Economy Events-------------------
        DepositEvent, TransactionEvent,

        //-------------------Game Flow Events-------------------
        SwitchProtectorEvent,

        //-------------------Toast Events-------------------
        ShowToastEvent,

        //-------------------Sound Events-------------------
        SoundEffectEvent;
    }

    public static class DoubleIntegerEventHolder
    {
        public static GameEventWith2Ints

        //-------------------DailyTask Events-------------------
        TaskEvent;
    }

    public static class EventDictionariesHolder
    {

        public static Dictionary<string, GameEvent> PowerEvents = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "SortUndo", SimpleEventsHolder.UndoEvent },
            { "SwapColor", SimpleEventsHolder.SwapColorsEvent },
            { "ExtraTube", SimpleEventsHolder.ExtraTubeEvent }
        };

        public static Dictionary<string, GameEvent> UpdatePowerStatusEvent = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "SortUndo", SimpleEventsHolder.UpdateUndoStatusEvent },
            { "SwapColor", SimpleEventsHolder.UpdateSwapStatusEvent },
            { "ExtraTube", SimpleEventsHolder.UpdateExtraTubeStatusEvent }
        };

        public static Dictionary<string, GameEvent> RewardPowerEvent = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "SortUndo", SimpleEventsHolder.RewardUndoEvent },
            { "SwapColor", SimpleEventsHolder.RewardSwapColor },
            { "ExtraTube", SimpleEventsHolder.RewardExtraTubeEvent }
        };
    }
}
