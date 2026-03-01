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
        Player.Instance?.playerController.OnMove(context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;
        Player.Instance?.playerController.OnDash(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;
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
        if (GameManager.Instance.state != GameState.Gameplay)
            return;
        Player.Instance?.playerController.OnJump(context);
    }

    // ------------ Pausing ------------

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (GameManager.Instance.state == GameState.Gameplay)
                GameManager.Instance.PauseGame();
            else if (GameManager.Instance.state == GameState.MainMenu || GameManager.Instance.state == GameState.PauseMenu)
                MenuManager.Instance.Escape();
        }
    }

    // ------------ Inventory ------------

    public void Next(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.started)
            Inventory.Instance?.Next();
    }

    public void Previous(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.started)
            Inventory.Instance?.Previous();
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.started)
            Inventory.Instance?.hotbar.TryDropItem();
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.started)
            Inventory.Instance?.hotbar.TryUseItem();
    }

    public void SelectSlot(int index)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        Inventory.Instance?.hotbar.SelectSlot(index);
    }

    // ------------ Recipes ------------

    public void ShowRecipes(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state != GameState.Gameplay)
            return;

        if (context.started)
            Inventory.Instance?.recipeBook.TriggerVisualization();
    }
}