using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    // reference to the npc dialogBox and dialogText

    [SerializeField] GameObject npcDialogBox;
    [SerializeField] TextMeshProUGUI npcDialogText;
    [SerializeField] int lettersPerSecond;
    


    // event to notify the game that the state of game need to change to dialog

    public event Action dialogIsShown; // this event will be triggered when the dialog is active
    public event Action dialogIsClosed; // this event will be triggered when the dialog is closed


    
    // to make sure that there is only one instance of DialogSystem throughout the program
    public static DialogSystem occurence {  get; private set; } // since its static we can reference it from every class we want

    private void Awake()
    {
        // Set the instance to the current object
        occurence = this; // will be called when the object is first created in the scene


    }

    // variables

    int variableToStoreCurrentLine = 0; // start at 0 to store the number of current lines, so the function know if the dialog is done or not
    Dialog dialog; // making it gloably
    bool textIsBeingTyped; // will be used as a check if a text is being typed or not
    Action textIsFinished; // will be used to check if dialogs is done or not

    public bool DialogTextIsShowingInGame { get; private set; }


    // making a function to show the text in game
    public IEnumerator ShowTextInGame(Dialog dialog, Action textIsDone = null) // will take an object of the dialog class and using an action textIsDone to know if the dialog is finished or not
    {

        // waiting for one frame to not cause issues with pressing z and dialog boxes activating at the same time

        yield return new WaitForEndOfFrame();


        // when the dialog is active we need to trigger the event

        dialogIsShown?.Invoke(); // triggers the event if its not null

        DialogTextIsShowingInGame = true; // will be set to true when dialog box is open

        this.dialog = dialog; // using this since dialog was defined as gloably
        textIsFinished = textIsDone; // setting it to onFinished variabel

        // first we need to activate the dialogBox where the text will show up
        npcDialogBox.SetActive(true); // activating the npc dialogBox

        StartCoroutine(ThisFunctionTypesText(dialog.Lines[0])); // using the function to type text with the first line from the dialog
        



    }

    // function when player is interaction with an object
    public void InteractionUpdater()
    {
        // check player inputs first

        if (Input.GetKeyDown(KeyCode.Z) && !textIsBeingTyped) // if the player presses z and no text is being typed
        {
            ++variableToStoreCurrentLine;
            if (variableToStoreCurrentLine < dialog.Lines.Count) // if the current lines is less than the lines in the dialog box
            {
                StartCoroutine(ThisFunctionTypesText(dialog.Lines[variableToStoreCurrentLine])); // then we show the other line by calling the function with the value in currentLine
            }
            else // otherwise we close the dialogbox and run the closed dialog event
            {
                variableToStoreCurrentLine = 0; // resetting the current lines back to zero so it wont cause any issues when the player interacts with something else
                DialogTextIsShowingInGame = false; // will be set to false when the dialog box is closed
                npcDialogBox.SetActive(false); // closing the dialogBox
                textIsFinished?.Invoke(); // after closing the dialogBox then we run the testIsFinished action
                dialogIsClosed?.Invoke(); // calling the closed dialog event

            }


        }



    }


    // function to animate the dialog text
    public IEnumerator ThisFunctionTypesText(string textLines)
    {
        textIsBeingTyped = true; // set it to true when its typing a text
        npcDialogText.text = "";
        foreach (var lettersInTextLines in textLines.ToCharArray()) // looping through each letter in the text
        {
            npcDialogText.text += lettersInTextLines; // then we add it one by one
            yield return new WaitForSeconds(1f / lettersPerSecond); //1 second show 30 letters

        }
        textIsBeingTyped = false; // set it to false after its done typing

        

    }


}
