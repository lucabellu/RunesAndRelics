using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Clean")]
public class CleanTask : Task
{
    public override void StartTask()
    {
        GameManager.Instance.canTalkWithBoss = true;
        GameManager.Instance.canCleanCobwebs = true;
    }

    public override void CompleteTask()
    {
        GameManager.Instance.currentTaskIndex++;
        GameManager.Instance.bossDoor.transform.GetComponent<Dialogue>().firstBossDialogue = false;
        GameManager.Instance.bossDoor.UnhighlightDoor();
    }

}
