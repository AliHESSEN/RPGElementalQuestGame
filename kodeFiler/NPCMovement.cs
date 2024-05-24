using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class NPCMovement : MonoBehaviour, interactionInterface
{
    // references

    [SerializeField] Dialog dialog;
    [SerializeField] List<Sprite> npcImages; // used for npc spirtes
    [SerializeField] List<Vector2> npcRoute; // list of movements for the npc
    [SerializeField] float timeBetweenNpcMovement; // so we can sett the time from the inspector

    SpriteAnimator spriteAnimator;
    GameCharacters gameCharacter;

    ModesForNpc npcSituation; // npc modes

    // variables

    float waitTime = 0f; // used for time for npcs movements
    int startingRoute = 0; // starts from 0

    private void Awake()
    {
        gameCharacter = GetComponent<GameCharacters>();
    }


    private void Start()
    {
        spriteAnimator = new SpriteAnimator(npcImages, GetComponent<SpriteRenderer>());
        spriteAnimator.Start(); // start the animations



    }

    // Must implement all functions defined in the interface (interactionInterface)
    public void InteractionFunction(Transform charThatInitiat) // initiator will be the character that initiated the interaction
    {


        // we can only talk to npc if they are stadning still
        if (npcSituation == ModesForNpc.Waiting)
        {

            npcSituation = ModesForNpc.Talking; // setting the npc situation to text before showing the text
            gameCharacter.FunctionToLookAtTarget(charThatInitiat.position);
            StartCoroutine(DialogSystem.occurence.ShowTextInGame(dialog, () => { npcSituation = ModesForNpc.Waiting; waitTime = 0f; })); // using a lambda to set the state back to idle when the dialog is over and also setting timer to 0
            //Debug.Log("Talking with NPC");




        }

    }

    private void Update()
    {
        


        if (npcSituation == ModesForNpc.Waiting) // if the situation is idle
        {

            waitTime += Time.deltaTime;
            if (waitTime > timeBetweenNpcMovement) // if idle timer is greater than 2 sec
            {
                waitTime = 0f; // starts on 0
                if (npcRoute.Count > 0)
                    StartCoroutine(thisFunctionMakesNpcWalk()); // then the npc will move up

            }

        }


        gameCharacter.AnimationMovementUpdater();



    }


    IEnumerator thisFunctionMakesNpcWalk()
    {
        npcSituation = ModesForNpc.Walking;

        var previousNpcWalkingLoc = transform.position; // storing the current position

        yield return gameCharacter.CharacterMovementFunction(npcRoute[startingRoute]); // with current walking route for npc

        if (transform.position != previousNpcWalkingLoc) // checking if the old character loc is not the same as the old loc
            startingRoute = (startingRoute + 1) % npcRoute.Count; // increasing the pattern by 1 after moving the npc and move back when it reaches the last pattern

        npcSituation = ModesForNpc.Waiting;
    }


    // npc modes that will be used for walking and talking

    public enum ModesForNpc { Waiting, Walking, Talking }



}
