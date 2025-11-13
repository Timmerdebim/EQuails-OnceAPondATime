using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.InputSystem;

public class DuckPlayer : MonoBehaviour {
    public DuckController duck;
    public DuckCameraController cam;

    private void Start() {
#if !UNITY_EDITOR // Lock cursor for builds, not in editor
        Cursor.lockState = CursorLockMode.Locked;
#endif
        // Tell camera to follow transform
        cam.SetFollowTransform(duck.cameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        cam.IgnoredColliders.Clear();
        cam.IgnoredColliders.AddRange(duck.GetComponentsInChildren<Collider>());
    }

    private void Update() {
        //if (Input.GetMouseButtonDown(0)) {
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
    }

    private void LateUpdate() {
        // Handle rotating the camera along with physics movers
        if (cam.RotateWithPhysicsMover && duck.motor.AttachedRigidbody != null) {
            cam.PlanarDirection =
                duck.motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation *
                cam.PlanarDirection;
            cam.PlanarDirection = Vector3
                .ProjectOnPlane(cam.PlanarDirection, duck.motor.CharacterUp).normalized;
        }
        cam.Update();

    }

    // inputs
    public void OnMove(InputAction.CallbackContext context) {
        // read the value for the "move" action each event call
        Vector2 moveAmount = context.ReadValue<Vector2>();
        // print
        duck.moveInputVector = Vector2.ClampMagnitude(moveAmount, 1);
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.action.WasPressedThisFrame()) {
            duck.timeSinceDashInput = 0;
            duck.animator.SetTrigger("dash");
        }
    }
    
    public void OnAttack1(InputAction.CallbackContext context)
    {
        duck.attackType = DuckController.AttackType.light;

        if (context.action.WasPressedThisFrame()) {
            duck.animator.SetInteger("attackType", 1);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        duck.interactInput = context.action.IsPressed();
    }
    
    public void OnFlutter(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame()) {
            duck.animator.SetBool("flutterPressed", true);
        }
        if (context.action.WasReleasedThisFrame()) {
            duck.animator.SetBool("flutterPressed", false);
        }
    }
}