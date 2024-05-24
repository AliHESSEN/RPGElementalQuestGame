using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elementsInZone : MonoBehaviour
{

    // making a list for the wild elementals in the zone

   [SerializeField] List<Element> wildElements;

    // function to give the player a random elemental when triggering a battle in the zone

    public Element randomElementInBattle()

    {
        // stroing it in a variable
        var wildElement = wildElements[Random.Range(0, wildElements.Count)]; // return a random value from 0 to the number in the list

        // initialization it

        wildElement.FunctionToInitializElementalMoves();
        return wildElement;

    }


}
