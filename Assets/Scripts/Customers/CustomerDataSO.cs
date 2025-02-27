using UnityEngine;

[CreateAssetMenu(fileName = "CustomerDataSO", menuName = "Scriptable Objects/CustomerDataSO")]
public class CustomerDataSO : ScriptableObject
{
    public string customerName;
    public string customerDialogue;
    public int customerAge;

    public Race customerRace;
    public Kingdom kingdom;
    public Occupation occupation;
}

public enum Race
{
    HUMAN,
    ELF,
    ORC,
    DWARF,
    DEMON,
    FAIRY,
    UNDEAD,
    NONE
}

public enum Kingdom
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
    NONE
}

public enum Occupation
{
    BARD,
    WARRIOR,
    CLERIC,
    DRUID,
    RANGER,
    WIZARD,
    NONE
}
