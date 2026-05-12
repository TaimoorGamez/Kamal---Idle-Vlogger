using System;
using System.Collections.Generic;

namespace Core.Events
{
    public delegate void GameEvent();

    public delegate void GameEventInteger(int val);

    public delegate void GameEventWith2Ints(int index, double val);

    public static class SimpleEventsHolder
    {
        public static GameEvent

        //-------------------Game Flow Events-------------------
        SelfDestructionEvent, StoryPartComplete, CheckPluginStatus,
        UpdatePriceTxt, UpdateMapProgress, StopStreaming,

        //-------------------Economy Events-------------------
        UpdateCashTxtEvent, UpdateGoldTxtEvent, UpdateSubscribeTxtEvent,

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
        //-------------------Game Flow Events-------------------
        UpdateItemEvent,

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
        public static Dictionary<string, GameEvent> RewardPowerEvent = new Dictionary<string, GameEvent>(StringComparer.Ordinal)
        {
            { "SortUndo", SimpleEventsHolder.RewardUndoEvent },
            { "SwapColor", SimpleEventsHolder.RewardSwapColor },
            { "ExtraTube", SimpleEventsHolder.RewardExtraTubeEvent }
        };
    }
}
