using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform sputnik = null;
    [SerializeField] private float initialSpeed = 10f;
    [SerializeField] private float increaseSpeed = 1.25f;

    [SerializeField] private bool isEnabled = true;
    [SerializeField] private bool allowMovement = true;
    [SerializeField] private bool allowRotation = false;
    [SerializeField] private bool allowRotationOnRightClick = true;
    [SerializeField] private bool cursorVisibleByDefault = true;

    [SerializeField] private bool allowMiddleClickPanning = true;
    [Range(1f, 360f)][SerializeField] private float middleClickPanningSpeed = 12f;

    [SerializeField] private KeyCode forwardButton = KeyCode.W;
    [SerializeField] private KeyCode backwardButton = KeyCode.S;
    [SerializeField] private KeyCode rightButton = KeyCode.D;
    [SerializeField] private KeyCode leftButton = KeyCode.A;
    [SerializeField] private KeyCode TopButton = KeyCode.E;
    [SerializeField] private KeyCode BotButton = KeyCode.Q;

    [SerializeField] private float cursorSensitivity = 0.025f;
    [SerializeField] private bool cursorToggleAllowed = false;
    [SerializeField] private KeyCode cursorToggleButton = KeyCode.Escape;
    private float currentSpeed = 0f;
    private bool moving = false;
    private bool togglePressed = false;
    private Vector3 currCameraPosition;

    [Range(-360, 360)][SerializeField] private float minAngle = 89f;
    [Range(-360, 360)][SerializeField] private float maxAngle = 271f;
    private bool sled = false;
    private Vector3 initalOffset;
    private void OnEnable()
    {
        if (cursorToggleAllowed && isEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (!isEnabled) return;
        if (allowMovement)
        {
            bool lastMoving = moving;
            Vector3 deltaPosition = Vector3.zero;

            if (moving)
                currentSpeed += increaseSpeed * Time.deltaTime;

            moving = false;

            CheckMove(forwardButton, ref deltaPosition, transform.forward, false, false);
            CheckMove(backwardButton, ref deltaPosition, -transform.forward, false, false);
            CheckMove(rightButton, ref deltaPosition, transform.right, false, false);
            CheckMove(leftButton, ref deltaPosition, -transform.right, false, false);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                CheckMove(forwardButton, ref deltaPosition, transform.forward * 2, true, false);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                CheckMove(backwardButton, ref deltaPosition, -transform.forward * 2, false, true);
            }

            if (moving)
            {
                if (moving != lastMoving)
                    currentSpeed = initialSpeed;

                transform.position += deltaPosition * currentSpeed * Time.deltaTime;
            }
            else currentSpeed = 0f; 
            if (Input.GetKey(TopButton))
            {
                transform.localPosition += transform.up * 10 * Time.deltaTime;
            }
            else if (Input.GetKey(BotButton))
            {
                transform.localPosition += -transform.up * 10 * Time.deltaTime;
            }
            if (Input.GetMouseButton(2))
                if (sled){
                    sled = false;}
                else
                {
                    sled = true;
                    initalOffset = transform.position - sputnik.position;
                }
        }
        if (sled)
        {
            transform.position = sputnik.position + initalOffset;
        }

        if (allowRotation)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x += -Input.GetAxis("Mouse Y") * 359f * cursorSensitivity;
            eulerAngles.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
            if (eulerAngles.x < minAngle || eulerAngles.x > maxAngle)
            {
                transform.eulerAngles = eulerAngles;
            }
        }

        if (allowRotationOnRightClick && Input.GetMouseButton(1))
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x += -Input.GetAxis("Mouse Y") * 359f * cursorSensitivity;
            eulerAngles.y += Input.GetAxis("Mouse X") * 359f * cursorSensitivity;
            if (eulerAngles.x < minAngle || eulerAngles.x > maxAngle)
            {
                transform.eulerAngles = eulerAngles;
            }
        }

        if (allowMiddleClickPanning && Input.GetMouseButtonDown(2))
        {
            currCameraPosition = gameObject.transform.position;
        }

        if (allowMiddleClickPanning && Input.GetMouseButton(2))
        {
            gameObject.transform.position = new Vector3(currCameraPosition.x + Input.GetAxis("Mouse X") * middleClickPanningSpeed * cursorSensitivity, currCameraPosition.y - Input.GetAxis("Mouse Y") * middleClickPanningSpeed * cursorSensitivity, currCameraPosition.z);
            currCameraPosition = gameObject.transform.position;
        }

        if (cursorToggleAllowed)
        {
            if (Input.GetKey(cursorToggleButton))
            {
                if (!togglePressed)
                {
                    togglePressed = true;
                    Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
                    Cursor.visible = !Cursor.visible;
                }
            }
            else togglePressed = false;
        }
        else
        {
            togglePressed = false;
            if (!cursorVisibleByDefault) Cursor.visible = false;
        }
    }

    private void CheckMove(KeyCode keyCode, ref Vector3 deltaPosition, Vector3 directionVector, bool forceForward, bool forceBackward)
    {
        if (Input.GetKey(keyCode) || forceBackward || forceForward)
        {
            moving = true;
            deltaPosition += directionVector;
        }
    }
}
