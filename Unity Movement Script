using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{

    private Vector3 movement, lastMove;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed = 10, dashSpeed, dashTime, dashRate;
    private float lastDash;
    private Boolean dashing, colliding, placing;
    [SerializeField] private Animator playerAnim;

    [SerializeField] private GameObject dropController;
    private int stun  = 1;


    [SerializeField] private Slider healthSlider;

    private void Start()
    {
        healthSlider.maxValue = 200;
    }


    void Update()
    {
        healthSlider.value = GetComponent<Health>().getHealth();
        placing = dropController.GetComponent<PlaceThing>().isPlacing();
        if (!dashing && !placing && stun != -1)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
        }
        else if(placing)
        {
            movement = Vector3.zero;
        }
        else if(stun == -1)
        {
            moveSpeed = 0;  
        }
        
        Attack();
        playerAnim.SetFloat("Speed", Mathf.Abs(movement.x) + Mathf.Abs(movement.z));
        playerAnim.SetFloat("PlayerSpeed", moveSpeed);
        if(movement != Vector3.zero)
        {
            lastMove= movement;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) && Time.time > lastDash && movement != Vector3.zero)
        {
            lastDash = Time.time + dashRate;
            StartCoroutine(dashWait());
        }
        Look();
    
    }

    void Look()
    {
        if (movement != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, -135, 0));
            var skewed = matrix.MultiplyPoint3x4(movement);

            var rel = (transform.position + skewed) - transform.position;
            var rot = Quaternion.LookRotation(rel, Vector3.up);
            if(moveSpeed < 50 && colliding == false)
            {
                moveSpeed += 0.3f;
            }


            transform.rotation = rot;
        }
        else
        {
            playerAnim.SetBool("Shifting", false);
            moveSpeed = 5.0f;
        }
    }

    private IEnumerator dashWait()
    {
        playerAnim.SetBool("Shifting", true);
        moveSpeed *= dashSpeed;
        playerAnim.SetFloat("Speed", Mathf.Abs(lastMove.x) + Mathf.Abs(lastMove.z));
        dashing = true;
        yield return new WaitForSeconds(dashTime);
        moveSpeed /= dashSpeed;
        dashing = false;
        playerAnim.SetBool("Shifting", false);
    }

    private void Attack()
    {
        var tempSpeed = moveSpeed;
        if (Input.GetButton("Fire1"))
        {
            playerAnim.SetBool("Attack", true);
            playerAnim.SetFloat("PlayerSpeed", 5); 
            moveSpeed = 0;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            playerAnim.SetBool("Attack", false);
            playerAnim.SetFloat("PlayerSpeed", 5);
            moveSpeed = tempSpeed;
        }
    }

    public int getStun()
    {
        return stun;
    }

    public void setStun(int s)
    {
        stun = s;
        Debug.Log(stun);
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            rb.MovePosition(rb.position + (transform.forward * lastMove.normalized.magnitude *1.5f) * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + (transform.forward * movement.normalized.magnitude) * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (!(collision.gameObject.tag.Equals("Floor") || collision.gameObject.tag.Equals("Weapon")))
        {
            moveSpeed = 0;
            colliding = true;
           
        }
    }

    private void OnCollisionExit(Collision collision)
    {
       if(!(collision.gameObject.tag.Equals("Floor") || collision.gameObject.tag.Equals("Weapon")))
        {
            moveSpeed = 10.0f;
            colliding = false;
            
        }
    }
    public void setCollide(bool collide)
    {
        colliding = collide;
    }

}
