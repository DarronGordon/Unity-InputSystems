using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCtrlSendMsg : MonoBehaviour
{

    Rigidbody rb;
    public Vector2 mouseDeltaPos;
    public Vector2 moveDirection;
    public bool action;
    [SerializeField] float impulse;

    [Header("Test_Object")]
    [SerializeField] MeshRenderer TestObject_Mesh;

    [Header("Test_Object Params")]
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float pitchSpeed = 1f;
    [SerializeField] float moveSpeed = 1f;
    void Look(InputValue value)
    {
        mouseDeltaPos = value.Get<Vector2>();
    }

    void Move(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    void LeftClick(InputValue value)
    {
        action = value.isPressed;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x, 0f, moveDirection.y) * moveSpeed * Time.deltaTime;

        if (action)
        {
            TestObject_Mesh.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(.8f, 1f));
        }

        transform.localRotation = transform.localRotation * Quaternion.Euler(mouseDeltaPos.y * rotationSpeed * Time.deltaTime, mouseDeltaPos.x * rotationSpeed * Time.deltaTime, 0f);
    }
}
