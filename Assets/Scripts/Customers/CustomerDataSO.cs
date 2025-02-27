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
    public Guild guild;
    public GuildRank guildRank;
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

public enum Guild
{
    HOLY,
    ASSASSIN,
    MERCHANT,
    MAGE,
    NATURE,
    NONE
}

public enum GuildRank
{
    NOVICE,
    APPRENTICE,
    JOURNEYMAN,
    VICEMASTER,
    MASTER,
    NONE
}
