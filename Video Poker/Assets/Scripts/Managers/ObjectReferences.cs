using UnityEngine;

/// Utility class for storing global references cleanly.
/// Wasn't ultimately used for much but may be convenient in the future.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class ObjectReferences : MonoBehaviour
{
    // Could change to array to handle larger simultaneous plays
    public static PlayerHand playerHand = null;
}
