using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    [Header("Movement")]

    [SerializeField] private Transform orientation;

    private Vector3 movement;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed, mouseSens;
    private float OGSpeed, OGJump, speedySpeed, rotY, rotX;
    [SerializeField] private Camera mainCamera;

    private Vector3 OGSize;



    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode sprintKey;
    public KeyCode crouchKey;



    [Header("Jumping")]

    public float jumpForce;
    public float jumpCooldown;
    public float airMultipier;
    public float playerHeight;

    [SerializeField] private LayerMask ground, wall;
    private bool grounded, readyToJump = true;



    [Header("Sprinting")]
    public float sprintMultipier;
    private bool sprinting, stopSprint, run;



    [Header("Crouching")]
    public float crouchMultipier;
    public float slideDecay;
    private bool sliding, sprintJump, superSlide;

    [Header("Wallrunning")]
    public float wallSpeed;
    private bool leftWall, rightWall, wallRunning;





    private void Start()
    {
        Application.targetFrameRate = 800;
        OGSpeed = moveSpeed;
        OGSize = transform.localScale;
        OGJump = jumpForce;
        speedySpeed = moveSpeed * sprintMultipier;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }
    // Update is called once per frame
    void Update()
    {
        //Movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movement = orientation.right * x + orientation.forward * z;

        rotX = Input.GetAxis("Mouse X") * mouseSens;
        rotY -= Input.GetAxis("Mouse Y") * mouseSens;

        //RotY
        rotY = Mathf.Clamp(rotY, -80, 80);
        mainCamera.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

        //RotX
        transform.Rotate(0, rotX, 0);






        //Jumping
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        leftWall = Physics.Raycast(transform.position, -orientation.right, 4f, wall);
        rightWall = Physics.Raycast(transform.position, orientation.right, 4f, wall);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            if (sliding)
            {
                sprintJump = true;
            }
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (grounded)
        {
            if (rb.velocity.x > 1)
            {
                rb.velocity -= new Vector3(0.1f, 0, 0);
            }

            if (rb.velocity.z > 1)
            {
                rb.velocity -= new Vector3(0, 0, 0.1f);
            }
        }
        if (!grounded)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, OGSize, 0.02f);
        }
        if (sliding)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(OGSize.x, OGSize.y * 0.4f, OGSize.z), 0.02f);
        }




        //Sprinting
        if (Input.GetKey(sprintKey) && !sprinting && !sliding)
        {
            sprinting = true;
            Sprint();

        }

        if (sprintJump)
        {
            SprintJump();
            sprintJump = false;
        }

        if (Input.GetKeyUp(sprintKey))
        {
            stopSprint = true;
            sprinting = false;
        }

        if (stopSprint && grounded && !sliding)
        {
            stopSprint = false;
            ResetSpeed();
        }






        //Sliding - Crouching
        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            Slide();
            sliding = true;
        }
        if (!grounded && Input.GetKeyDown(crouchKey))
        {
            sliding = true;

        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = OGSize;
            ResetSpeed();
        }

        if (sliding && grounded)
        {
            DecaySlide();
            rb.velocity = new Vector3(rb.velocity.x * 0.95f, rb.velocity.y, rb.velocity.z * 0.95f);
            if (superSlide)
            {
                jumpForce = OGJump;
                moveSpeed *= 1.6f;
                jumpForce *= 1.1f;
                superSlide = false;
                //rb.AddForce(Vector3.down, ForceMode.Impulse);
            }
        }
        else if (sliding && !grounded)
        {
            moveSpeed = speedySpeed;
            superSlide = true;
        }








        //Wallrunning

        if (leftWall && !wallRunning)
        {
            rb.AddForce(-orientation.right * 20, ForceMode.Impulse);
            wallRunning = true;
        }
        if (rightWall && !wallRunning)
        {
            rb.AddForce(orientation.right * 20, ForceMode.Impulse);
            wallRunning = true;
        }

        if (wallRunning)
        {
            rb.AddForce(orientation.forward * wallSpeed * 0.05f, ForceMode.Impulse);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 50);
            if ((Input.GetKeyDown(jumpKey) && leftWall))
            {
                rb.velocity = Vector3.zero;
                wallRunning = false;
                rb.AddForce((orientation.right + transform.up * 0.7f + orientation.forward * 0.35f) * 175, ForceMode.Impulse);
            }
            else if (Input.GetKeyDown(jumpKey) && rightWall)
            {

                rb.velocity = Vector3.zero;
                wallRunning = false;
                rb.AddForce((-orientation.right + transform.up * 0.7f + orientation.forward * 0.35f) * 175, ForceMode.Impulse);
            }

        }

        if (!leftWall && !rightWall && wallRunning)
        {
            rb.velocity -= Vector3.one * 0.02f;
            wallRunning = false;
        }


    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else if (!grounded)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * airMultipier * Time.fixedDeltaTime);
        }
    }






    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Sprint()
    {
        moveSpeed *= sprintMultipier;
    }

    private void SprintJump()
    {
        rb.AddForce((orientation.forward + transform.up * 0.03f) * moveSpeed * 2f, ForceMode.Impulse);
        sprintJump = false;
    }

    private void Crouch()
    {
        moveSpeed *= crouchMultipier;
    }

    private void Slide()
    {
        moveSpeed *= 1.25f;
    }

    private void DecaySlide()
    {
        if (moveSpeed > 0)
        {
            moveSpeed -= slideDecay;
        }
        else
        {
            moveSpeed = 0;
            ResetSpeed();
            Crouch();
        }
    }

    private void ResetSpeed()
    {
        moveSpeed = OGSpeed;
        jumpForce = OGJump;
        sliding = false;
        sprinting = false;
        sprintJump = false;
    }


}