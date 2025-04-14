using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Serve Customer")]
public class ServeCustomerTask : Task
{
    public override void StartTask()
    {
        SpawnNextCustomer();
    }

    public override void CompleteTask()
    {
        GameManager.Instance.currentTaskIndex++;
    }

    public void SpawnNextCustomer()
    {
        if (GameManager.Instance.customerIndex < GameManager.Instance.currentCustomers.Count)
        {
            CustomerLogic customer = Instantiate(GameManager.Instance.currentCustomers[GameManager.Instance.customerIndex], GameManager.Instance.customerSpawn.position, GameManager.Instance.customerSpawn.rotation);
            GameManager.Instance.currentCustomer = customer;
            GameManager.Instance.customerIndex++;
            GameManager.Instance.PlayEnterAudio();
        }
        else
        {
            Debug.Log("All customers served today!");
        }
    }
}
