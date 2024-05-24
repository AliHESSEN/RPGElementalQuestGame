using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class includes data of abilitis that will change during battles
public class Ability
{
    public AbilityCore Core { get; set; }
    public int Ap { get; set; }

    public Ability(AbilityCore elemCore)
    {
        Core = elemCore;
        Ap = elemCore.Ap;
    }
}
