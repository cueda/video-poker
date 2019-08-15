using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
