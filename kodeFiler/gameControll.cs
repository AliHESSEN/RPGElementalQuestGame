using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// defining the different gamestates

public enum GameState {OpenWorld, ElementBattle, Text, Locked }

public class GameControll : MonoBehaviour

{
    // defining refrences

    GameState situation;

    // making the gameControll accesable to every class as areference

    public static GameControll gameControllRef { get; private set; }

    private void Awake()
    {
        gameControllRef = this;
        ConditionsDB.Init();

    }

    [SerializeField] playerMovement playerMovements; // player controll
    [SerializeField] BattleSystem battleSystem; // battle system
    [SerializeField] Camera mainCamera; // camera
    [SerializeField] public AudioSource backGroundMusic; // background music
    [SerializeField] public AudioSource battleMusic; // battle music
    [SerializeField] public AudioSource npcBattleMusic; // for npc battle music



    private void Start()
    {

        // when the player triggers an encounter
        playerMovements.triggeredEncounter += thisFunctionBeginsElementalBattle;
        battleSystem.OnBattleOver += thisFunctionWillEndBattle;
        playerMovements.EnteredNPCTriggerRange += (Collider2D charHitBox) => // when this event is triggered then this lambda function will run with a collider2D
        {

            // we can use this collider to get refrences to the NPC controll
            var npcVar = charHitBox.GetComponentInParent<NPCBattle>(); // since its in the Fov that is a child of npc

            if (npcVar != null) // if trainer is not null
            {

                situation = GameState.Locked; // setting the state to locked to not let the player move when the npc have triggered the event
                StartCoroutine(npcVar.thisFunctionTriggerNpcBattle(playerMovements)); // then the triggerTrainerBattle will run

            }


        };


        // for dialogs

        DialogSystem.occurence.dialogIsShown += () =>
        {

            situation = GameState.Text; // change the state to text using lambda
        };

        DialogSystem.occurence.dialogIsClosed += () =>
        {
            if (situation == GameState.Text) // only do this if the current state is text
                situation = GameState.OpenWorld; // if the dialog box is closed return to open world
        };



    }

    //start battle function

    void thisFunctionBeginsElementalBattle()

    {
        //setting the state to battle
        backGroundMusic.gameObject.SetActive(false); // turns off background music
        situation = GameState.ElementBattle;
        battleMusic.gameObject.SetActive(true); //turns on battle music
        // enables the battle gameobject since its turned off
        battleSystem.gameObject.SetActive(true);
        // after that we need to disable the main camera so we dont have two camera active at the same time
        mainCamera.gameObject.SetActive(false);

        var playerCharTeam = playerMovements.GetComponent<Party>(); // // Retrieves the team component from the playerMovements object and stores it in the variable.

        var untamedElements = FindObjectOfType<elementsInZone>().GetComponent<elementsInZone>().randomElementInBattle(); // Finds an instance of the elementsInZone script in the scene and gets its elementsInZone component and calls the randomElementInBattle function 
        battleSystem.ThisFunctionWillStartTheBattle(playerCharTeam, untamedElements); // starting the battle with the parameter for elementals in open world and team for player


    }

    NPCBattle npc;

    public void ThisFunctionStartsBattleWithNpc(NPCBattle npc1) // with npc as the parameter

    {
        //setting the state to battle

        situation = GameState.ElementBattle;
        // enables the battle gameobject since its turned off
        battleSystem.gameObject.SetActive(true);
        npcBattleMusic.gameObject.SetActive(true); // turning on npc battle music
        // after that we need to disable the main camera so we dont have two camera active at the same time
        backGroundMusic.gameObject.SetActive(false); // turns of bg music
        mainCamera.gameObject.SetActive(false);

        this.npc = npc1;

        var charParty = playerMovements.GetComponent<Party>(); // // Retrieves the Party component from the playerMovements object and stores it in the variable charParty
        var npcParty = npc1.GetComponent<Party>();

        battleSystem.ThisFunctionWillStartNpcBattle(charParty, npcParty); // starting the battle with the parameter for npc party and party for player


    }

    //end battle function

    void thisFunctionWillEndBattle(bool variableForWinning)
    {

        if (npc != null && variableForWinning == true) // if its a npc battle and the player won
        {
            npc.ThisFunctionWontLetNpcBattleAgainAfterLoss(); // calling the Battle lost function that disabels the npc field of view
            npc = null; // adn setting the npc to 0

        }


        //if we win we set the game state to openworld
        situation = GameState.OpenWorld;
        battleMusic.gameObject.SetActive(false); // turns off battle music
        battleSystem.gameObject.SetActive(false); // turning off battle scene
        npcBattleMusic.gameObject.SetActive(false); // turning off npc battle music
        mainCamera.gameObject.SetActive(true); // turning on the main camera
        backGroundMusic.gameObject.SetActive(true); // turns on bg music

    }

    // Update is called once per frame
    private void Update()
    {

        // if the state is in open world then we give the controll to the player

        if (situation == GameState.OpenWorld)
        {
            //then we call the player movement update function
            playerMovements.PlayerMovementUpdater();


        }

        // and if its in battle then we give the controll to the battle system
        else if (situation == GameState.ElementBattle)

        {
            // then we call the update battle function
            battleSystem.FunctionToUpdateChoices();



        }

        // if the player is interacting with something
        else if (situation == GameState.Text)

        {
            DialogSystem.occurence.InteractionUpdater(); // then we give the controll to the update function for interaction

        }



    }





}
