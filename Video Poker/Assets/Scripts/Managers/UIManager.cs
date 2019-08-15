using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Image component of each UI element (put the five card displays in ascending order)
    [SerializeField]
    private Image[] cardDisplays = null;

    // Image component of each payout panel (put the five panels in ascending order)
    [SerializeField]
    private Image[] payoutDisplays = null;

    // Text objects showing HELD status (enable and disable object itself to show/hide)
    [SerializeField]
    private Text[] heldDisplays = null;

    // Text displays to be edited during gameplay
    [SerializeField]
    private Text handText = null;
    [SerializeField]
    private Text winText = null;
    [SerializeField]
    private Text betText = null;
    [SerializeField]
    private Text creditText = null;
    [SerializeField]
    private GameObject gameOverObject = null;

    // Sprites of cards in ascending order (put ace ~ king in inspector)
    [SerializeField]
    private Sprite[] clubSprites = null;
    [SerializeField]
    private Sprite[] diamondSprites = null;
    [SerializeField]
    private Sprite[] heartSprites = null;
    [SerializeField]
    private Sprite[] spadeSprites = null;
    // Additional sprite for card back
    [SerializeField]
    private Sprite cardBackSprite = null;

    // Color for highlighted and inactive payout table
    [SerializeField]
    private Color payoutHighlightColor = Color.red;
    [SerializeField]
    private Color payoutNormalColor = Color.blue;

    // Speed multiplier of animations, clamped for safety in Awake
    [SerializeField]
    private float animSpeedMultiplier = 2;

    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }


    void Awake()
    {
        // Singleton enforcement for GameObject
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        animSpeedMultiplier = Mathf.Clamp(animSpeedMultiplier, .1f, 10f);
    }


    private void OnEnable()
    {
        EventManager.Game.OnGameStart += OnGameStart;
        EventManager.Game.OnGameEnd += OnGameEnd;
        EventManager.Card.OnCardHeld += OnCardHeld;
        EventManager.Animation.OnStartAnimatePayout += OnStartAnimatePayout;
        EventManager.Credits.OnBetPlaced += OnBetPlaced;
        EventManager.Credits.OnCurrentBetValueChanged += OnCurrentBetValueChanged;
                
        HidePostgameElements();
    }


    private void OnDisable()
    {
        EventManager.Game.OnGameStart -= OnGameStart;
        EventManager.Game.OnGameEnd -= OnGameEnd;
        EventManager.Card.OnCardHeld -= OnCardHeld;
        EventManager.Animation.OnStartAnimatePayout -= OnStartAnimatePayout;
        EventManager.Credits.OnBetPlaced -= OnBetPlaced;
        EventManager.Credits.OnCurrentBetValueChanged -= OnCurrentBetValueChanged;
    }

    
    private void OnGameStart()
    {
        HidePostgameElements();
    }


    private void OnGameEnd()
    {
        gameOverObject.SetActive(true);
    }


    private void HidePostgameElements()
    {
        handText.enabled = false;
        winText.enabled = false;
        gameOverObject.SetActive(false);
        foreach (Text hd in heldDisplays)
        {
            hd.enabled = false;
        }
    }


    private void OnCardHeld(int index)
    {
        // Wait one frame to ensure visual updates after game logic
        StartCoroutine(UpdateHeldDisplayAtEndOfFrame(index));
    }


    private IEnumerator UpdateHeldDisplayAtEndOfFrame(int index)
    {
        yield return new WaitForEndOfFrame();
        heldDisplays[index].enabled = ObjectReferences.playerHand.IsCardHeld(index);
    }


    private void OnStartAnimatePayout(int oldVal, int newVal)
    {
        StartCoroutine(AnimatePayout(oldVal, newVal));
    }


    private IEnumerator AnimatePayout(int oldVal, int newVal)
    {
        int winCounter = 0;
        winText.text = "WIN " + winCounter;
        if (oldVal < newVal)
        {
            winText.enabled = true;
        }

        int curDisplay = oldVal;
        while (curDisplay < newVal)
        {
            yield return new WaitForSeconds(.25f / animSpeedMultiplier);

            curDisplay++;
            winCounter++;
            UpdateCreditText(curDisplay);
            winText.text = "WIN " + winCounter;
        }

        EventManager.Animation.OnPayoutAnimComplete?.Invoke();
    }


    private void OnBetPlaced(int oldVal, int newVal)
    {
        if(newVal <= oldVal)
        {
            UpdateCreditText(newVal);
        }
    }


    private void UpdateCreditText(int newVal)
    {
        if (newVal >= 0)
        {
            creditText.text = "Credits: " + newVal.ToString();
        }
        else
        {
            creditText.text = "GAMBLING ADDICTION";
        }
    }


    private void OnCurrentBetValueChanged(int oldVal, int newVal)
    {
        foreach (Image i in payoutDisplays)
        {
            i.color = payoutNormalColor;
        }
        if(newVal > 0)
        {
            payoutDisplays[newVal-1].color = payoutHighlightColor;
        }

        betText.text = "BET " + newVal;
    }


    // Will update card at position if flagged for update.
    // (Marking specific cards for update is important as it is used in redeal animation)
    public void UpdateCards(Card[] cards, bool[] willUpdatePosition)
    {
        StartCoroutine(AnimateCardDealing(cards, willUpdatePosition));        
    }


    private IEnumerator AnimateCardDealing(Card[] cards, bool[] willUpdatePosition)
    {
        // Set cards to update to back-faced image immediately
        for(int i=0; i<willUpdatePosition.Length; i++)
        {
            if(willUpdatePosition[i])
            {
                cardDisplays[i].sprite = cardBackSprite;
            }
        }

        // Update cards one at a time, delaying each time relative to speed manager
        for(int i=0; i<willUpdatePosition.Length; i++)
        {
            if(willUpdatePosition[i])
            {
                yield return new WaitForSeconds(.25f / animSpeedMultiplier);

                Card c = cards[i];
                int rank = (int)c.rank;
                switch (c.suit)
                {
                    case Card.Suit.CLUB:
                        cardDisplays[i].sprite = clubSprites[rank];
                        break;
                    case Card.Suit.DIAMOND:
                        cardDisplays[i].sprite = diamondSprites[rank];
                        break;
                    case Card.Suit.HEART:
                        cardDisplays[i].sprite = heartSprites[rank];
                        break;
                    case Card.Suit.SPADE:
                        cardDisplays[i].sprite = spadeSprites[rank];
                        break;
                    default:
                        Debug.LogError("Card does not fit any of the four default suits.");
                        break;
                }
            }
        }
        EventManager.Animation.OnDealAnimComplete?.Invoke();
    }


    public void UpdateHandText(PokerHands.Hands handValue)
    {
        //Debug.Log("Hand value is " + handValue);
        switch (handValue)
        {
            case PokerHands.Hands.ROYALFLUSH:
                handText.text = "ROYAL FLUSH";
                handText.enabled = true;
                break;
            case PokerHands.Hands.STRAIGHTFLUSH:
                handText.text = "STRAIGHT FLUSH";
                handText.enabled = true;
                break;
            case PokerHands.Hands.FOUROFAKIND:
                handText.text = "FOUR OF A KIND";
                handText.enabled = true;
                break;
            case PokerHands.Hands.FULLHOUSE:
                handText.text = "FULL HOUSE";
                handText.enabled = true;
                break;
            case PokerHands.Hands.FLUSH:
                handText.text = "FLUSH";
                handText.enabled = true;
                break;
            case PokerHands.Hands.STRAIGHT:
                handText.text = "STRAIGHT";
                handText.enabled = true;
                break;
            case PokerHands.Hands.THREEOFAKIND:
                handText.text = "THREE OF A KIND";
                handText.enabled = true;
                break;
            case PokerHands.Hands.TWOPAIR:
                handText.text = "TWO PAIR";
                handText.enabled = true;
                break;
            case PokerHands.Hands.JACKSORBETTER:
                handText.text = "JACKS OR BETTER";
                handText.enabled = true;
                break;
            default:
                handText.text = "";
                handText.enabled = false;
                break;
        }
    }
}
