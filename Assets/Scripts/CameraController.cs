using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private float minXRotation;
    [SerializeField] private float maxXRotation;

    [SerializeField] private float currentXRotation;

    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotateSpeed;

    private float x;
    private float y;

    private float moveX;

    private float moveZ;

    private float curZoom;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;
        currentXRotation = -50;
    }

    void Update()
    {
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.transform.localPosition = Vector3.up * curZoom;

        if(Input.GetMouseButton(1))
        {
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");

            currentXRotation += -y * rotateSpeed;
            currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);

            transform.eulerAngles = new Vector3(currentXRotation, transform.eulerAngles.y + (x * rotateSpeed), 0.0f);
        }

        Vector3 forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Vector3 right = cam.transform.right.normalized;

        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");

        Vector3 dir = forward * moveZ + right * moveX;
        dir.Normalize();

        dir *= moveSpeed * Time.deltaTime;

        transform.position += dir;
    }
}