using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAnimator : MonoBehaviour
{
    //reference
    SpriteRenderer spriteRenderer;

    [SerializeField] List<Sprite> DownMovementImages;
    [SerializeField] List<Sprite> UpMovementImages;
    [SerializeField] List<Sprite> LeftMovementImages;
    [SerializeField] List<Sprite> RightMovementImages;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;


    SpriteAnimator characterCurrentAnimation; // variable to store the current animation
    bool previousAnimation;




    // parameters will be needed, will be using moveX, moveY and var to check if char is moving

    public float moveX { get; set; }
    public float moveY { get; set; }

    public bool CharacterMovingCheck { get; set; }



    // state for animations and using the SpriteAnimator class to make animations

    SpriteAnimator stateForDownMovementAnimation;
    SpriteAnimator stateForUpMovementAnimation;
    SpriteAnimator stateForLeftMovementAnimation;
    SpriteAnimator stateForRightMovementAnimation;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // initialization the different movement animations
        stateForDownMovementAnimation = new SpriteAnimator(DownMovementImages, spriteRenderer);
        stateForUpMovementAnimation = new SpriteAnimator(UpMovementImages, spriteRenderer);
        stateForLeftMovementAnimation = new SpriteAnimator(LeftMovementImages, spriteRenderer);
        stateForRightMovementAnimation = new SpriteAnimator(RightMovementImages, spriteRenderer);
        CharacterFacingFunction(defaultDirection); // setting the facing direction to the default direction of the character

        characterCurrentAnimation = stateForDownMovementAnimation; 

    }

    private void Update()
    {

        var previousCharacterAnimation = characterCurrentAnimation;


        // deciding which animation to show based on the values of moveX and moveY

        if (moveX == 1) // walking right
            characterCurrentAnimation = stateForRightMovementAnimation;
        else if (moveX == -1) // walking left
            characterCurrentAnimation = stateForLeftMovementAnimation;
        else if (moveY == 1) // walk up
            characterCurrentAnimation = stateForUpMovementAnimation;
        else if (moveY == -1) // walk down
            characterCurrentAnimation = stateForDownMovementAnimation;

        // checking to see if the current animation is not the same as the previous animation

        if (characterCurrentAnimation != previousCharacterAnimation || CharacterMovingCheck != this.previousAnimation) // implemnting it this way to prevent sliding when changinge from animations
            characterCurrentAnimation.Start(); // then we call the start function


        // we only want to play the animations if char moving var returns true

        if (CharacterMovingCheck)
            characterCurrentAnimation.spriteAnimationUpdater();
        else
            spriteRenderer.sprite = characterCurrentAnimation.Frames[0]; // setting the sprite renderer to the first frame

        this.previousAnimation = CharacterMovingCheck; // setting previousanimation to characterMovingCheck to avoid sliding
    }


    public void CharacterFacingFunction(FacingDirection charFacingDirection) // with the facing direction as parameter
    {
        if (charFacingDirection == FacingDirection.Left)
            moveX = -1;
        else if (charFacingDirection == FacingDirection.Right)
            moveX = 1;
        else if (charFacingDirection == FacingDirection.Up)
            moveY = -1;
        else if (charFacingDirection == FacingDirection.Down)
            moveY = 1;




    }


    // to get access to FacingDirection
    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }


    public enum FacingDirection { Up, Down, Left, Right }


}
