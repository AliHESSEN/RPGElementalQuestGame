using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Party : MonoBehaviour
{

    [SerializeField] List<Element> elements; // making a list of elements

    public List<Element> Elements {
        get {
            return elements;
        }
    }

    private void Start()
    {
        foreach (var element in elements) // looping through all elements 
        {
            element.FunctionToInitializElementalMoves(); // call the initialization function
        }


    }

    // function to return elements that are not fainted in party

    public Element GetAliveElement()

    {
        //using link to check if the element HP is greater than 0

        return elements.Where(x => x.HP > 0).FirstOrDefault(); // the where function will loop thorugh the list of elements in the player party and return the onse that have HP > 0
                                                        // and return the first one





    }


}
