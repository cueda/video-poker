using System.Collections;
using UnityEngine;

/// Represents a single hand consisting of five cards.
/// (HAND_SIZE could be modified but the application won't magically handle more cards)
/// Designed to be easily adapted to multi-hand poker.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class PlayerHand : MonoBehaviour
{
    public PokerHands.Hands handValue { get; private set; }
    private const int HAND_SIZE = 5;

    private Card[] currentHand;
    private bool[] isHeld;


    private void Awake()
    {
        // Set this component as globally referrable for this implementation
        ObjectReferences.playerHand = this;
    }

    private void OnEnable()
    {
        EventManager.Game.OnGameStart += OnGameStart;
        EventManager.Game.OnRedeal += OnRedeal;

        EventManager.Card.OnCardHeld += OnCardHeld;

        EventManager.Animation.OnDealAnimComplete += OnDealAnimComplete;

        currentHand = new Card[HAND_SIZE];
        isHeld = new bool[HAND_SIZE];        
    }


    private void OnDisable()
    {
        EventManager.Game.OnGameStart -= OnGameStart;
        EventManager.Game.OnRedeal -= OnRedeal;

        EventManager.Card.OnCardHeld -= OnCardHeld;

        EventManager.Animation.OnDealAnimComplete -= OnDealAnimComplete;
    }


    private void OnGameStart()
    {
        // Reset held state and deal 5 cards
        for(int i=0; i<HAND_SIZE; i++)
        {
            isHeld[i] = false;
            currentHand[i] = Deck.Instance.DealCard();
            //Debug.Log("Dealt " + currentHand[i]);
        }
        CalculateHandAndUpdateUI(true);
    }


    private void OnRedeal()
    {
        for(int i=0; i<HAND_SIZE; i++)
        {
            if(!isHeld[i])
            {
                currentHand[i] = Deck.Instance.DealCard();
                //Debug.Log("Dealt " + currentHand[i]);
            }
        }
        CalculateHandAndUpdateUI(false);
    }

    private void CalculateHandAndUpdateUI(bool isFirstDeal)
    {
        handValue = PokerHands.CalculateHandValue(currentHand);
        StartCoroutine(UpdateCardsAtEndOfFrame(isFirstDeal));        
    }


    private IEnumerator UpdateCardsAtEndOfFrame(bool isFirstDeal)
    {
        yield return new WaitForEndOfFrame();

        // If this is the first deal, have card UI update/animate all cards
        // If this is not first deal, only update cards that are unheld
        bool[] cardsToUpdate = new bool[HAND_SIZE];
        for(int i=0; i<cardsToUpdate.Length; i++)
        {
            cardsToUpdate[i] = isFirstDeal ? true : !isHeld[i];
        }

        UIManager.Instance.UpdateCards(currentHand, cardsToUpdate);

    }


    private void OnCardHeld(int index)
    {
        isHeld[index] = !isHeld[index];
        //Debug.Log((isHeld[index] ? "Holding" : "Unholding") + " card " + index);
    }


    private void OnDealAnimComplete()
    {
        UIManager.Instance.UpdateHandText(handValue);
    }


    public bool IsCardHeld(int index)
    {
        return isHeld[index];
    }
}
