using UnityEngine;

/// Manages button triggers for UI elements.
/// Should be attached to a GameObject, then GameObject should be attached to buttons to reference functions here.
/// Effectively a interface for input.
/// 
/// Video Poker for Unity
/// by Christopher Ueda, 2019
public class ButtonManager : MonoBehaviour
{
    // Function name starts from 1, logical index starts from 0
    public void OnCard1HoldPressed()
    {
        EventManager.Input.OnHoldButtonPressed(0);
    }
    public void OnCard2HoldPressed()
    {
        EventManager.Input.OnHoldButtonPressed(1);
    }
    public void OnCard3HoldPressed()
    {
        EventManager.Input.OnHoldButtonPressed(2);
    }
    public void OnCard4HoldPressed()
    {
        EventManager.Input.OnHoldButtonPressed(3);
    }
    public void OnCard5HoldPressed()
    {
        EventManager.Input.OnHoldButtonPressed(4);
    }

    public void OnBetOnePressed()
    {
        EventManager.Input.OnAddBetOneButtonPressed();
    }
    public void OnBetMaxPressed()
    {
        EventManager.Input.OnAddBetMaxButtonPressed();
    }
    public void OnDealPressed()
    {
        EventManager.Input.OnDealButtonPressed();
    }
}
