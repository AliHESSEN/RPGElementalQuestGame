using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // using this so that this class will show up in the inspector
public class Dialog
{

    // making a list of dialogs off strings

    [SerializeField] List<string> lines;

    //Public property to access the 'lines' list

    public List<string> Lines
    {
        
       get { return lines; } // Getter method that returns the 'lines' list


    }



}
