using Base.Manager.Test;
using UnityEngine;
using Contents.Player;

public class TestScript : MonoBehaviour
{
    [SerializeField] private PlayerController player;
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
            player.Hit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            obj = ObjectPoolGenericManager.poolDic[id].UsePool(transform.position, Quaternion.identity);
            
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ObjectPoolGenericManager.poolDic[id].ReturnPool(obj);
        }
    }
}
