using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public PlayerMovement playerController;
    private InputAction moveAction, lookAction;
    Vector2 moveVector;
    Vector2 lookVector;
    bool canRotate; //might not need
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canRotate = true;
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
        playerController.Move(moveVector);

        lookVector = lookAction.ReadValue<Vector2>();
        playerController.Rotate(lookVector);

        if(Input.GetKeyDown(KeyCode.P))
        {
            gameObject.transform.position = new Vector3( gameObject.transform.position.x + 10f,   gameObject.transform.position.y + 50f,  gameObject.transform.position.z);
            print("efff");
        }

    }

    //actual might not need to do this.
    void HandleLookRotation()
    {
        lookVector = lookAction.ReadValue<Vector2>();
        playerController.Rotate(lookVector);
    }
}
