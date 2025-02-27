using UnityEngine;

[CreateAssetMenu(fileName = "TrinketDataSO", menuName = "Scriptable Objects/TrinketDataSO")]
public class TrinketDataSO : ScriptableObject
{
    public Race requiredRace;
    public Kingdom requiredKingdom;
    public Occupation requiredOccupation;
    public int requiredAge;
    public Guild requiredGuild;
    public GuildRank requiredGuildRank;
}
