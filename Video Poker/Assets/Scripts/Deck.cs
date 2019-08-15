using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    // Singleton deck - refer to instance to use this object
    private static Deck _instance;
    public static Deck Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new Deck();
            }
            return _instance;
        }
    }

    private Card[] cards;
    private int currentIndex = 0;
        

    private Deck()
    {
        cards = new Card[52];

        // Add the 13 cards for each suit into the deck
        int index = 0;
        for(int i=0; i<4; i++)
        {
            Card.Suit suit = (Card.Suit)i;
            for(int j=0; j<13; j++)
            {
                Card.Rank rank = (Card.Rank)j;
                cards[index] = new Card(suit, rank);
                index++;
            }

        }

        if(DebugObject.Instance.DEBUG_MODE)
        {
            for(int i=0; i<Mathf.Min(DebugObject.Instance.debugCardRanks.Length, DebugObject.Instance.debugCardSuits.Length); i++)
            {
                cards[i] = new Card(DebugObject.Instance.debugCardSuits[i], DebugObject.Instance.debugCardRanks[i]);
                Debug.Log("Replacing card " + i + " with " + cards[i]);
            }
        }
        ShuffleDeck();
    }

    
    // Shuffles deck by resetting current index to 0, then randomizing card positions (Fisher-Yates shuffle)
    // Effectively a fresh 52-card deck
    // NOTE: Will not shuffle deck if in Debug Mode as specified by DebugObject
    public void ShuffleDeck()
    {
        currentIndex = 0;
        if (!DebugObject.Instance.DEBUG_MODE)
        {
            // Shuffle by swapping card positions repeatedly (include current index to avoid non-uniform Sattolo cycle)
            for (int i = cards.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                //Debug.Log("Swapping " + j + " and " + i);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }
    }


    public Card DealCard()
    {
        Card nextCard = cards[currentIndex];
        currentIndex++;
        return nextCard;
    }
}
