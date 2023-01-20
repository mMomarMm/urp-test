using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurations")]
    public Camera cam;
    public float startingCenter;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public float lookXLimit = 45.0f;
    public float standRate;
    public float crouchRate;

    [Header("Speeds")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed;
    public float lookSpeed = 2.0f;

    [Header("Crouch Settings")]
    public float crouchBoost = 0.55f;
    //cro = crouch
    public float croHeight = 1.26f;
    //ori = original
    float oriHeight;

    CharacterController cC;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0, curSpeed, boost, currentHeight;
    bool crouching, canMove = true;

    void Awake()
    {
        cam = Camera.main;
        boost = 1;
        cC = GetComponent<CharacterController>();
        oriHeight = cC.height;
        startingCenter = cC.center.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canMove)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            // Press Shift to run
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { curSpeed = runningSpeed; }
            else curSpeed = walkingSpeed;


            if (Input.GetKeyDown(KeyCode.C))
            {
                boost = crouchBoost;
                crouching = true;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                boost = 1;
                crouching = false;
            }
            currentHeight = Mathf.Clamp(currentHeight, croHeight, oriHeight);
            cC.height = currentHeight;

            curSpeed *= boost;

            float curSpeedX = Input.GetAxis("Vertical");
            float curSpeedY = Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection = moveDirection.normalized * curSpeed;

            if (cC.isGrounded)
            {
                if (!canMove) moveDirection.y = movementDirectionY;
            }
            else
            {
                moveDirection.y = movementDirectionY;
                moveDirection.y -= gravity * Time.deltaTime;
            }
            Rotation();
        }
    }

    // Player and Camera rotation
    void Rotation()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void FixedUpdate()
    {
        if (crouching)
        {
            currentHeight -= crouchRate;
        }
        else
        {
            currentHeight += standRate;
            Vector3 c = cC.center;
            c.y = startingCenter;
            cC.center = c;

        }
        cC.Move(moveDirection * Time.deltaTime);
    }
}