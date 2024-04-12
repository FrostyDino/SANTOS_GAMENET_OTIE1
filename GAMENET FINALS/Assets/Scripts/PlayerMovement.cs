using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 40;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;

    public bool IsControlEnabled;

    private void Start()
    {
        IsControlEnabled = false;
    }

    void LateUpdate()
    {
        if (IsControlEnabled)
        {
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;

            transform.Rotate(0, rotation, 0);
        }
    }
}
