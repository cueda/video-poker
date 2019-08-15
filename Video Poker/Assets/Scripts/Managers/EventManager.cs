using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        public static Action<int> OnCardDrawn;
        public static Action<int> OnCardHeld;
    }


    public static class Animation
    {
        public static Action<int, int> OnStartAnimatePayout;
        public static Action OnDealAnimComplete;
        public static Action OnPayoutAnimComplete;
    }


    public static class Credits
    {
        public static Action<int, int> OnBetPlaced;
        public static Action<int, int> OnCurrentBetValueChanged;
    }


    public static class Input
    {
        public static Action<int> OnHoldButtonPressed;
        public static Action OnAddBetOneButtonPressed;
        public static Action OnAddBetMaxButtonPressed;
        public static Action OnDealButtonPressed;
    }
}
