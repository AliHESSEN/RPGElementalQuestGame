using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver}

public enum BattleAction { Move, SwitchElement, UseItem, Flee }

//This script controls the entire battle
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleBoxText dialogBox;
    [SerializeField] TeamDisplayer partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image npcImage;
    [SerializeField] public AudioSource clickSoundEffect;
    [SerializeField] public AudioSource confirmSoundEffect;

    // events

    public event Action<bool> OnBattleOver; // will be using this to let the gamecontroller now if the battle is over or not

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;



    // variables for party and random enemy elementals

    Element wildElement;
    Party playerParty;
    Party npcParty;

    int FleeAttempts; // used for fleeing from battle
    bool checkIfNpcBattle = false; // will be used to check if the encounter is a npc battle or not

    // need reference to both player and trainer controller to show details for both
    playerMovement player;
    NPCBattle trainer;


    bool AllPlayerElementsFainted() // will be used to check if all the player elementals is dead
    {
        return playerParty.Elements.All(element => element.HP <= 0);
    }

    public void ThisFunctionWillStartTheBattle(Party playerParty, Element wildElement) // calling the start Battle function with wildElement and player party
    {
        checkIfNpcBattle = false;

        this.playerParty = playerParty; // using this because we have the same variable names
        this.wildElement = wildElement; // same here
        player = playerParty.GetComponent<playerMovement>();
        StartCoroutine(FunctionToSetupTheBattle());
    }

    // function to start npc battle
    public void ThisFunctionWillStartNpcBattle(Party playerParty, Party trainerParty) // calling the start npc battle with player party and npc party
    {
        this.playerParty = playerParty; // using this because we have the same variable names
        this.npcParty = trainerParty;
        checkIfNpcBattle = true; // setting it to true

        player = playerParty.GetComponent<playerMovement>(); // getting the components
        trainer = trainerParty.GetComponent<NPCBattle>();

        StartCoroutine(FunctionToSetupTheBattle());
    }

    public IEnumerator FunctionToSetupTheBattle()
    {

        
        //clearing the hud
        playerUnit.FunctionToDisableHudInBattle();
        enemyUnit.FunctionToDisableHudInBattle();
        


        // if its not a npc battle

        if (!checkIfNpcBattle)
        {
            playerUnit.FunctionToDefineElements(playerParty.GetAliveElement()); // with the GetAliveElement function as parameter
            enemyUnit.FunctionToDefineElements(wildElement); //with the party and elements as parameter

            FleeAttempts = 0;

            dialogBox.FunctionToSetabilityNames(playerUnit.Element.Ability);

            yield return dialogBox.thisFunctionWillTypeDialogText($"A wild Element, {enemyUnit.Element.Base.Name} appeared.");
            yield return new WaitForSeconds(1f);

        }
        else // if its a npc battle
        {
            // we disable the elemental sprites first

            playerUnit.gameObject.SetActive(false); // disabeling the player elemental
            enemyUnit.gameObject.SetActive(false); // diabeling the enemy unit

            // then we show the player and npc image

            playerImage.gameObject.SetActive(true); // activating the player character imgage sprite
            npcImage.gameObject.SetActive(true); // same for enemy

            playerImage.sprite = player.Sprite; // sett the sprite for player
            npcImage.sprite = trainer.Sprite; // same for npc

            yield return dialogBox.thisFunctionWillTypeDialogText($"{trainer.Name} is ready to battle you"); // showing a dialog before the battle


            // sending first elemental in the party
            playerImage.gameObject.SetActive(false); // disabeling player image sprite
            playerUnit.gameObject.SetActive(true); // enabeling player elemental sprite in battle
            var playerElemental = playerParty.GetAliveElement();
            playerUnit.FunctionToDefineElements(playerElemental);
            yield return dialogBox.thisFunctionWillTypeDialogText($"{playerElemental.Base.Name}");
            dialogBox.FunctionToSetabilityNames(playerUnit.Element.Ability); // setting up the UI for moves


            // for npc

            npcImage.gameObject.SetActive(false); // turning off the sprite
            enemyUnit.gameObject.SetActive(true); // show the enemey elemental sprite
            var enemyElemental = npcParty.GetAliveElement();
            enemyUnit.FunctionToDefineElements(enemyElemental);



        }


        partyScreen.Init();
        FunctionThatLetsThePlayerSelectActions();




    }



    void ThisFunctionWillBeUsedToCallBattleOver(bool won) // battle over functon
    {
        state = BattleState.BattleOver;
        playerParty.Elements.ForEach(p => p.BattleEndingFunction());
        OnBattleOver(won);
    }

    void FunctionThatLetsThePlayerSelectActions() // for action selection
    {
        state = BattleState.ActionSelection;
        StartCoroutine(DisplayActionDialogFunction());

        

    }

    void FunctionThatShowPlayerPartyScreen() // shows player party /team screen
    {
        state = BattleState.PartyScreen;
        partyScreen.functionToSetUpelementalTeamData(playerParty.Elements);
        partyScreen.gameObject.SetActive(true);

    }

    private bool battleTextIsTyping = false; // will be used as a check to not let the players press when dialog is being typed
    IEnumerator DisplayActionDialogFunction() // show text in battle
    {
        battleTextIsTyping = true; // Sett til true når dialogen starter
        yield return StartCoroutine(dialogBox.thisFunctionWillTypeDialogText("Choose an action"));
        yield return new WaitForSeconds(1f); // Pause for 1 second
        dialogBox.ActivateChoiceSelector(true);
        battleTextIsTyping = false; // Sett til false når dialogen er ferdig

    }


    //After the playeraction, the player needs to choose a move
    void FunctionUsedToLetPlayerSelectMoves()
    {
        state = BattleState.MoveSelection;
        dialogBox.ActivateChoiceSelector(false);
        dialogBox.ActivateDialogText(false);
        dialogBox.ActivateAbilitySelector(true);
    }



    IEnumerator TurnDeciderFunction(BattleAction choicePlayer)
    {
        state = BattleState.RunningTurn;

        if (choicePlayer == BattleAction.Move)
        {
            playerUnit.Element.AbilityiCurrent = playerUnit.Element.Ability[currentMove];
            enemyUnit.Element.AbilityiCurrent = enemyUnit.Element.RandomEnemyMoveFunction();

            //Check who goes first
            bool varCheckPlayerFirstTurn = playerUnit.Element.Speed >= enemyUnit.Element.Speed;

            var firstElement = (varCheckPlayerFirstTurn) ? playerUnit : enemyUnit;
            var secondObject = (varCheckPlayerFirstTurn) ? enemyUnit : playerUnit;

            var secondElement = secondObject.Element;

            //first turn
            yield return ManageBattleFunction(firstElement, secondObject, firstElement.Element.AbilityiCurrent);
            yield return FunctionForWhenTurnIsDoneLogic(firstElement);
            if (state == BattleState.BattleOver) yield break;

            if (secondElement.HP > 0)
            {
                //second turn
                yield return ManageBattleFunction(secondObject, firstElement, secondObject.Element.AbilityiCurrent);
                yield return FunctionForWhenTurnIsDoneLogic(secondObject);
                if (state == BattleState.BattleOver) yield break;
            }

        }
        else
        {
            if (choicePlayer == BattleAction.SwitchElement)
            {
                var selectedElement = playerParty.Elements[currentMember];
                state = BattleState.Busy;
                yield return FunctionToChangeElement(selectedElement);
            }
            else if (choicePlayer == BattleAction.Flee)
            {
                yield return FunctionForFleeAttempts();
            }

            //enemy gets turn
            var enemyMove = enemyUnit.Element.RandomEnemyMoveFunction();
            yield return ManageBattleFunction(enemyUnit, playerUnit, enemyMove);
            yield return FunctionForWhenTurnIsDoneLogic(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            FunctionThatLetsThePlayerSelectActions();

    }


    IEnumerator ManageBattleFunction(BattleUnit originalUnit, BattleUnit otherUnit, Ability ability)
    {
        bool checkIfAllowed = originalUnit.Element.DetermineIfMoveIsAllowed();
        if (!checkIfAllowed)
        {
            yield return DisplayStatusChangesFunction(originalUnit.Element);
            yield return originalUnit.BothHuds.FunctionToUpdateElementalsHP();
            yield break;
        }

        ability.Ap--;
        //This will show the player element used move on the screen
        yield return dialogBox.thisFunctionWillTypeDialogText($"{originalUnit.Element.Base.Name} used {ability.Core.Name}");

        originalUnit.FunctionToShowAttackAnimation();
        yield return new WaitForSeconds(1f); //wait for 1 second before the enemy recive damage
        otherUnit.FunctionToShowHitAnimation();

        if (ability.Core.AbilityCategory == AbilityCategory.Status)
        {
            yield return ManageBattleEffects(ability, originalUnit.Element, otherUnit.Element);
        }
        else
        {
            
            var dataForDamage = otherUnit.Element.FunctionForTakingDamage(ability, originalUnit.Element); //This will apply damage to the enemy
            yield return otherUnit.BothHuds.FunctionToUpdateElementalsHP();
            yield return DisplayingDamageInformationFunction(dataForDamage);
        }

        if (otherUnit.Element.HP <= 0)
        {
            yield return DeadElementsLogicFunction(otherUnit);
        }


    }

    IEnumerator ManageBattleEffects(Ability ability, Element root, Element endTarget)
    {
        //status Condition
        var statusManagerVar = ability.Core.AbilityEffects;
        if (ability.Core.AbilityEffects.Buffs != null)
        {
            if (ability.Core.AbilityTarget == AbilityTarget.Self)
                root.FunctionToAddBoosts(statusManagerVar.Buffs);
            else
                endTarget.FunctionToAddBoosts(statusManagerVar.Buffs);
        }

        //stat boosting
        if (statusManagerVar.Buffs != null)
        {
            if (ability.Core.AbilityTarget == AbilityTarget.Self)
                root.FunctionToAddBoosts(statusManagerVar.Buffs);
            else
                endTarget.FunctionToAddBoosts(statusManagerVar.Buffs);
        }

        //stauts condition
        if (statusManagerVar.Status != HarmfulEffectID.none)
        {
            endTarget.FunctionForStatus(statusManagerVar.Status);
        }

        //harmful effects conditions
        if (statusManagerVar.HarmfulEffect != HarmfulEffectID.none)
        {
            endTarget.functionToSetHarmfulEffects(statusManagerVar.HarmfulEffect);
        }

        yield return DisplayStatusChangesFunction(root);
        yield return DisplayStatusChangesFunction(endTarget);
    }

    IEnumerator DisplayStatusChangesFunction(Element element) // show status changes for an element
    {
        while (element.ChangeInStatus.Count > 0) // keep displaying status changes 
        {
            var statusInCombatText = element.ChangeInStatus.Dequeue();
            yield return dialogBox.thisFunctionWillTypeDialogText(statusInCombatText);
        }

    }

    IEnumerator FunctionForWhenTurnIsDoneLogic(BattleUnit startObject)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //statuses like burn or psn will hurt the element after the turn
        startObject.Element.ChoiceAfterTurn();
        yield return DisplayStatusChangesFunction(startObject.Element);
        yield return startObject.BothHuds.FunctionToUpdateElementalsHP();
        if (startObject.Element.HP <= 0)
        {
            yield return DeadElementsLogicFunction(startObject);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);

        }
    }

    IEnumerator DeadElementsLogicFunction(BattleUnit deadElement)
    {
        yield return dialogBox.thisFunctionWillTypeDialogText($"{deadElement.Element.Base.Name} Fainted");
        deadElement.FunctionToShowDeathAnimation();
        
        yield return new WaitForSeconds(2f); //waiting for 2 sec

        if (!deadElement.CheckForPlayerUnit)
        {
            //Exp gain
            int expGainAfterKill = deadElement.Element.Base.ExpAmountGiven;
            int enemyElementLevel = deadElement.Element.Level;
            //float trainerBonus = (isTrainerBattle)? 1.5f : 1f;

            int calcExp = Mathf.FloorToInt(expGainAfterKill * enemyElementLevel /* * trainerBonus*/) / 7;
            playerUnit.Element.Exp += calcExp;
            yield return dialogBox.thisFunctionWillTypeDialogText($"{playerUnit.Element.Base.Name} got {calcExp} exp");
            yield return playerUnit.BothHuds.FunctionForExpAnimation();

            //check level up
            while (playerUnit.Element.FunctionForLevelUpCheck())
            {
                playerUnit.BothHuds.FunctionToSetElementLevel();
                yield return dialogBox.thisFunctionWillTypeDialogText($"{playerUnit.Element.Base.Name} increased to level {playerUnit.Element.Level}"); 

                yield return playerUnit.BothHuds.FunctionForExpAnimation(true);

            }


        }

        ThisFunctionChecksIfTheBattleIsDone(deadElement);
    }

    void ThisFunctionChecksIfTheBattleIsDone(BattleUnit deadElement)
    {
        if (deadElement.CheckForPlayerUnit)
        {
            var nextElementInParty = playerParty.GetAliveElement();
            if (nextElementInParty != null)
                FunctionThatShowPlayerPartyScreen();

            else
                if (AllPlayerElementsFainted())
            {
                ThisFunctionWillBeUsedToCallBattleOver(false); //  will be set to false that the player lost
                // change to Game Over-scene
                SceneManager.LoadScene("GameOverScene");

                
            }

        }
        else

            if (!checkIfNpcBattle)
        {
            ThisFunctionWillBeUsedToCallBattleOver(true); // ending the battle if its not an npc battle


        }
        else

        {

            // check if there is anymore healty elementals in the npc party
            var nextElementalNPCParty = npcParty.GetAliveElement();
            if (nextElementalNPCParty != null)
                StartCoroutine(functionThatSendsNextNpcElemental(nextElementalNPCParty)); // if there is still alive elemental in the npcs group

            else
                ThisFunctionWillBeUsedToCallBattleOver(true); // if not then the battle is over



        }




    }

    IEnumerator DisplayingDamageInformationFunction(DamageInformation dmgInfoInBattle) // show damage information in battle
    {
        if (dmgInfoInBattle.Critical) // If the attack is a critical hit
            yield return dialogBox.thisFunctionWillTypeDialogText("Critical hit");

        if (dmgInfoInBattle.Type > 1f) // If the attack is effective display a message and wait for 1,5
        {
            yield return dialogBox.thisFunctionWillTypeDialogText("Its weak to that element");
            yield return new WaitForSeconds(1.5f);
        }


        else if (dmgInfoInBattle.Type < 1f) // same if its not effective
        {
            yield return dialogBox.thisFunctionWillTypeDialogText("Its resistant to that element");
            yield return new WaitForSeconds(1.5f);
        }


    }

    //After the playerAction, the player need to choose an action
    public void FunctionToUpdateChoices()
    {
        if (state == BattleState.ActionSelection)
        {
            functionToHandlePlayerAction();
        }
        else if (state == BattleState.MoveSelection)
        {
            functionToHandlePlayerAbilitySelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            functionToHandlePlayerPartySelection();
        }
    }


    void functionToHandlePlayerAction() //the selected action will change based on the user input
    {

        if (battleTextIsTyping)
        {
            // Dialog is being typed, ignore inputs
            return;
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            clickSoundEffect.Play(); // play sound effect
            ++currentAction;

        }
            
        else if (Input.GetKeyDown(KeyCode.A))
        {
            clickSoundEffect.Play(); // play sound effect
            --currentAction;
        }
           
        else if (Input.GetKeyDown(KeyCode.S))
        {
            clickSoundEffect.Play(); // play sound effect
            currentAction += 2;
        }
           
        else if (Input.GetKeyDown(KeyCode.W))
        {
            clickSoundEffect.Play(); // play sound effect
            currentAction -= 2;
        }
            

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateChoices(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            confirmSoundEffect.Play(); // play confirm sound effect
            if (currentAction == 0)
            {
                //Fight is selected
                FunctionUsedToLetPlayerSelectMoves();
            }
            else if (currentAction == 1)
            {
                //Purse is selected
            }
            else if (currentAction == 2)
            {
                //Element is selected
                prevState = state;
                FunctionThatShowPlayerPartyScreen();
            }
            else if (currentAction == 3)
            {
                //flee is selected
                StartCoroutine(TurnDeciderFunction(BattleAction.Flee));

            }
        }
    }


    void functionToHandlePlayerAbilitySelection() // controlls the player ability selection
    {
        if (battleTextIsTyping)
        {
            // ignore inputs if text is being typed
            return;
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            clickSoundEffect.Play();
            ++currentMove;
        }
            
        else if (Input.GetKeyDown(KeyCode.A))
        {
            clickSoundEffect.Play();
            --currentMove;
        }
            
        else if (Input.GetKeyDown(KeyCode.S))
        {
            clickSoundEffect.Play();
            currentMove += 2;
        }
            
        else if (Input.GetKeyDown(KeyCode.W))
        {
            clickSoundEffect.Play();
            currentMove -= 2;
        }
            

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Element.Ability.Count - 1);

        dialogBox.UpdateAbilitySelection(currentMove, playerUnit.Element.Ability[currentMove]);

        //perform a move
        if (Input.GetKeyDown(KeyCode.Z))
        {
            confirmSoundEffect.Play();
            dialogBox.ActivateAbilitySelector(false);
            dialogBox.ActivateDialogText(true);
            StartCoroutine(TurnDeciderFunction(BattleAction.Move)); //This will perform the move
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            confirmSoundEffect.Play();
            dialogBox.ActivateAbilitySelector(false);
            dialogBox.ActivateDialogText(true);
            FunctionThatLetsThePlayerSelectActions();
        }
    }

    void functionToHandlePlayerPartySelection()
    {
        if (battleTextIsTyping)
        {
            // text is typed ignore inputs
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            clickSoundEffect.Play();
            ++currentMember;
        }
            
        else if (Input.GetKeyDown(KeyCode.A))
        {
            clickSoundEffect.Play();
            --currentMember;
        }
           
        else if (Input.GetKeyDown(KeyCode.S))
        {
            clickSoundEffect.Play();
            currentMember += 2;
        }
            
        else if (Input.GetKeyDown(KeyCode.W))
        {
            clickSoundEffect.Play();
            currentMember -= 2;
        }
            

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Elements.Count - 1);

        partyScreen.ElementPickFunction(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            confirmSoundEffect.Play(); // play confirm sound effect
            var elementPicked = playerParty.Elements[currentMember];
            if (elementPicked.HP <= 0)
            {
                partyScreen.FunctionToShowInfoText("Element is knocked out, pick another");
                return;
            }
            if (elementPicked == playerUnit.Element)
            {
                partyScreen.FunctionToShowInfoText("This is the same one, cant pick this one");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(TurnDeciderFunction(BattleAction.SwitchElement));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(FunctionToChangeElement(elementPicked));

            }


        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            confirmSoundEffect.Play(); // play confirm sound
            partyScreen.gameObject.SetActive(false);
            FunctionThatLetsThePlayerSelectActions();
        }


    }

    IEnumerator FunctionToChangeElement(Element changedElement) // change the element of a player  in battle
    {

        if (playerUnit.Element.HP > 0) // If the player current element has remaining HP show  a switch animation
        {

            yield return dialogBox.thisFunctionWillTypeDialogText($"Return! {playerUnit.Element.Base.name}");
            playerUnit.FunctionToShowDeathAnimation();
            yield return new WaitForSeconds(2f);
        }
        // Set the player element to the new one and update ability names
        playerUnit.FunctionToDefineElements(changedElement);
        dialogBox.FunctionToSetabilityNames(changedElement.Ability);
        yield return dialogBox.thisFunctionWillTypeDialogText($"Get in {changedElement.Base.Name}!");

        state = BattleState.RunningTurn;
    }

    IEnumerator functionThatSendsNextNpcElemental(Element changedNPCElemental)
    {

        state = BattleState.Busy;
        enemyUnit.FunctionToDefineElements(changedNPCElemental); // sending out the next elemental
        yield return dialogBox.thisFunctionWillTypeDialogText($"{trainer.Name}: your turn {changedNPCElemental.Base.Name}");

        state = BattleState.RunningTurn; // so the battle can continue



    }



    IEnumerator FunctionForFleeAttempts()
    {
        state = BattleState.Busy;
        if (checkIfNpcBattle)
        {
            yield return dialogBox.thisFunctionWillTypeDialogText($"you cant get away!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++FleeAttempts;

        int playerElementalSpeedStat = playerUnit.Element.Speed;
        int enemyElemetntalSpeedStat = enemyUnit.Element.Speed;

        if (enemyElemetntalSpeedStat < playerElementalSpeedStat)
        {
            yield return dialogBox.thisFunctionWillTypeDialogText($"Got away safely");
            ThisFunctionWillBeUsedToCallBattleOver(true);
        }
        else
        {
            float fleeProb = (playerElementalSpeedStat * 128) / enemyElemetntalSpeedStat + 30 * FleeAttempts;
            fleeProb = fleeProb % 256;

            if (UnityEngine.Random.Range(0, 256) < fleeProb)
            {
                yield return dialogBox.thisFunctionWillTypeDialogText($"Got away safely");
                ThisFunctionWillBeUsedToCallBattleOver(true);

            }
            else
            {
                yield return dialogBox.thisFunctionWillTypeDialogText($"gotta stay!");
                state = BattleState.RunningTurn;
            }
        }

    }

}

