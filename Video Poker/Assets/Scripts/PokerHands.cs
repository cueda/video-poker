using System;

/// Handles winning hand representation and their calculations off of a five-or-less card hand.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class PokerHands
{
    public enum Hands
    {
        ROYALFLUSH,
        STRAIGHTFLUSH,
        FOUROFAKIND,
        FULLHOUSE,
        FLUSH,
        STRAIGHT,
        THREEOFAKIND,
        TWOPAIR,
        JACKSORBETTER,
        NOTHING
    }

    public static readonly int[] handMultipliers = new int[] {250, 50, 25, 9, 6, 4, 3, 2, 1, 0};


    public static Hands CalculateHandValue (Card[] hand)
    {
        // Make a copy of current hand to sort, then search for valid hands efficiently
        Card[] rankSorted = new Card[hand.Length];
        Array.Copy(hand, rankSorted, hand.Length);
        Array.Sort(rankSorted, Card.GetAscendingRankComparer());

        // Could divide logic to minimize check repetition, but repeats included for code clarity
        // (ex. straight flush check duplicates check for straight/flush)
        if (IsHandRoyalFlush(rankSorted)) return Hands.ROYALFLUSH;
        if (IsHandStraightFlush(rankSorted)) return Hands.STRAIGHTFLUSH;
        if (IsHandFourOfAKind(rankSorted)) return Hands.FOUROFAKIND;
        if (IsHandFullHouse(rankSorted)) return Hands.FULLHOUSE;
        if (IsHandFlush(rankSorted)) return Hands.FLUSH;
        if (IsHandStraight(rankSorted)) return Hands.STRAIGHT;
        if (IsHandThreeOfAKind(rankSorted)) return Hands.THREEOFAKIND;
        if (IsHandTwoPair(rankSorted)) return Hands.TWOPAIR;
        if (IsHandJacksOrBetter(rankSorted)) return Hands.JACKSORBETTER;
        return Hands.NOTHING;
    }


    #region Hand combination checking
    // All checks assume rank-sorted hands
    // (Could do rank-sorting per function, but causes a lot of repeated sorting work)

    // If hand is a straight flush, and the lowest ranks are ACE and TEN, it is a royal flush
    private static bool IsHandRoyalFlush(Card[] rankSortedHand)
    {
        if(IsHandStraightFlush(rankSortedHand))
        {
            if(rankSortedHand[0].rank == Card.Rank.ACE && rankSortedHand[1].rank == Card.Rank.TEN)
            {
                return true;
            }
        }
        return false;
    }


    // If all cards have the same suit as the first card, 
    // and each card is 1 higher than the previous, it is a straight flush
    // (Possibly a way to store results and reuse instead of recomputing flush/straight)
    private static bool IsHandStraightFlush(Card[] rankSortedHand)
    {
        return IsHandFlush(rankSortedHand) && IsHandStraight(rankSortedHand);
    }


    // If a given rank is found four times in successive cards, it is a four-of-a-kind
    private static bool IsHandFourOfAKind(Card[] rankSortedHand)
    {
        Card.Rank curRank = rankSortedHand[0].rank;
        int count = 0;

        for (int i = 0; i < rankSortedHand.Length; i++)
        {
            if (curRank == rankSortedHand[i].rank)
            {
                count++;
                if(count >= 4)
                {
                    return true;
                }
            }
            else
            {
                count = 1;
                curRank = rankSortedHand[i].rank;
            }
        }

        return false;
    }


    // Split hands into two groups - at card 2 and 3 if different, otherwise at card 3 and 4
    // If hands contain a three-of-a-kind in one subhand and a pair in the other, it is a full house
    private static bool IsHandFullHouse(Card[] rankSortedHand)
    {
        Card[] smallerGroup = new Card[2];
        Card[] largerGroup = new Card[3];
        if (rankSortedHand[1].rank != rankSortedHand[2].rank)
        {
            Array.Copy(rankSortedHand, 0, smallerGroup, 0, 2);
            Array.Copy(rankSortedHand, 2, largerGroup, 0, 3);
        }
        else
        {
            Array.Copy(rankSortedHand, 0, largerGroup, 0, 3);
            Array.Copy(rankSortedHand, 3, smallerGroup, 0, 2);
        }
        return IsHandThreeOfAKind(largerGroup) && DoesPairExist(smallerGroup);
    }

    // If all cards' suits match the first card, it is a flush
    private static bool IsHandFlush(Card[] rankSortedHand)
    {
        Card.Suit suit = rankSortedHand[0].suit;
        foreach(Card c in rankSortedHand)
        {
            if(c.suit != suit)
            {
                return false;
            }
        }
        return true;
    }

    
    // If all ranks are sequentially ascending by 1, hand is a straight
    // Includes a special case for TJQKA high-straight
    private static bool IsHandStraight(Card[] rankSortedHand)
    {
        // Special case for high-straight Ace wraparound
        // (if first card is Ace, and is followed by 10)
        bool mayBeHighStraight = false;
        if (rankSortedHand[0].rank == Card.Rank.ACE && rankSortedHand[1].rank == Card.Rank.TEN)
        {
            mayBeHighStraight = true;
        }

        for (int i=0; i<rankSortedHand.Length-1; i++)
        {
            if(mayBeHighStraight && i == 0)
            {
                // Do no check and continue from second card as Card.Rank.TEN
            }
            else if (rankSortedHand[i+1].rank != rankSortedHand[i].rank+1)
            {
                return false;
            }
        }
        return true;
    }


    // If a given rank is found three times in successive cards, it is a three-of-a-kind
    private static bool IsHandThreeOfAKind(Card[] rankSortedHand)
    {
        Card.Rank curRank = rankSortedHand[0].rank;
        int count = 0;

        for (int i = 0; i < rankSortedHand.Length; i++)
        {
            if (curRank == rankSortedHand[i].rank)
            {
                count++;
                if(count >= 3)
                {
                    return true;
                }
            }
            else
            {
                count = 1;
                curRank = rankSortedHand[i].rank;
            }
        }

        return false;
    }


    // Checks for a first sequential pair, 
    // then searches for a second pair with a separate rank from the first
    // If a second pair is found, hand contains a two-pair
    private static bool IsHandTwoPair(Card[] rankSortedHand)
    {
        Card.Rank firstPairRank = rankSortedHand[0].rank;
        Card.Rank secondPairRank = rankSortedHand[0].rank;
        bool firstPairFound = false;
        
        for(int i=1; i<rankSortedHand.Length; i++)
        {
            if(!firstPairFound)
            {
                if(firstPairRank == rankSortedHand[i].rank)
                {
                    firstPairFound = true;
                    if(i < rankSortedHand.Length-1)
                    {
                        secondPairRank = rankSortedHand[i + 1].rank;
                        i++;
                    }
                }
                else
                {
                    firstPairRank = rankSortedHand[i].rank;
                }
            }
            else
            {
                if(secondPairRank != firstPairRank)
                {
                    if(secondPairRank == rankSortedHand[i].rank)
                    {
                        return true;
                    }
                    else
                    {
                        secondPairRank = rankSortedHand[i].rank;
                    }
                }
            }
        }

        return false;
    }


    // Searches for a single sequential pair
    // Checks if that pair is jacks or higher, and makes a second check for aces (lowest)
    private static bool IsHandJacksOrBetter(Card[] rankSortedHand)
    {
        for (int i = 0; i < rankSortedHand.Length - 1; i++)
        {
            if (rankSortedHand[i].rank == rankSortedHand[i + 1].rank)
            {
                if(rankSortedHand[i].rank >= Card.Rank.JACK || rankSortedHand[i].rank == Card.Rank.ACE)
                {
                    return true;
                }
            }
        }
        return false;
    }


    // Not used for winning hands calculations itself, 
    // but purely a utility for checking for pairs in a Card[] subset
    private static bool DoesPairExist(Card[] rankSortedHand)
    {
        for(int i=0; i<rankSortedHand.Length-1; i++)
        {
            if(rankSortedHand[i].rank == rankSortedHand[i+1].rank)
            {
                return true;
            }
        }
        return false;
    }

    #endregion

}
