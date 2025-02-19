using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform camPos;

    private void Update()
    {
        transform.position = camPos.position;
    }
}
