using System;
using System.Collections.Generic;
using System.Data;
using Project.Menus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInput = moveInput.normalized;
        Player.Instance.SetInputDirection(new Vector3(moveInput.x, 0f, moveInput.y));

        if (moveInput.magnitude > 0.1f)
        {
            Player.Instance.animator.SetBool("isMoving", true);
            Player.Instance.SetViewDirection(Player.Instance._inputDirection);
            //front/back facing sprites ~Lars
            if (!Mathf.Approximately(moveInput.y, 0f)) //this looks dumb, but otherwise it flips when just moving L/R
            {
                Player.Instance.animator.SetFloat("FacingDirection", Mathf.Clamp01(moveInput.y));
            }
            if (!Mathf.Approximately(Player.Instance._inputDirection.x, 0f)) //this looks dumb, but otherwise it flips when just moving down
            {
                Player.Instance.sprite.flipX = Player.Instance._inputDirection.x > 0f;
            }
        }
        else
            Player.Instance.animator.SetBool("isMoving", false);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
            Player.Instance.animator.SetBool("dash", true);
        if (context.action.WasReleasedThisFrame())
            Player.Instance.animator.SetBool("dash", false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame() && Player.Instance.playerData.canAttack)
            Player.Instance.animator.SetBool("attack", true);
        if (context.action.WasReleasedThisFrame())
            Player.Instance.animator.SetBool("attack", false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Player.Instance.interactInput = context.action.IsPressed();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
        {
            if (Player.Instance.playerData.canFlutter) Player.Instance.animator.SetBool("flutter", true);
            else if (Player.Instance.playerData.canHop) Player.Instance.animator.SetBool("hop", true);
        }
        if (context.action.WasReleasedThisFrame())
        {
            Player.Instance.animator.SetBool("hop", false);
            Player.Instance.animator.SetBool("flutter", false);
        }
    }

    // ------------ Pausing ------------

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            if (GameManager.Instance.state == GameState.Gameplay)
                GameManager.Instance.PauseGame();
            else if (GameManager.Instance.state == GameState.MainMenu || GameManager.Instance.state == GameState.PauseMenu)
                MenuManager.Instance.Escape();
        }
    }

    // ------------ Inventory ------------

    public void DropItem(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
            Inventory.Instance?.DropItem();
    }

    public void ConsumeItem(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.action.WasPressedThisFrame())
            Inventory.Instance?.ConsumeItem();
    }

    public void SelectSlot(int index)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Inventory.Instance?.SelectSlot(index);
    }
}