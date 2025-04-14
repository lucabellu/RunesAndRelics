using UnityEngine;

public abstract class Task : ScriptableObject
{
    public abstract void StartTask();
    public abstract void CompleteTask();
}
