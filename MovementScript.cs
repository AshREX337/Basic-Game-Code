using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    [Header("Movement")]

    [SerializeField] private Transform orientation;

    private Vector3 movement, lastPos, lastMove;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed, mouseSens, groundDrag;
    private float rotY, rotX, tempMoveSpeed, camFov;
    [SerializeField] private Camera mainCamera;




    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode sprintKey;
    public KeyCode crouchKey;



    [Header("Jumping")]

    public float jumpForce;
    private float jumpForceMod = 3;
    public float jumpCooldown;
    public float airMultiplier;
    public float playerHeight;

    [SerializeField] private LayerMask ground;
    private bool grounded, readyToJump = true;



    private bool snap;
    private float prevX;



    [Header("Speed Changes")]

    public float crouchMultiplier;
    public float sprintMultiplier;
    public float sprintFovStrength;



    private void Start()
    {
        //Application.targetFrameRate = 800;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camFov = mainCamera.fieldOfView;
        tempMoveSpeed = moveSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        //Movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movement = orientation.right * x + orientation.forward * z;
        movement.Normalize();

        rotX = Input.GetAxis("Mouse X") * mouseSens;
        rotY -= Input.GetAxis("Mouse Y") * mouseSens;

        SpeedControl();


        //RotY
        rotY = Mathf.Clamp(rotY, -80, 80);
        mainCamera.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

        //RotX
        transform.Rotate(0, rotX, 0);




        //Jumping
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (!grounded)
        {
            rb.linearDamping = 0;
        }
        else
        {
            rb.linearDamping = groundDrag;   
        }


        //Crouching
        if (Input.GetKeyDown(crouchKey))
        {
            moveSpeed *= crouchMultiplier;
            
        }
        else if(Input.GetKeyUp(crouchKey))
        {
            moveSpeed = tempMoveSpeed;
        }

        //Sprinting
        if (Input.GetKeyDown(sprintKey))
        {
            moveSpeed *= sprintMultiplier;
            jumpForce += jumpForceMod;
        }
        else if (Input.GetKeyUp(sprintKey))
        {
            moveSpeed = tempMoveSpeed;
            jumpForce -= jumpForceMod;
        }
        //Sprint FOV Shift
        if(Input.GetKey(sprintKey))
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camFov + sprintFovStrength, 0.05f);
        }
        else if(mainCamera.fieldOfView > camFov)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camFov, 0.05f);
        }
        else
        {
            mainCamera.fieldOfView = camFov;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void MovePlayer()
    {
        if (grounded)
        {
            rb.AddForce(movement * moveSpeed * 10, ForceMode.Force);
        }
        else
        {
            rb.AddForce(movement * moveSpeed * 10 * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limited = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limited.x, rb.linearVelocity.y, limited.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        readyToJump = true;
    }




}
