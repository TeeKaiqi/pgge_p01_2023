using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public CharacterController characterController;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>(); 

    }

    // Update is called once per frame
    void Update()
    {
        float speed = walkSpeed;

        //float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }

        //if (animator == null) return;
         
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;

        characterController.Move(movement * speed * Time.deltaTime);

        animator.SetFloat("PosX", horizontalInput);
        animator.SetFloat("PosZ", verticalInput);

    }

}
