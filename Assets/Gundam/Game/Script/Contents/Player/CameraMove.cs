using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform PlayerBody;
    [SerializeField] private float mouseSpeed = 5f;
    [SerializeField] private List<Transform> Weapons; //상체
    private float _mouseX = 0;
    private float _mouseY = 0;
    float yaw;
    float pitch;

    // Start is called before the first frame update
    void Start()
    {
        yaw = PlayerBody.rotation.eulerAngles.y;

        // 카메라의 현재 X각도(상하)
        float x = transform.localEulerAngles.x;
        // 0~360 → -180~180 으로 보정
        if (x > 180f) x -= 360f;
        pitch = x;
    }

    // Update is called once per frame
    void Update()
    {
        _mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        _mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        yaw += _mouseX;       // 좌우(플레이어)
        pitch -= _mouseY;       // 위아래(카메라)

        // 위아래 제한
        pitch = Mathf.Clamp(pitch, -30f, 30f);

        // 실제 회전 적용
        PlayerBody.rotation = Quaternion.Euler(0f, yaw, 0f);     // 좌우이동은 Player 몸통을 옮겨 종속된 카메라가 따라감
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);   // 상하이동은 카메라만
        //추가로 무기들도 상하이동 같이 함(카메라 시야와 발사위치 오차 줄이기)
        foreach (Transform weapon in Weapons)
        {
            weapon.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}
