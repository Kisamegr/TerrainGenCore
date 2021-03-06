﻿using UnityEngine;

public class WaterRenderer : MonoBehaviour {

  public Material waterMaterial;

  private Camera mainCamera;
  private Camera reflectionCamera;
  private Camera refractionCamera;

  private RenderTexture reflectionTexture;
  private RenderTexture refractionTexture;

  private bool initialized = false;
  private float waterLevelY;

  Vector3 clipPlanePos = Vector3.zero;
  Vector3 clipPlaneNormal = Vector3.up;

  private void OnRenderObject() {
    if (initialized)
      UpdateCameras();
  }

  public void CreateCameras(int resolution, float waterLevelY) {
    mainCamera = Camera.main;
    this.waterLevelY = waterLevelY;
    clipPlanePos = new Vector3(0, waterLevelY, 0);

    // Calculate the texture's height based on the resolution and the aspect ratio
    int textureWidth  = resolution;
    int textureHeight = Mathf.FloorToInt(resolution / mainCamera.aspect);

    // Create the textures

    reflectionTexture = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGB32);
    refractionTexture = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGB32);


    // Reflection camera
    GameObject reflectionCameraObj = new GameObject("Reflection Camera");
    reflectionCamera = reflectionCameraObj.AddComponent<Camera>();
    reflectionCamera.CopyFrom(mainCamera);
    reflectionCamera.enabled = false;
    reflectionCamera.cullingMask = reflectionCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Water"));
    reflectionCamera.targetTexture = reflectionTexture;

    // Refraction Camera
    GameObject refractionCameraObj = new GameObject("Refraction Camera");
    refractionCamera = refractionCameraObj.AddComponent<Camera>();
    refractionCamera.CopyFrom(mainCamera);
    refractionCamera.enabled = false;
    refractionCamera.cullingMask = reflectionCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Water"));
    refractionCamera.targetTexture = refractionTexture;

    initialized = true;
  }

  void UpdateCameras() {
    // Get the main camera's position and rotation
    Vector3 cameraPosition   = mainCamera.transform.position;
    Quaternion cameraRoation = mainCamera.transform.rotation;

    // Calculate the main camera to water Y distance, and cache the camera's euler angles
    float camToWaterDistance  = Mathf.Abs(cameraPosition.y) - waterLevelY;
    Vector3 cameraEulerAngles = mainCamera.transform.rotation.eulerAngles;

    // Set the reflection camera position under the water, at the same distance Y as the main camera
    reflectionCamera.transform.position = new Vector3(cameraPosition.x,
      cameraPosition.y - 2 * camToWaterDistance,
      cameraPosition.z);

    // Reflect main camera's rotation in the X axis and set it as the reflection camera's rotation
    cameraEulerAngles.x *= -1;
    Quaternion rotation = new Quaternion {
      eulerAngles = cameraEulerAngles
    };
    reflectionCamera.transform.rotation = rotation;

    // Set the refraction camera's position and rotation to match the main camera
    refractionCamera.transform.position = cameraPosition;
    refractionCamera.transform.rotation = cameraRoation;

    // Set up the reflection projection
    Vector4 reflectionClipPlane = CameraSpacePlane(reflectionCamera, clipPlanePos, clipPlaneNormal, 1.0f);
    reflectionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(reflectionClipPlane);

    // Set up the refraction projection
    Vector4 refractionClipPlane = CameraSpacePlane(refractionCamera, clipPlanePos, clipPlaneNormal, -1.0f);
    refractionCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(refractionClipPlane);

    // Render the cameras
    reflectionCamera.Render();
    refractionCamera.Render();

    // Update the shader textures
    waterMaterial.SetTexture("_ReflectionTexture", reflectionTexture);
    waterMaterial.SetTexture("_RefractionTexture", refractionTexture);
  }

  /* The above static functions were taken from the original Unity Water script */

  // Given position/normal of the plane, calculates plane in camera space.
  static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign) {
    Vector3 offsetPos = pos + normal * 0.07f;
    Matrix4x4 m = cam.worldToCameraMatrix;
    Vector3 cpos = m.MultiplyPoint(offsetPos);
    Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
    return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
  }

  // Calculates reflection matrix around the given plane
  static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane) {
    reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
    reflectionMat.m01 = (-2F * plane[0] * plane[1]);
    reflectionMat.m02 = (-2F * plane[0] * plane[2]);
    reflectionMat.m03 = (-2F * plane[3] * plane[0]);

    reflectionMat.m10 = (-2F * plane[1] * plane[0]);
    reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
    reflectionMat.m12 = (-2F * plane[1] * plane[2]);
    reflectionMat.m13 = (-2F * plane[3] * plane[1]);

    reflectionMat.m20 = (-2F * plane[2] * plane[0]);
    reflectionMat.m21 = (-2F * plane[2] * plane[1]);
    reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
    reflectionMat.m23 = (-2F * plane[3] * plane[2]);

    reflectionMat.m30 = 0F;
    reflectionMat.m31 = 0F;
    reflectionMat.m32 = 0F;
    reflectionMat.m33 = 1F;
  }

}
