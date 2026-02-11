using Base.Manager.Test;
using Contents.Mech;
using UnityEngine;
using Contents.Player;

public class TestScript : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private MechHealth health;
    [SerializeField] private PoolID id;
    [SerializeField] private GameObject obj;
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
            health.Hit(10);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            MonsterSpawner.Instance.Spawn();
        }
    }
}
