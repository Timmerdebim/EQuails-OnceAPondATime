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
        Player.Instance?.playerController.OnMove(context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Player.Instance?.playerController.OnDash(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Player.Instance?.playerController.OnAttack(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Player.Instance.interactInput = context.action.IsPressed();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Player.Instance?.playerController.OnJump(context);
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