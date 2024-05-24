using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// this class handles the battle dialog and ability selection UI in a game.
public class BattleBoxText : MonoBehaviour
{
    // Serialized fields to be set in the Unity Editor
    [SerializeField] int lettersPerSecond; // Speed at which letters appear on the screen
    [SerializeField] Color highlightedColor; // Color used to highlight selected options

    [SerializeField] TextMeshProUGUI dialogText; // Reference to the dialog text UI element
    [SerializeField] GameObject choiceSelector; // Reference to the choice selector UI element
    [SerializeField] GameObject abilitySelector; // Reference to the ability selector UI element
    [SerializeField] GameObject abilityDetails; // Reference to the ability details UI element

    [SerializeField] List<TextMeshProUGUI> choiceTexts; // List of text elements for choices
    [SerializeField] List<TextMeshProUGUI> abilityNames; // List of text elements for ability names

    [SerializeField] TextMeshProUGUI apText; // Reference to the text element showing ability points (AP)
    [SerializeField] TextMeshProUGUI elemTypeText; // Reference to the text element showing ability element type

    // Function to set the dialog text instantly
    public void DefineDialog(string defDialog)
    {
        dialogText.text = defDialog;
    }

    // Coroutine to display the dialog text one letter at a time
    public IEnumerator thisFunctionWillTypeDialogText(string dialogInGame)
    {
        dialogText.text = "";
        foreach (var lettersInText in dialogInGame.ToCharArray())
        {
            dialogText.text += lettersInText;
            yield return new WaitForSeconds(1f / lettersPerSecond); // Wait based on letters per second
        }

        yield return new WaitForSeconds(1f); // Additional wait time after displaying the text
    }

    // Function to enable or disable the dialog text UI element
    public void ActivateDialogText(bool active)
    {
        dialogText.enabled = active;
    }

    // Function to enable or disable the choice selector UI element
    public void ActivateChoiceSelector(bool choiceActive)
    {
        choiceSelector.SetActive(choiceActive);
    }

    // Function to enable or disable the ability selector and details UI elements
    public void ActivateAbilitySelector(bool abiActive)
    {
        abilitySelector.SetActive(abiActive);
        abilityDetails.SetActive(abiActive);
    }

    // Function to update the visual style of choice options based on the selected choice
    public void UpdateChoices(int choiceSelected)
    {
        for (int i = 0; i < choiceTexts.Count; ++i)
        {
            if (i == choiceSelected)
                choiceTexts[i].color = highlightedColor; // Highlight the selected choice
            else
                choiceTexts[i].color = Color.black; // Set other choices to black
        }
    }

    // Function to update the ability selection UI
    public void UpdateAbilitySelection(int abilitySelected, Ability ability)
    {
        for (int i = 0; i < abilityNames.Count; ++i)
        {
            if (i == abilitySelected)
                abilityNames[i].color = highlightedColor; // Highlight the selected ability
            else
                abilityNames[i].color = Color.black; // Set other abilities to black
        }
        // Update the AP and element type text
        apText.text = $"PP {ability.Ap}/{ability.Core.Ap}";
        elemTypeText.text = ability.Core.Type.ToString();
    }

    // Function to set ability names in the UI
    public void FunctionToSetabilityNames(List<Ability> ability)
    {
        for (int i = 0; i < abilityNames.Count; ++i)
        {
            if (i < ability.Count)
                abilityNames[i].text = ability[i].Core.Name; // Set ability name if available
            else
                abilityNames[i].text = "-"; // Set to "-" if no ability is available
        }
    }
}
