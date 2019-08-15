using UnityEngine;

/// <summary>
/// DebugObject component should be attached to a GameObject in scene.
/// Set DEBUG_MODE to true and populate debugCardRanks and debugCardSuits to replace the first X cards of Deck.cs's cards.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
/// </summary>
public class DebugObject : MonoBehaviour
{
    // Set to TRUE to disable shuffling and ability to manually set cards on startup
    // NOTE: Will allow duplicate cards within the deck. Don't use for more than one round.
    public bool DEBUG_MODE = true;
    public Card.Rank[] debugCardRanks;
    public Card.Suit[] debugCardSuits;
    
    private static DebugObject _instance;
    public static DebugObject Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
