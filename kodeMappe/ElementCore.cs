using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Element/Create new Element")] // to create new elemetns 
public class ElementCore : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string elementInformation;

    [SerializeField] Sprite elemImageFront;
    [SerializeField] Sprite elemImageBack;

    [SerializeField] ElementType Type1;
    [SerializeField] ElementType Type2;
    [SerializeField] GenderType Gender;

    //Base stats for Elements
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int magicDefense;
    [SerializeField] int magic;
    [SerializeField] int speed;
    [SerializeField] int expAmountGiven;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] List<LearnableMove> learnableMoves; // moves to learn

    public int FunctionTogetExp(int levelElemental)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (levelElemental * levelElemental * levelElemental) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return levelElemental * levelElemental * levelElemental;
        }

        return -1;
    }


    public string Name
    {
        get { return name; }
    }

    public string ElementInformation
    {
        get { return elementInformation; }
    }

    public Sprite ElemImageFront
    {
        get { return elemImageFront; }
    }

    public Sprite ElemImageBack
    {
        get { return elemImageBack; }
    }

    public ElementType type1
    {
        get { return Type1; }
    }

    public ElementType type2
    {
        get { return Type2; }
    }

    public GenderType gender
    {
        get { return Gender; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Magic
    {
        get { return magic; }
    }

    public int MagicDefence
    {
        get { return magicDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public int ExpAmountGiven => expAmountGiven;

    public GrowthRate GrowthRate => growthRate;
}




[System.Serializable]
public class LearnableMove
{
    [SerializeField] AbilityCore moveBase;
    [SerializeField] int level;

    //lager properties for å expose disse
    public AbilityCore Base
    {
        get { return moveBase; }
    }
    public int Level
    {
        get { return level; }
    }
}

public enum ElementType  //All the types in our game
{
    None, // index = 0
    Normal, // index = 1
    Fire, // index = 2
    Water, // index = 3
    Lightning, // index = 4
    Nature, // index = 5
    Ice, // index = 6
    Earth, // index = 7
    Air, // index = 8
    Dark, // index = 9
    Light, // index = 10
    Holy, // index = 11
    Death, // index = 12
    Poison

}

public enum GrowthRate
{
    Fast, MediumFast
}

public enum Stat
{
    Attack,
    Defense,
    Magic,
    MagicDefence,
    Speed
}

// will make a table to store the type and their effectivness against each other. 
public class TypeTable

{
    // the type need to be in the same order as the ElementType

    //making a 2D array

    static float[][] tableForTypes =
    {
        //rows = atkType and columns = defType

        // nor fire water lightning nature, ice , earth, air, dark, light, holy , death
        new float[] {1f, 0.5f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 0.5f, 0.5f, 0f},
        new float[] {1f, 0.5f, 0.5f, 1f, 2f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f}, //fire
        new float[] {1f,2f,0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f}, // water
        new float[] {1f, 1f, 2f, 0.5f, 0.5f, 1f, 0f, 0.5f, 1f, 0.5f, 1f, 1f}, // lightning
        new float[] {1f, 0.5f, 1f, 1f, 0.5f, 0.5f, 0.5f, 1f, 0.5f, 1f, 1f, 0.5f}, // nature
        new float[] {1f, 0.5f, 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 1f}, // ice
        new float[] {1f, 0.5f, 1f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f}, // earth
        new float[] {1f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 0.5f, 1f, 1f, 1f, 1f}, //air
        new float[] {1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 2f, 0.5f, 0.5f}, // dark
        new float[] {1f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 0.5f, 1f}, //light
        new float[] {1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 2f}, //holy
        new float[] {1f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f, 2f, 0f}, //death
        new float[] {1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 0.5f} //poison
    };
        
    public static float GetElementalEffectiveness(ElementType atkType, ElementType defType)
    {
        // if there is no difference in attack type and defence type then we return 1
        if (atkType == ElementType.None || defType == ElementType.None)
        
            return 1;

            // get the effectivness from the table
            int rows = (int)atkType - 1;
            int columns = (int)defType - 1;

            // returning the table with rows and columns

            return tableForTypes[rows][columns];



    }


}

public enum GenderType //the genders for elements
{
    Male,
    Female

}
