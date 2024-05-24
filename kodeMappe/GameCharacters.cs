using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class GameCharacters : MonoBehaviour
{
    // variables
    public float CharacterMoveSpeed; // used for character move speed
    public bool CheckMovement { get; private set; } // used for as a check for character movement

    // references

    GameCharacterAnimator animator;



    public void Awake()
    {
        animator = GetComponent<GameCharacterAnimator>();
    }


    public IEnumerator CharacterMovementFunction(Vector2 movementVector, Action MovementFinished = null) // will use this move vector to calculate target position
    {


        // setting the parameters
        animator.moveX = Mathf.Clamp(movementVector.x, -1f, 1f); // moveX will be set to what it gets from the move vector
        animator.moveY = Mathf.Clamp(movementVector.y, -1f, 1f); // moveY will be set to what it gets from the move vector
        

        // calculate target location
        var targetLocation = transform.position; // current location + inputs
        targetLocation.x += movementVector.x;
        targetLocation.y += movementVector.y;


        // moving the character to the target location only if the tile is walke able

        if (!CheckTilesFunction(targetLocation))
            yield break; // break if its not walkable

        CheckMovement = true; // setting the animator to true before we start the walking animations

        while ((targetLocation - transform.position).sqrMagnitude > Mathf.Epsilon)
        // hvis det er en forskjellen mellom nåværende lokasjon og ønsket lokasjon , så skal funksjonen under kjøres
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, CharacterMoveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetLocation; // setter players nåværende lokasjon til target lokasjon


        CheckMovement = false; // setting it to false when character is done moving



        MovementFinished?.Invoke();

    }

    // function to update movement animation

    public void AnimationMovementUpdater()
    {
        animator.CharacterMovingCheck = CheckMovement;
    }


    private bool CheckTilesFunction(Vector3 targetTileLocation) // will be used to check if all the tiles are clear for movement and not just the target tile
    {

        var calcDiff = targetTileLocation - transform.position; // variable to calculate the difference between character position and target position
        var direction = calcDiff.normalized; // using this to get a direction and this will return a vector with same direction as calDiff, but by using normalized it will have a length off one

        // will use Physics2D to "draw" a box in the path to check for this

        // it needs start pos, size, angle (0), direction and distance. adding also + direction to not start the box from the character tile, so it wont "collide" from its starting tile
        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, calcDiff.magnitude - 1, LayersInGame.layerRef.Objects | LayersInGame.layerRef.InteractionLayer | LayersInGame.layerRef.PlayerLayer) == true)

            // if it returns true then there is something in the path
            return false;

        return true; // if not then we return true and the character can walk in the path



    }



    // function to check if you can walk on the tile
    private bool FunctionToCheckIfYouCanWalkOnTile(Vector3 targetLocation2)

    {
        //check if there is something that the player can collide with in the traget tile and it needs target location and radius and the layer

        Vector2 bottomLeft = targetLocation2 - new Vector3(0.15f, 0.15f);
        Vector2 topRight = targetLocation2 + new Vector3(0.15f, 0.15f); 

        if (Physics2D.OverlapArea(bottomLeft, topRight, LayersInGame.layerRef.Objects | LayersInGame.layerRef.InteractionLayer) != null) // sjekker om object og interaction layers ikke er 0
        {
            return false;
        }
        return true;

    }

    // function to make character look at the target location
    public void FunctionToLookAtTarget(Vector3 targetLocation3)
    {

        // need to check the different between X and Y off the character loc and target loc
        var xAxisdifference = Mathf.Floor(targetLocation3.x) - Mathf.Floor(transform.position.x); // want the difference in int instead of an decimal value
        var yAxisdifference = Mathf.Floor(targetLocation3.y) - Mathf.Floor(transform.position.y); // same for Y coordinates


        // checking if x or y is == 0

        if (xAxisdifference == 0 || yAxisdifference == 0) // the look function will only run if either X or Y is 0 since characters in this game cant look at diagonal
        {
            // move the characters based on their X and Y values
            animator.moveX = Mathf.Clamp(xAxisdifference, -1f, 1f);
            animator.moveY = Mathf.Clamp(yAxisdifference, -1f, 1f);

        }
        else
        {
            Debug.Log("The character cant look diagonaly");
        }


    }

    // making it accessable for other classes
    public GameCharacterAnimator Animator
    {
        get => animator;

    }


}
