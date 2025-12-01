using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;

public class DuckPlayer : MonoBehaviour
{
    public DuckController duck;
    public DuckCameraController cam;

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        Vector2 moveAmount = context.ReadValue<Vector2>();
        // print
        duck.moveInputVector = Vector2.ClampMagnitude(moveAmount, 1);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            duck.timeSinceDashInput = 0;
            duck.animator.SetTrigger("dash");
        }
    }

    public void OnAttack1(InputAction.CallbackContext context)
    {
        duck.attackType = DuckController.AttackType.light;

        if (context.action.WasPressedThisFrame())
        {
            duck.animator.SetInteger("attackType", 1);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        duck.interactInput = context.action.IsPressed();
    }

    public void OnFlutter(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            duck.animator.SetBool("flutterPressed", true);
        }
        if (context.action.WasReleasedThisFrame())
        {
            duck.animator.SetBool("flutterPressed", false);
        }
    }
}