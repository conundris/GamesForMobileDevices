using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TouchSys.Finger;
using UnityEngine;

public class TouchSystem : MonoBehaviour
{
    /// <summary>This contains all the active and enabled LeanTouch instances</summary>
    public static List<TouchSystem> Instances = new List<TouchSystem>();
    
    /// <summary>This list contains all currently active fingers (including simulated ones)</summary>
    public static List<Finger> Fingers = new List<Finger>(3);
    
    /// <summary>This gets fired when a finger begins touching the screen (LeanFinger = The current finger)</summary>
    public static System.Action<Finger> OnFingerDown;

    /// <summary>This gets fired every frame a finger is touching the screen (LeanFinger = The current finger)</summary>
    public static System.Action<Finger> OnFingerTouching;

    /// <summary>This gets fired when a finger stops touching the screen (LeanFinger = The current finger)</summary>
    public static System.Action<Finger> OnFingerUp;

    /// <summary>This gets fired when a finger taps the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time) (LeanFinger = The current finger)</summary>
    public static System.Action<Finger> OnFingerTapped;

    /// <summary>This gets fired when a finger swipes the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time, and also moves more than the 'SwipeThreshold' distance) (LeanFinger = The current finger)</summary>
    public static System.Action<Finger> OnFingerSwiped;

    /// <summary>This gets fired every frame at least one finger is touching the screen (List = Fingers).</summary>
    public static System.Action<List<Finger>> OnGesture;

    /// <summary>This gets fired after a finger has stopped touching the screen for more than TapThreshold seconds, and is removed from both the active and inactive finger lists.</summary>
    public static System.Action<Finger> OnFingerExpired;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>The first active and enabled LeanTouch instance.</summary>
    public static TouchSystem Instance
    {
        get
        {
            return Instances.Count > 0 ? Instances[0] : null;
        }
    }
    // Source: LeanTouch
    /// <summary>If you multiply this value with any other pixel delta (e.g. ScreenDelta), then it will become device resolution independant.</summary>
    public static float ScalingFactor
    {
        get
        {
            // Get the current screen DPI
            var dpi = Screen.dpi;

            // If it's 0 or less, it's invalid, so return the default scale of 1.0
            if (dpi <= 0)
            {
                return 1.0f;
            }

            // DPI seems valid, so scale it against the reference DPI
            return 200 / dpi;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Finger Loop
        // Poll --> Get current touch data and convert into Finger Instances
        PollFingers();
        // UpdateFingers --> Update current Finger Data, possible remove unused Fingers
        UpdateFingers();
        // Update --> Update Events
        UpdateEvents();
        
        
    }
    
    
    public static Camera GetCamera(Camera currentCamera, GameObject gameObject = null)
    {
        if (currentCamera == null)
        {
            if (gameObject != null)
            {
                currentCamera = gameObject.GetComponent<Camera>();
            }

            if (currentCamera == null)
            {
                currentCamera = Camera.main;
            }
        }

        return currentCamera;
    }

    private void PollFingers()
    {
        if (Input.touchCount > 0)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    AddFinger(touch.fingerId, touch.position);
                }
            }
        }
    }
    
    private void UpdateEvents()
    {
        var fingerCount = Fingers.Count;

        if (fingerCount > 0)
        {
            for (var i = 0; i < fingerCount; i++)
            {
                var finger = Fingers[i];
					
                if (finger.Tapped) OnFingerTapped?.Invoke(finger);
                if (finger.Swiped) OnFingerSwiped?.Invoke(finger);
                if (finger.Down) OnFingerDown?.Invoke(finger);
                if (finger.Touching) OnFingerTouching?.Invoke(finger);
                if (finger.Up) OnFingerUp?.Invoke(finger);
            }

            OnGesture?.Invoke(Fingers);
        }
    }

    private void UpdateFingers()
    {
        /*for (var i = Fingers.Count - 1; i >= 0; i--)
        {
            var finger = Fingers[i];

            // Up?
            if (finger.Up == true)
            {
                // Tap or Swipe?
                if (finger.Age <= TapThreshold)
                {
                    if (finger.SwipeScreenDelta.magnitude * ScalingFactor < SwipeThreshold)
                    {
                        finger.Tap       = true;
                        finger.TapCount += 1;
                    }
                    else
                    {
                        finger.TapCount = 0;
                        finger.Swipe    = true;
                    }
                }
                else
                {
                    finger.TapCount = 0;
                }
            }
            // Down?
            else if (finger.Down == false)
            {
                // Age it
                finger.Age += Time.unscaledDeltaTime;
            }
        }*/
    }
    
    private void AddFinger(int index, Vector2 screenPosition)
    {
        Fingers.Add(new Finger
        {
            Index = index,
            StartScreenPosition = screenPosition,
            LastScreenPosition = screenPosition
        });
    }
    
    protected virtual void OnEnable()
    {
        Instances.Add(this);
    }

    protected virtual void OnDisable()
    {
        Instances.Remove(this);
    }
}
