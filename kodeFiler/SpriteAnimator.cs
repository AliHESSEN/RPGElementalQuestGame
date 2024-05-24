using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{

    //refrences

    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRateIntervals; // intervals used to change the sprites (60 fps)


    //variables

    int fps; // to keep track of fps
    float variableForTimer; // used to track time



    // constructor to initialization it

    public SpriteAnimator(List<Sprite> framesVar, SpriteRenderer spriteRenderer, float frameRate = 0.16f) // 0.16f = 60fps

    {

        // initialization of the variable with the given values
        this.spriteRenderer = spriteRenderer;
        this.frames = framesVar;
        this.frameRateIntervals = frameRate;


    }

    // function to start animation

    public void Start()

    {
        fps = 0; // first frame
        variableForTimer = 0f;
        spriteRenderer.sprite = frames[0]; // setting the renderer to the first frame


    }

    public void spriteAnimationUpdater()

    {
        variableForTimer += Time.deltaTime;
        if (variableForTimer > frameRateIntervals)
        {
            fps = (fps + 1) % frames.Count; // if the fps is the last frame then it will go back to the first frame of the animation
            spriteRenderer.sprite = frames[fps]; // changing the sprite renderer to the fps
            variableForTimer -= frameRateIntervals; // resetting the timer


        }


    }

    public List<Sprite> Frames
    {
        get { return frames; }

    }

}
