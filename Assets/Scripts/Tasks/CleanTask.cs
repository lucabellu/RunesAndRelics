using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Clean")]
public class CleanTask : Task
{
    public override void StartTask()
    {
        GameManager.Instance.canCleanCobwebs = true;
        Debug.Log("Cleaning task started");
    }

    public override void CompleteTask()
    {
        GameManager.Instance.currentTaskIndex++;
    }

}
