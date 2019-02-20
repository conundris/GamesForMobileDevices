using System.Collections;
using System.Collections.Generic;
using TouchSys.Finger;
using UnityEngine;

public class TouchEvents : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        // Hook into the events we need
        TouchSystem.OnFingerDown  += OnFingerDown;
        TouchSystem.OnFingerTouching   += OnFingerSet;
        TouchSystem.OnFingerUp    += OnFingerUp;
        TouchSystem.OnFingerTapped   += OnFingerTap;
        TouchSystem.OnFingerSwiped += OnFingerSwipe;
        TouchSystem.OnGesture     += OnGesture;
    }

    protected virtual void OnDisable()
    {
        // Unhook the events
        TouchSystem.OnFingerDown  += OnFingerDown;
        TouchSystem.OnFingerTouching   += OnFingerSet;
        TouchSystem.OnFingerUp    += OnFingerUp;
        TouchSystem.OnFingerTapped   += OnFingerTap;
        TouchSystem.OnFingerSwiped += OnFingerSwipe;
        TouchSystem.OnGesture     += OnGesture;
    }

    public void OnFingerDown(Finger finger)
    {
        Debug.Log("Finger " + finger.Index + " began touching the screen");
    }

    public void OnFingerSet(Finger finger)
    {
        Debug.Log("Finger " + finger.Index + " is still touching the screen");
    }

    public void OnFingerUp(Finger finger)
    {
        Debug.Log("Finger " + finger.Index + " finished touching the screen");
    }

    public void OnFingerTap(Finger finger)
    {
        Debug.Log("Finger " + finger.Index + " tapped the screen");
    }

    public void OnFingerSwipe(Finger finger)
    {
        Debug.Log("Finger " + finger.Index + " swiped the screen");
    }

    public void OnGesture(List<Finger> fingers)
    {
        Debug.Log("Gesture with " + fingers.Count + " finger(s)");
        /*Debug.Log("    pinch scale: " + LeanGesture.GetPinchScale(fingers));
        Debug.Log("    twist degrees: " + LeanGesture.GetTwistDegrees(fingers));
        Debug.Log("    twist radians: " + LeanGesture.GetTwistRadians(fingers));
        Debug.Log("    screen delta: " + LeanGesture.GetScreenDelta(fingers));*/
    }
}
