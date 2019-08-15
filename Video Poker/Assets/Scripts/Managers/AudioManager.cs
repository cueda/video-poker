using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Plays sounds on certain events (payout increments and card dealing)
/// Triggers off of events fired when cards are drawn or credits are gained.
/// (There's probably a better system for this that uses existing events
///  rather than events made specifically for audio playback)
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource blip;
    [SerializeField]
    private AudioSource coin;


    private void OnEnable()
    {
        EventManager.Animation.OnOneCardDrawAnimated += OnOneCardDrawAnimated;
        EventManager.Animation.OnOneCreditPayoutAnimated += OnOneCreditPayoutAnimated;
    }


    private void OnDisable()
    {
        EventManager.Animation.OnOneCardDrawAnimated -= OnOneCardDrawAnimated;
        EventManager.Animation.OnOneCreditPayoutAnimated -= OnOneCreditPayoutAnimated;
    }


    private void OnOneCardDrawAnimated()
    {
        blip.Play();
    }


    private void OnOneCreditPayoutAnimated()
    {
        coin.Play();
    }
}
