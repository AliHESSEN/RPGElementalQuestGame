using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCharacterAnimator;

public class NPCBattle : MonoBehaviour, interactionInterface
{

    // refrences

    [SerializeField] GameObject triggerImage; // to make it show in the inspector
    [SerializeField] Dialog text; // to make it show in the inspector
    [SerializeField] Dialog textAfterBattle; // to make it show in the inspector
    [SerializeField] GameObject fov;
    [SerializeField] Sprite sprite; // used for npc images
    [SerializeField] string name; // used for name
    GameCharacters gameCharacter; // ref to the gameCaracter

    //variabels
    bool npcBattleStatus = false; // will be used to check if npc or bosses have lost the battle


    //functions

    private void Awake()
    {
        gameCharacter = GetComponent<GameCharacters>();
    }


    private void Start()
    {

        thisFunctionSetsFovAccordingToDirection(gameCharacter.Animator.DefaultDirection); // sett the fov rotation based on the default facing direction of the character

    }

    private void Update()
    {
        gameCharacter.AnimationMovementUpdater();

    }

    public void InteractionFunction(Transform initiator)
    {
        // when the player interacts with the boss or npc it should look at the player

        gameCharacter.FunctionToLookAtTarget(initiator.position); // using the initiator (players) position as parameter

        // then we only wanna start the npc or boss battle if they have not already lost

        if (!npcBattleStatus)
        {

            StartCoroutine(DialogSystem.occurence.ShowTextInGame(text, () => {
                GameControll.gameControllRef.ThisFunctionStartsBattleWithNpc(this);



            }));


        }
        else // if the npc or boss have already been defeated
        {

            StartCoroutine(DialogSystem.occurence.ShowTextInGame(textAfterBattle)); // showing the defeat dialog for npc or boss
        }




    }



    public IEnumerator thisFunctionTriggerNpcBattle(playerMovement variabelUsedForplayer) // with a reference to playerMovment 

    {
        triggerImage.SetActive(true); // activating the icon over the npc that are battling
        yield return new WaitForSeconds(0.5f); // waiting for half a second
        triggerImage.SetActive(false); // turning it off again

        // after that we make the npc move

        // first we need to find the difference vector between the player character and the npc that is battling

        var vectorForDifference = variabelUsedForplayer.transform.position - transform.position;
        var movementVector = vectorForDifference - vectorForDifference.normalized; // we dont want the npc to walk inside the player tile, so we need to implement that with -1 tile
        movementVector = new Vector2(Mathf.Round(movementVector.x), Mathf.Round(movementVector.y)); // since our world is built on tiles we dont want a decimal number, so we are converting them to ints
        yield return gameCharacter.CharacterMovementFunction(movementVector);

        // then the npc will talk and then start the battle


        StartCoroutine(DialogSystem.occurence.ShowTextInGame(text, () => {
            GameControll.gameControllRef.ThisFunctionStartsBattleWithNpc(this);



        }));




    }

    // function used to not let npc / bosses battle the player again after they have been defeated
    public void ThisFunctionWontLetNpcBattleAgainAfterLoss()

    {
        npcBattleStatus = true; // setting it to true
        fov.gameObject.SetActive(false); // disabeling the field of view

    }

    // this function will be used to the set the field of view for the npc depending on their facing direction

    public void thisFunctionSetsFovAccordingToDirection(FacingDirection varForDirection)
    {

        // setting the Z rotation

        float anglePosition = 0f; // staring with Z = 0
        if (varForDirection == FacingDirection.Right)
            anglePosition = 90f;

        else if (varForDirection == FacingDirection.Left)
            anglePosition = 270f;

        else if (varForDirection == FacingDirection.Up)
            anglePosition = 180f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, anglePosition);

    }


    // getting acces to the references

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }




}
