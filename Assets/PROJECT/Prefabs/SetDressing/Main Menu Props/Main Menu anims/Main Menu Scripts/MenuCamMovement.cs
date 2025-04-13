using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamMovement : MonoBehaviour
{
    [SerializeField] float sensitivity = 0.1f;
    [SerializeField] float xClamp;
    [SerializeField] float yClamp;

    private Vector3 initialPosition;
    [SerializeField]float mouseX;
    [SerializeField]float mouseY;

    private void Start()
    {
        initialPosition = transform.position;
    }


    void Update()
    {

        mouseX += Input.GetAxis("Mouse X");
        mouseY += Input.GetAxis("Mouse Y");

        mouseX = Mathf.Clamp(mouseX, -xClamp, xClamp);
        mouseY = Mathf.Clamp(mouseY, -yClamp, yClamp);

        Vector3 newPosition = initialPosition + new Vector3(mouseX * sensitivity, mouseY * sensitivity, 0);

        transform.position = newPosition;

    }
}