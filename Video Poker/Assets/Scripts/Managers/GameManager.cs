using System.Collections;
using UnityEngine;

/// The main mega-object for game logic.
/// Primarily keeps track of game state (betting, dealing, holding, redealing, payout) and holds global game values.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int startingCredits = 100;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private static int _credits;
    public static int Credits
    {
        get { return _credits; }
        set
        {
            int old = _credits;
            _credits = value;
            if(old >= _credits)
            {
                EventManager.Credits.OnBetPlaced?.Invoke(old, _credits);
            }
        }
    }

    private static int _currentBet;
    public static int CurrentBet
    {
        get { return _currentBet; }
        set
        {
            int old = _currentBet;
            _currentBet = value;
            EventManager.Credits.OnCurrentBetValueChanged?.Invoke(old, _currentBet);


        }
    }

    // At most one of these should be true at any time - use to determine when player can interact
    private bool isGameReadyToStart = false;
    private bool isGameStarting = false;
    private bool isGameWaitingForHold = false;
    private bool isGameRedealing = false;

    // For proper "Bet One" button implementation
    // Toggles after a game ends and allowes "Bet One" to reset to 1
    private bool shouldResetCurrentBet = false;


    private void Awake()
    {
        // Singleton enforcement for GameObject
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    private void OnEnable()
    {
        EventManager.Game.OnGameReadyToStart += OnGameReadyToStart;
        EventManager.Game.OnGameStart += OnGameStart;
        EventManager.Game.OnGameWaitingForHold += OnGameWaitingForHold;
        EventManager.Game.OnGameEnd += OnGameEnd;
        EventManager.Game.OnGamePayout += OnGamePayout;

        EventManager.Animation.OnDealAnimComplete += OnDealAnimComplete;
        EventManager.Animation.OnPayoutAnimComplete += OnPayoutAnimComplete;

        EventManager.Input.OnHoldButtonPressed += OnHoldButtonPressed;
        EventManager.Input.OnAddBetOneButtonPressed += OnAddBetOneButtonPressed;
        EventManager.Input.OnAddBetMaxButtonPressed += OnAddBetMaxButtonPressed;
        EventManager.Input.OnDealButtonPressed += OnDealButtonPressed;
}


    private void OnDisable()
    {
        EventManager.Game.OnGameReadyToStart -= OnGameReadyToStart;
        EventManager.Game.OnGameStart -= OnGameStart;
        EventManager.Game.OnGameWaitingForHold -= OnGameWaitingForHold;
        EventManager.Game.OnGameEnd -= OnGameEnd;

        EventManager.Animation.OnDealAnimComplete -= OnDealAnimComplete;
        EventManager.Animation.OnPayoutAnimComplete -= OnPayoutAnimComplete;

        EventManager.Input.OnHoldButtonPressed -= OnHoldButtonPressed;
        EventManager.Input.OnAddBetOneButtonPressed -= OnAddBetOneButtonPressed;
        EventManager.Input.OnAddBetMaxButtonPressed -= OnAddBetMaxButtonPressed;
        EventManager.Input.OnDealButtonPressed -= OnDealButtonPressed;
    }


    private void Start()
    {
        _credits = startingCredits;
        Credits = startingCredits;
        CurrentBet = 0;
        EventManager.Game.OnGameReadyToStart?.Invoke();
    }


    private void OnGameReadyToStart()
    {
        // Delay flag update for one frame to ensure other actions requiring flag are performed first
        Deck.Instance.ShuffleDeck();
        StartCoroutine(DelayStartFlagUpdateOneFrame());
    }


    private IEnumerator DelayStartFlagUpdateOneFrame()
    {
        yield return new WaitForEndOfFrame();
        isGameReadyToStart = true;
    }


    private void OnGameStart()
    {
        Credits -= CurrentBet;
    }


    private void OnGameWaitingForHold()
    {
        // Delay flag update for one frame to ensure other actions requiring flag are performed first
        StartCoroutine(DelayHoldFlagUpdateOneFrame());
    }


    private IEnumerator DelayHoldFlagUpdateOneFrame()
    {
        yield return new WaitForEndOfFrame();
        isGameStarting = false;
        isGameWaitingForHold = true;
    }


    private void OnGameEnd()
    {
        // Delay flag update for one frame to ensure other actions requiring flag are performed first
        StartCoroutine(DelayRedealFlagUpdateOneFrame());
        EventManager.Game.OnGamePayout?.Invoke();

    }


    private IEnumerator DelayRedealFlagUpdateOneFrame()
    {
        yield return new WaitForEndOfFrame();
        isGameRedealing = false;
        shouldResetCurrentBet = true;
    }


    private void OnGamePayout()
    {
        // TODO: rewarding even without win???

        int originalValue = Credits;
        PokerHands.Hands pHand = ObjectReferences.playerHand.handValue;
        if(pHand != PokerHands.Hands.NOTHING)
        {
            if(pHand == PokerHands.Hands.ROYALFLUSH && CurrentBet == 5)
            {
                Credits += 4000;
            }
            else
            {
                //Debug.Log(pHand + " * " + CurrentBet);
                Credits += PokerHands.handMultipliers[(int)pHand] * CurrentBet;
            }
        }
        EventManager.Animation.OnStartAnimatePayout?.Invoke(originalValue, Credits);
    }


    private void OnDealAnimComplete()
    {
        if (isGameStarting)
        {
            EventManager.Game.OnGameWaitingForHold?.Invoke();
        }
        if (isGameRedealing)
        {
            EventManager.Game.OnGameEnd?.Invoke();
        }
    }


    private void OnPayoutAnimComplete()
    {
        EventManager.Game.OnGameReadyToStart?.Invoke();
    }


    private void OnHoldButtonPressed(int index)
    {
        if(isGameWaitingForHold)
        {
            EventManager.Card.OnCardHeld?.Invoke(index);
        }
    }


    private void OnAddBetOneButtonPressed()
    {
        if(isGameReadyToStart)
        {
            if(shouldResetCurrentBet)
            {
                shouldResetCurrentBet = false;
                CurrentBet = 1;
            }
            else
            {
                if (CurrentBet < 5)
                {
                    CurrentBet += 1;
                }
                if (CurrentBet >= 5)
                {
                    StartGame();
                }
            }
        }
    }


    private void OnAddBetMaxButtonPressed()
    {
        if(isGameReadyToStart)
        {
            CurrentBet = 5;
            StartGame();
        }
    }


    private void OnDealButtonPressed()
    {
        if(isGameReadyToStart && CurrentBet > 0)
        {
            StartGame();
        }
        else if(isGameWaitingForHold)
        {
            Redeal();
        }
    }


    private void StartGame()
    {
        isGameReadyToStart = false;
        isGameStarting = true;
        EventManager.Game.OnGameStart?.Invoke();
    }


    private void Redeal()
    {
        isGameWaitingForHold = false;
        isGameRedealing = true;
        EventManager.Game.OnRedeal?.Invoke();
    }
}
