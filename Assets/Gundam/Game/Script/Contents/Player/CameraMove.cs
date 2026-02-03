using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform PlayerBody;
    [SerializeField] private float mouseSpeed = 5f;
    private float mouseX = 0;
    private float mouseY = 0;

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
        mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        yaw += mouseX;       // 좌우(플레이어)
        pitch -= mouseY;       // 위아래(카메라)

        // 위아래 제한
        pitch = Mathf.Clamp(pitch, -70f, 30f);

        // 실제 회전 적용
        PlayerBody.rotation = Quaternion.Euler(0f, yaw, 0f);     // 좌우이동은 Player 몸통을 옮겨 종속된 카메라가 따라감
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);   // 상하이동은 카메라만 옮긴다

    }
}
