using System;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class PlayerMovement : MonoBehaviour
{


     private CharacterController characterController;
     public float moveSpeed = 10f, RotationSpeed = 5f;
     private float roatationY;
     private float gravity = 9.81f;
     private float verticalVelocity;
     private Vector3 move;

    public AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
       
    }
    public bool isMoving()
    {

        //return move.x != 0 || move.z != 0;
        return new Vector2(move.x, move.z).sqrMagnitude > 0.001f;
    }

    public void Move(Vector2 movementVector)
    {
        Debug.Log("Move called: " + movementVector);
        move = transform.forward * movementVector.y + transform.right * movementVector.x;
        move = move * moveSpeed * Time.deltaTime;
        move.y = VerticalForceCalculation();
        characterController.Move(move);

        if (audioManager != null)
        {
            audioManager.SetFootstepMoving(isMoving());
        }
    }
    public void Rotate(Vector2 rotateVector)
    {
        roatationY += rotateVector.x * RotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, roatationY, 0);
    }
    private float VerticalForceCalculation()
    {
        if(characterController.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        return verticalVelocity;
    }

}
