using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    private Camera cam;

    public float resetTimer;
    [SerializeField] private float resTime;

    [Header("Variaveis")]
    public float normalSpeed;
    public float fastSpeed;
    private float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public float rotationAmountMouse;
    public Vector3 zoomAmount;
    private Quaternion startRotation;
    private Vector3 startPosition;
    [Range(0,3)]
    [SerializeField] private float colorChangeMultiplier;

    [Header("Limites de camera")]
    public float minZoomY;
    public float maxZoomY;
    public float minZoomZ;
    public float maxZoomZ;
    public float maxPos;
    public float hue;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    void Start()
    {
        cam = cameraTransform.GetComponent<Camera>();

        resTime = 0;

        startRotation = transform.rotation;
        startPosition = transform.position;


        minZoomZ = -GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 3f;
        maxZoomY = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 3f;
        maxPos = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 3.5f;

        if (maxZoomY < minZoomY)
        {
            maxZoomY = minZoomY;
        }
        if (maxZoomZ < minZoomZ)
        {
            maxZoomZ = minZoomZ;
        }

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        
    }

    private void Update()
    {
        float H, S, V;
         
        Color.RGBToHSV(cam.backgroundColor, out H, out S, out V);
        //Debug.Log(H /*+ " " + S + " " + V*/);



        if (hue >= 1)
        {
            hue = 0;
        }
        else
        {
            hue += (Time.deltaTime / 100) * colorChangeMultiplier;
        }

        cam.backgroundColor = Color.HSVToRGB(hue,S,V);
    }

    private void LateUpdate()
    {
        resTime -= Time.deltaTime / Time.timeScale;
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        //mover camera
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else movementSpeed = normalSpeed;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }


        //rodar camera
        if (Camera.main.orthographic)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                newRotation *= Quaternion.Euler(Vector3.up * 45f);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                newRotation *= Quaternion.Euler(Vector3.up * -45f);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Q))
            {
                newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
            }
            if (Input.GetKey(KeyCode.E))
            {
                newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
            }
        }


        //reset camera rotation
        if (Input.GetKeyDown(KeyCode.F))
        {
            newRotation = startRotation;
            newPosition = startPosition;
        }

        newPosition.x = Mathf.Clamp(newPosition.x,-maxPos,maxPos);
        newPosition.z = Mathf.Clamp(newPosition.z,-maxPos,maxPos);

        transform.position = Vector3.Lerp(transform.position, newPosition, (Time.deltaTime * movementTime)/Time.timeScale);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, (Time.deltaTime * movementTime) / Time.timeScale);
    }

    void HandleMouseInput()
    {
        //Zoom camera
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;

            if (Camera.main.orthographic)
            {
                Camera.main.orthographicSize = newZoom.y;
            }          
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            newZoom -= zoomAmount;
        }else if (Input.GetKeyDown(KeyCode.C))
        {
            newZoom += zoomAmount;
        }
        //Dar drag da camera
        if (Input.GetMouseButtonDown(2))
        {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    dragCurrentPosition = ray.GetPoint(entry);

                    newPosition = transform.position + (dragStartPosition - dragCurrentPosition) * 2;
                }
                else
                {
                    dragCurrentPosition = ray.GetPoint(entry);

                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
                
            }
        }
        
        //rodar a camera
        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Input.mousePosition;

            if (resTime <= 0)
            {
                resTime = resetTimer;
            }
            else //Reset da camera
            {
                newRotation = startRotation;
                newPosition = startPosition;
            }
        }

        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / rotationAmountMouse));
        }

        Camera.main.orthographicSize = Mathf.Clamp(newZoom.y,minZoomY,maxZoomY);
        newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoomY);
        newZoom.z = Mathf.Clamp(newZoom.z, minZoomZ, maxZoomZ);
    }
}
