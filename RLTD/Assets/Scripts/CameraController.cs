using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

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

    [Header("Limites de camera")]
    public float minZoomY;
    public float maxZoomY;
    public float minZoomZ;
    public float maxZoomZ;
    public float maxPos;


    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    void Start()
    {
        resTime = 0;

        startRotation = transform.rotation;
        startPosition = transform.position;


        minZoomZ = -GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 10f;
        maxZoomY = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 10f;
        maxPos = GameObject.FindGameObjectWithTag("GridManager").GetComponent<WorldGeneration>().MaxWave * 12f;


        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        
    }

    private void LateUpdate()
    {
        resTime -= Time.deltaTime;
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
            //newRotation = startRotation;
            newPosition = startPosition;
        }

        newPosition.x = Mathf.Clamp(newPosition.x,-maxPos,maxPos);
        newPosition.z = Mathf.Clamp(newPosition.z,-maxPos,maxPos);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }


    //TO DO: CAMERA DAR SNAP A CADA 45 GRAUS USANDO O RATO
    void HandleMouseInput()
    {
        //Dar zoom - n funciona se camera estiver em orthographic mode
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;

            if (Camera.main.orthographic)
            {
                Camera.main.orthographicSize = newZoom.y;
            }
            
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
                //newRotation = startRotation;
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

        newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoomY);
        newZoom.z = Mathf.Clamp(newZoom.z, minZoomZ, maxZoomZ);
    }
}
