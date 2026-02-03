using UnityEngine;
using Contents.Player;

public class TestScript : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Hit();
        }
    }
}
