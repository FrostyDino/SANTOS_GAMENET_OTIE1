using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;

    private MovementController movementController;
    public FixedTouchField fixedTouchField;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        movementController = this.GetComponent<MovementController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        movementController.joystickInputAxis.x = joystick.Horizontal;
        movementController.joystickInputAxis.y = joystick.Vertical;
        movementController.lookInputAxis = fixedTouchField.TouchDist;

        animator.SetFloat("horizontal", joystick.Horizontal);
        animator.SetFloat("vertical", joystick.Vertical);

        if (Mathf.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9)
        {
            animator.SetBool("IsRunning", true);
            movementController.speed = 10;
        }
        else 
        {
            animator.SetBool("IsRunning", false);
            movementController.speed = 5;
        }
    }
}
