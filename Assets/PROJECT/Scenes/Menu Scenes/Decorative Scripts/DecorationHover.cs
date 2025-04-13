using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHover : MonoBehaviour
{
    public float rotationSpeed = 30.0f; 
    public float hoverSpeed = 2.0f;     
    public float hoverAmplitude = 0.5f; 
    public float colorChangeSpeed = 2.0f; 
    private Vector3 initialPosition;
    private Renderer objectRenderer;
    private Color startColor;
    private Color endColor;
    private float colorChangeTimer = 0.0f;

    void Start()
    {
       
        initialPosition = transform.position;

      
        objectRenderer = GetComponent<Renderer>();

       
        startColor = GetRandomColor();
        endColor = GetRandomColor();
    }

    void Update()
    {

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = initialPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;

        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
        colorChangeTimer += Time.deltaTime * colorChangeSpeed;

        objectRenderer.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(colorChangeTimer, 1));
        if (colorChangeTimer >= 1.0f)
        {
            colorChangeTimer = 0.0f;
            startColor = endColor;
            endColor = GetRandomColor();
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
