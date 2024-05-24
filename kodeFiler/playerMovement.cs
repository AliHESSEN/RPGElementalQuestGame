using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class playerMovement : MonoBehaviour
{

    //refrences

    [SerializeField] Sprite sprites; // used for npc images
    [SerializeField] string names; // used for name


    private Vector2 PlayerMovementInput; //used to take inputs

    private GameCharacters character; // reference to the Character class
    
    

    //events

    public event Action triggeredEncounter;
    public event Action<Collider2D> EnteredNPCTriggerRange;


    // functions

    private void Awake()
    {
        
        character = GetComponent<GameCharacters>(); // used for characters

    }



   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void PlayerMovementUpdater()
    { 
        
        

        if (!character.CheckMovement) // if ChackMovement is not false
        {
            PlayerMovementInput.x = Input.GetAxisRaw("Horizontal"); // takes x-axis
            PlayerMovementInput.y = Input.GetAxisRaw("Vertical"); // takes y-axis

            // removing diagonal movement
            if (PlayerMovementInput.x != 0) PlayerMovementInput.y = 0;

            // if input is not 0

            if (PlayerMovementInput != Vector2.zero)

            {



                StartCoroutine(character.CharacterMovementFunction(PlayerMovementInput, RunBothBattleFunctions)); // with input and check for battle function as parameters


                

            }
 

        }

        character.AnimationMovementUpdater();

   

        if (Input.GetKeyDown(KeyCode.Z))
        {
            interactionFunction(); // kjører interaksjon funksjonen
        }


    }

    void interactionFunction()
    {
        var playerFacingDirection = new Vector3(character.Animator.moveX, character.Animator.moveY); // gets moveX og moveY from animator
        var interactionPosition = transform.position + playerFacingDirection; // get char position

        var InteractionCheck = Physics2D.OverlapCircle(interactionPosition, 0.2f, LayersInGame.layerRef.InteractionLayer);
        
        // checks if the value is not 0
        if (InteractionCheck != null) // if there is something the player can interact with
        {
            InteractionCheck.GetComponent<interactionInterface>()?.InteractionFunction(transform); // sending with the interactFunction
        }


    }

    private void RunBothBattleFunctions() // will use this function to run both check for random encounter and npc battles
    {

        BattleEncounter();
        CheckIfInTrainerView();



    }





    // function for battle check
    private void BattleEncounter()
    {

        
        Vector2 bottomLeft = new Vector2(transform.position.x - 0.15f, transform.position.y - 0.15f);
        Vector2 topRight = new Vector2(transform.position.x + 0.15f, transform.position.y + 0.15f);
        
        // check if the player is walking over grass
       

        if (Physics2D.OverlapArea(bottomLeft, topRight, LayersInGame.layerRef.LongGrass) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 13)
            {
                
                character.Animator.CharacterMovingCheck = false; // turn off player movement in battle
                triggeredEncounter();

            }
        }



    }


    // function for npc battles
    private void CheckIfInTrainerView()

    {

        Vector2 bottomLeft2 = new Vector2(transform.position.x - 0.15f, transform.position.y - 0.15f);
        Vector2 topRight2 = new Vector2(transform.position.x + 0.15f, transform.position.y + 0.15f);


        var TrainerViewTrigger = Physics2D.OverlapArea(bottomLeft2, topRight2, LayersInGame.layerRef.FovLayer);

        if (TrainerViewTrigger != null) // checking if its not null

        {

            // triggering the npc battle event

            character.Animator.CharacterMovingCheck = false; // turn off player movement
            EnteredNPCTriggerRange?.Invoke(TrainerViewTrigger);



        }



    }


    public string Name
    {
        get => names;
    }

    public Sprite Sprite
    {
        get => sprites;
    }





}


