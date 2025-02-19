using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform player;
    private float rotX;
    private float rotY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        rotY += mouseX;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
        player.rotation = Quaternion.Euler(0, rotY, 0);
    }
}
