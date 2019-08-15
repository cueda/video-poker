using System;

/// Eventing system for Video Poker game.
/// Broadcasts events when called anywhere in the application.
/// Functions can be added to these delegates to be called at the appropriate time
/// (but timing is not guaranteed)
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class EventManager
{
    public static class Game
    {
        public static Action OnAddBetMax;
        public static Action OnAddBetOne;
        public static Action OnGameReadyToStart;
        public static Action OnGameStart;
        public static Action OnGameWaitingForHold;
        public static Action OnRedeal;
        public static Action OnGameEnd;
        public static Action OnGamePayout;
    }


    public static class Card
    {
        // Card index drawn (0-4)
        public static Action<int> OnCardHeld;
    }


    public static class Animation
    {
        // Old credits value, new credits value
        public static Action<int, int> OnStartAnimatePayout;
        public static Action OnDealAnimComplete;
        public static Action OnPayoutAnimComplete;

        public static Action OnOneCardDrawAnimated;
        public static Action OnOneCreditPayoutAnimated;
    }


    public static class Credits
    {
        // Old credits value, new credits value
        public static Action<int, int> OnBetPlaced;
        // Old bet value, new bet value
        public static Action<int, int> OnCurrentBetValueChanged;
    }


    public static class Input
    {
        // Button index (0-4)
        public static Action<int> OnHoldButtonPressed;
        public static Action OnAddBetOneButtonPressed;
        public static Action OnAddBetMaxButtonPressed;
        public static Action OnDealButtonPressed;
    }
}
