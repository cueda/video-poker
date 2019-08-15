using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
