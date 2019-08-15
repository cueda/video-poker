using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : IComparable
{
    public Rank rank { get; }
    public Suit suit { get; }

    public enum Suit
    {
        CLUB,
        DIAMOND,
        HEART,
        SPADE
    }


    public enum Rank
    {
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING
    }


    public Card(Suit s, Rank r)
    {
        rank = r;
        suit = s;
    }


    // Default sort sorts by suit, then rank
    // Is currently unused in project
    int IComparable.CompareTo(object obj)
    {
        Card other = (Card)obj;
        if(suit < other.suit)
        {
            return 1;
        }
        else if (suit > other.suit)
        {
            return -1;
        }
        else
        {
            if(rank < other.rank)
            {
                return 1;
            }
            else if (rank > other.rank)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }


    // Sorts by ascending rank, then suit
    private class SortAscendingRankHelper : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            Card c1 = (Card)x;
            Card c2 = (Card)y;
            if(c1.rank < c2.rank)
            {
                return -1;
            }
            else if (c1.rank > c2.rank)
            {
                return 1;
            }
            else
            {
                if(c1.suit < c2.suit)
                {
                    return -1;
                }
                else if (c1.suit > c2.suit)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }


    public static IComparer GetAscendingRankComparer()
    {
        return (IComparer)new SortAscendingRankHelper();
    }


    public override string ToString()
    {
        return rank + " of " + suit + "S";
    }
}
