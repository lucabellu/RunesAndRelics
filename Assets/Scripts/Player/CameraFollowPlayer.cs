using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform camPos;

    private void LateUpdate()
    {
        transform.position = camPos.position;
    }
}
