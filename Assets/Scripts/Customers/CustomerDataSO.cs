using UnityEngine;

[CreateAssetMenu(fileName = "CustomerDataSO", menuName = "Scriptable Objects/CustomerDataSO")]
public class CustomerDataSO : ScriptableObject
{
    public enum Race
    {
        HUMAN,
        ELF,
        ORC,
        DWARF,
        DEMON,
        FAIRY,
        UNDEAD
    }

    public enum Kingdom
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    public enum Occupation
    {
        BARD,
        WARRIOR,
        CLERIC,
        DRUID,
        RANGER,
        WIZARD
    }

    public string customerName;
    public string customerDialogue;
    public int customerLevel;

    public Race customerRace;
    public Kingdom kingdom;
    public Occupation occupation;
}
