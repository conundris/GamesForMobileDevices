using UnityEngine;
using Debug = UnityEngine.Debug;

public class Touch : MonoBehaviour
{
    private float dist;
    private Transform toDrag;
    private GameObject SelectedObject = null;

    private bool dragging = false;
    private bool rotating = false;
    private bool pinching = false;
    private bool swiping = false;
    
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    public float zoomRate = 100.0f;
    public float panSpeed = 0.001f;
    public float zoomDampening = 1.0f;

    private Vector2 touchZeroPos;
    private Vector2 touchOnePos;

    private float initialDistance;
    private float newDistance;
    
    Vector2?[] oldTouchPositions = {
        null,
        null
    };
    Vector2 oldTouchVector;
    float oldTouchDistance;
    
    void Update() {
        Vector3 position;
        
        if (Input.touchCount == 0) {
            oldTouchPositions[0] = null;
            oldTouchPositions[1] = null;
        }
        
        if (Input.touchCount < 1) {
            dragging = false; 
            return;
        }

        if (Input.touches[0].phase == TouchPhase.Began) {
            if (Input.touchCount == 1)
            {
                touchZeroPos = Input.touches[0].position;

                var ray = GetCamera(null).ScreenPointToRay(touchZeroPos);

                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.transform.gameObject.CompareTag("Selectable"))
                    {
                        SelectedObject = hit.transform.gameObject;

                        SelectedObject.GetComponent<Selectable>().ChangeColor(true);

                        dist = hit.transform.position.z - GetCamera(null).transform.position.z;

                        dragging = true;
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                touchOnePos = Input.touches[1].position;
                initialDistance = (touchZeroPos - touchOnePos).sqrMagnitude;
            }
        } else if (Input.touchCount == 2 && Input.touches[1].phase == TouchPhase.Began && Input.touches[0].phase == TouchPhase.Stationary)
        {
            touchOnePos = Input.touches[1].position;
            initialDistance = (touchZeroPos - touchOnePos).sqrMagnitude;
        } else if (Input.touches[0].phase == TouchPhase.Moved) {
            if (Input.touchCount == 1)
            {
                if (SelectedObject != null && dragging)
                {
                    position = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, dist);
                    position = GetCamera(null).ScreenToWorldPoint(position);
                    SelectedObject.gameObject.GetComponent<Movable>().Move(position);
                }
                else
                {
                    Vector2 touchDeltaPosition = Input.touches[0].deltaPosition;
                    GetCamera(null).transform.Translate(-touchDeltaPosition.x * 0.001f, -touchDeltaPosition.y * 0.001f, 0);
                }
            }
        }
        else if (Input.touchCount == 2 && Input.touches[1].phase == TouchPhase.Moved)
        {
            var touchOne = Input.touches[1];
            var touchZero = Input.touches[0];
                
            if (SelectedObject != null)
            {
                Rotate(touchZero, touchOne);
                Scale();
            }
            else
            {
                  /*Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;
            
                GetCamera(null).fieldOfView += difference * 0.05f;*/

                /*Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                swiping = true;
                GetCamera(null).transform.Rotate(touchDeltaPosition.y * 0.09f, touchDeltaPosition.x * 0.09f, 0);*/

                Zoom(touchZero, touchOne);
                Rotate(touchZero, touchOne);
            }
        }
        else if (Input.touches[0].phase == TouchPhase.Ended)
        {
            if (SelectedObject != null)
            {
                SelectedObject.GetComponent<Selectable>().ChangeColor(false);
                SelectedObject = null;
            }

            //initialDistance = 0;
            
        }
        /*else if (Input.touches[1].phase == TouchPhase.Ended)
        {
             
        }*/
    }

    private void Scale()
    {
        /*Vector2 touch1 = Input.GetTouch(0).position;
        Vector2 touch2 = Input.GetTouch(1).position;
        
        newDistance = (touch1 - touch2).sqrMagnitude;
        Debug.LogWarning("newDistance: " + newDistance);
        float changeInDistance = newDistance - initialDistance;
        Debug.LogWarning("changeInDistance: " + changeInDistance);
        float percentageChange = changeInDistance / initialDistance;
        Debug.LogWarning("percentageChange: " + percentageChange);
        
        
        var localScale = SelectedObject.transform.localScale;
        
        Vector3 newScale = localScale;
        Debug.LogWarning("Before newScale: " + newScale);
        newScale += percentageChange * localScale;
        Debug.LogWarning("After newScale: " + newScale);*/
        
        //SelectedObject.GetComponent<Scalable>().Scale(newScale);
        
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

        Vector3 newScale = SelectedObject.transform.localScale - new Vector3(deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
        SelectedObject.transform.localScale = newScale;
        
        
    }
    
    private void Rotate(UnityEngine.Touch touchZero, UnityEngine.Touch touchOne)
    {
        //transform.Rotate(0, 0, (touchZero.deltaPosition.y - touchZero.deltaPosition.x) * 0.05f, Space.World);
        //transform.Rotate(0, touchZero.deltaPosition.y * 0.05f, - touchZero.deltaPosition.x * 0.05f, 0, Space.World);
        
            oldTouchPositions[0] = Input.GetTouch(0).deltaPosition;
            oldTouchPositions[1] = Input.GetTouch(1).deltaPosition;
            oldTouchVector = (Vector2) (oldTouchPositions[0] - oldTouchPositions[1]);
            oldTouchDistance = oldTouchVector.magnitude;
        
        
            Vector2 newTouchVector = (touchZero.position - touchOne.position) * 0.005f;
            float newTouchDistance = newTouchVector.magnitude;

        if (SelectedObject != null)
        {
            SelectedObject.transform.localRotation *= Quaternion.Euler(new Vector3(0, 0,
                Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 1.54532924f));            
        }
        else
        {
            GetCamera(null).transform.localRotation *= Quaternion.Euler(new Vector3(0, 0,
                Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 1.54532924f));            
        }

    }

    private void Zoom(UnityEngine.Touch touchZero, UnityEngine.Touch touchOne)
    {
        Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
        float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;
        float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

        desiredDistance = deltaMagDiff * Time.deltaTime * zoomRate * 0.02f;

        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        position = transform.position - (Vector3.forward * currentDistance);

        //TODO: Change to specifically change main or selected Camera
        transform.position = position;
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

    public void HandleFirstTouch()
    {
        
    }

    public void HandleSecondTouch()
    {
        
    }

    public void HandleThirdTouch()
    {
        
    }
}
