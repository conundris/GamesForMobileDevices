using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TouchInput : MonoBehaviour
{
	public Toggle gyroToggle;
	private Gyroscope gyro;
	
    private float dist;
    private GameObject selectedObject = null;

    private bool dragging = false;
    
    const float zoomRate = 50.0f;
    
    const float pinchTurnRatio = Mathf.PI / 2;
    const float minTurnAngle = 0.5f;
 
    const float pinchRatio = 1;
    const float minPinchDistance = 1.5f;
    
    public float panSpeed = 0.001f;
    
    Vector2?[] oldTouchPositions = {
        null,
        null
    };
    Vector2 oldTouchVector;
    float oldTouchDistance;
    

    
    private float pt;

    void Start()
    {
	    gyro = Input.gyro;

	    gyro.enabled = false;
	    
	    //https://docs.unity3d.com/ScriptReference/UI.Toggle-onValueChanged.html
	    gyroToggle.onValueChanged.AddListener(delegate { gyroToggleChanged(gyroToggle.isOn); });
    }

    void gyroToggleChanged(bool isOn)
    {
	    gyro.enabled = isOn;

	    if (!gyro.enabled)
	    {
		    Camera.main.transform.rotation = new Quaternion();
	    }

    }
    
    void Update() {
        Vector3 position;

        if (gyro.enabled)
        {
	        Camera.main.transform.rotation  = gyro.attitude;
        }
        else
        {
			if (Input.touchCount == 0) {
				oldTouchPositions[0] = null;
				oldTouchPositions[1] = null;
			}
			
			if (Input.touchCount < 1) {
				dragging = false; 
				return;
			}
	
			if (Input.touches[0].phase == TouchPhase.Began)
			{
				if (Input.touchCount == 1)
				{
					var touchZeroPos = Input.touches[0].position;
	
					var ray = Camera.main.ScreenPointToRay(touchZeroPos);
	
					if (Physics.Raycast(ray, out var hit))
					{
						if (hit.transform.gameObject.CompareTag("Selectable"))
						{
							selectedObject = hit.transform.gameObject;
	
							selectedObject.GetComponent<Selectable>().ChangeColor(true);
	
							dist = hit.transform.position.z - transform.position.z;
	
							dragging = true;
						} else if (hit.transform.gameObject.GetComponent<Accelerator>() != null)
						{
							var acceleratorObject = hit.transform.gameObject.GetComponent<Accelerator>();
							acceleratorObject.enabled = !acceleratorObject.enabled;
						}
					}
				}
			} else if (Input.touches[0].phase == TouchPhase.Moved) {
				if (Input.touchCount == 1)
				{
					if (selectedObject != null && dragging)
					{
						position = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, dist);
						position = Camera.main.ScreenToWorldPoint(position);
						selectedObject.gameObject.GetComponent<Transformable>().Move(position);
					}
					else
					{
						var touchDeltaPosition = Input.touches[0].deltaPosition;
						transform.Translate(-touchDeltaPosition.x * 0.001f, -touchDeltaPosition.y * 0.001f, 0);
					}
				}
			}
			else if (Input.touchCount == 4)
			{
				// https://answers.unity.com/questions/899037/applicationquit-not-working-1.html
				#if UNITY_EDITOR
				// Application.Quit() does not work in the editor so
				// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
				UnityEditor.EditorApplication.isPlaying = false;
				#else
                         Application.Quit();
				#endif
			}
			else if (Input.touches[0].phase == TouchPhase.Ended)
			{
				if (selectedObject != null)
				{
					selectedObject.GetComponent<Selectable>().ChangeColor(false);
					selectedObject = null;
					dragging = false;
				}   
			}
        }
    }
    
    void LateUpdate() {
	    float pinchAmount = 0;
	    Quaternion desiredRotation = transform.rotation;

	    if (!gyro.enabled)
	    {
		    Calculate();
	    
		    if (Input.touchCount == 2)
		    {
			    if (Mathf.Abs(pinchDistanceDelta) > 0) { // zoom
				    pinchAmount = pinchDistanceDelta;
				    Debug.Log("Is Pinch. -> " + pinchDistanceDelta);
				    Scale();
			    }
 
			    if (Mathf.Abs(turnAngleDelta) > 0) { // rotate
				    Rotate(Input.GetTouch(0), Input.GetTouch(1));
			    }
		    }
	    }
    }

    // https://github.com/n1nj4z33/unity/blob/master/Assets/Scripts/Camera/TouchCamera.cs
    private void Rotate(Touch touchZero, Touch touchOne)
    {
        oldTouchPositions[0] = Input.GetTouch(0).deltaPosition;
        oldTouchPositions[1] = Input.GetTouch(1).deltaPosition;
        oldTouchVector = (Vector2) (oldTouchPositions[0] - oldTouchPositions[1]);
        oldTouchDistance = oldTouchVector.magnitude;
    
    
        var newTouchVector = (touchZero.position - touchOne.position) * 0.005f;
        var newTouchDistance = newTouchVector.magnitude;

        if (selectedObject != null)
        {
            selectedObject.transform.localRotation *= Quaternion.Euler(new Vector3(0, 0,
                Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 1.54532924f));            
        }
        else
        {
            transform.localRotation *= Quaternion.Euler(new Vector3(0, 0,
                Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 1.54532924f));            
        }
    }

    private void Rotate()
    {
	    // Based from Whiteboard
	    var ct = Mathf.Atan2(
		    Input.touches[0].position.y - Input.touches[1].position.y, 
		    Input.touches[0].position.x - Input.touches[1].position.x);

	    if (selectedObject != null)
	    {
		    // Object rotation 
		    //SelectedObject.transform.Rotate(Vector3.forward, (ct - pt) * Mathf.Rad2Deg);
		    selectedObject.GetComponent<Transformable>().Rotate((ct - pt) * Mathf.Rad2Deg);
	    }
	    else
	    {
		    // Camera rotation
		    transform.Rotate(transform.forward, ct - pt);
	    }

	    pt = ct;
    }
    
    private void Scale()
    {       
	    // Store both touches.
	    var touchZero = Input.GetTouch(0);
	    var touchOne = Input.GetTouch(1);

	    // Find the position in the previous frame of each touch.
	    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
	    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

	    // Find the magnitude of the vector (the distance) between the touches in each frame.
	    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
	    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

	    // Find the difference in distances between each frame.
	    float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * 0.005f;

	    if (selectedObject != null)
	    {
		    Vector3 newScale = selectedObject.transform.localScale - new Vector3(deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
		    selectedObject.GetComponent<Transformable>().Scale(newScale);		    
	    }
	    else
	    {
		    var desiredDistance = deltaMagnitudeDiff * Time.deltaTime * zoomRate;
		    transform.position += Vector3.forward * desiredDistance;
	    }
    }

	/// <summary>
	///   The delta of the angle between two touch points
	/// </summary>
	static public float turnAngleDelta;
	/// <summary>
	///   The angle between two touch points
	/// </summary>
	static public float turnAngle;
 
	/// <summary>
	///   The delta of the distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistanceDelta;
	/// <summary>
	///   The distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistance;
 
	/// <summary>
	///   Calculates Pinch and Turn - This should be used inside LateUpdate
	///   http://wiki.unity3d.com/index.php/DetectTouchMovement
	/// </summary>
	static public void Calculate () {
		pinchDistance = pinchDistanceDelta = 0;
		turnAngle = turnAngleDelta = 0;
 
		// if two fingers are touching the screen at the same time ...
		if (Input.touchCount == 2) {
			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];
 
			// ... if at least one of them moved ...
			if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
				// ... check the delta distance between them ...
				pinchDistance = Vector2.Distance(touch1.position, touch2.position);
				float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
				                                      touch2.position - touch2.deltaPosition);
				pinchDistanceDelta = pinchDistance - prevDistance;
 
				// ... if it's greater than a minimum threshold, it's a pinch!
				if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
					pinchDistanceDelta *= pinchRatio;
				} else {
					pinchDistance = pinchDistanceDelta = 0;
				}
 
				// ... or check the delta angle between them ...
				turnAngle = Angle(touch1.position, touch2.position);
				float prevTurn = Angle(touch1.position - touch1.deltaPosition,
				                       touch2.position - touch2.deltaPosition);
				turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);
 
				// ... if it's greater than a minimum threshold, it's a turn!
				if (Mathf.Abs(turnAngleDelta) > minTurnAngle) {
					turnAngleDelta *= pinchTurnRatio;
				} else {
					turnAngle = turnAngleDelta = 0;
				}
			}
		}
	}
 
	// http://wiki.unity3d.com/index.php/DetectTouchMovement
	private static float Angle (Vector2 pos1, Vector2 pos2) {
		var from = pos2 - pos1;
		var to = new Vector2(1, 0);
 
		var result = Vector2.Angle( from, to );
		var cross = Vector3.Cross( from, to );
 
		if (cross.z > 0) {
			result = 360f - result;
		}
 
		return result;
	}
    
    
}
