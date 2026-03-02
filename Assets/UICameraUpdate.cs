using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UICameraUpdate : MonoBehaviour
{
    private Camera _baseCamera;
    private UniversalAdditionalCameraData _baseData;
    private Camera _uiCamera;

    private void Awake()
    {
        _baseCamera = GetComponent<Camera>();
        _baseData = _baseCamera.GetUniversalAdditionalCameraData();
    }

    private void OnEnable()
    {
        BindUICamera();
    }

    private void OnDisable()
    {
        UnbindUICamera();
    }

    private void BindUICamera()
    {
        if (_baseData == null) return;

        GameObject uiCamObj = GameObject.FindGameObjectWithTag("UICamera");
        if (uiCamObj == null)
        {
            Debug.LogWarning("UICamera 태그를 가진 카메라를 찾지 못했습니다.");
            return;
        }

        _uiCamera = uiCamObj.GetComponent<Camera>();
        if (_uiCamera == null)
        {
            Debug.LogWarning("UICamera 오브젝트에 Camera 컴포넌트가 없습니다.");
            return;
        }

        var uiData = _uiCamera.GetUniversalAdditionalCameraData();
        if (uiData == null)
        {
            Debug.LogWarning("UICamera에 UniversalAdditionalCameraData가 없습니다.");
            return;
        }

        // 안전하게 Overlay로 설정
        uiData.renderType = CameraRenderType.Overlay;

        if (!_baseData.cameraStack.Contains(_uiCamera))
        {
            _baseData.cameraStack.Add(_uiCamera);
        }
    }

    private void UnbindUICamera()
    {
        if (_baseData == null || _uiCamera == null) return;

        if (_baseData.cameraStack.Contains(_uiCamera))
        {
            _baseData.cameraStack.Remove(_uiCamera);
        }
    }
}
