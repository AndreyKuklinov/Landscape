// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace BitBenderGames {

  /// <summary>
  /// A little helper script that allows to focus the camera on a transform either
  /// via code, or by wiring it up with one of the many events of the mobile touch camera
  /// or mobile picking controller.
  /// </summary>
  [RequireComponent(typeof(MobileTouchCamera))]
  public class FocusCameraOnItem : MonoBehaviourWrapped {

    [SerializeField]
    private float transitionDuration = 0.5f;

    [SerializeField]
    private AnimationCurve transitionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    private MobileTouchCamera MobileTouchCamera { get; set; }

    private Vector3 posTransitionStart;
    private Vector3 posTransitionEnd;
    private Quaternion rotTransitionStart;
    private Quaternion rotTransitionEnd;
    private float zoomTransitionStart;
    private float zoomTransitionEnd;
    private float timeTransitionStart;
    private bool isTransitionStarted;

    public float TransitionDuration {
      get { return transitionDuration; }
      set { transitionDuration = value; }
    }

    public void Awake() {
      MobileTouchCamera = GetComponent<MobileTouchCamera>();
      isTransitionStarted = false;
    }

    public void LateUpdate() {

      if (MobileTouchCamera.IsDragging || MobileTouchCamera.IsPinching) {
        timeTransitionStart = Time.time - transitionDuration;
      }

      if (isTransitionStarted == true) {
        if (Time.time < timeTransitionStart + transitionDuration) {
          UpdateTransform();
        } else {
          SetTransform(posTransitionEnd, rotTransitionEnd, zoomTransitionEnd);
          isTransitionStarted = false;
        }
      }
    }

    private void UpdateTransform() {
      float progress = (Time.time - timeTransitionStart) / transitionDuration;
      float t = transitionCurve.Evaluate(progress);
      Vector3 positionNew = Vector3.Lerp(posTransitionStart, posTransitionEnd, t);
      Quaternion rotationNew;
      rotationNew = Quaternion.Lerp(rotTransitionStart, rotTransitionEnd, t);
      float zoomNew = Mathf.Lerp(zoomTransitionStart, zoomTransitionEnd, t);
      SetTransform(positionNew, rotationNew, zoomNew);
    }

    public void OnPickItem(RaycastHit hitInfo) {
      FocusCameraOnTransform(hitInfo.transform);
    }

    public void OnPickItem2D(RaycastHit2D hitInfo2D) {
      FocusCameraOnTransform(hitInfo2D.transform);
    }

    public void OnPickableTransformSelected(Transform pickableTransform) {
      FocusCameraOnTransform(pickableTransform);
    }

    public void FocusCameraOnTransform(Transform targetTransform) {
      if (targetTransform == null) {
        return;
      }
      FocusCameraOnTarget(targetTransform.position);
    }

    public void FocusCameraOnTransform(Vector3 targetPosition) {
      FocusCameraOnTarget(targetPosition);
    }

    public void FocusCameraOnTarget(Vector3 targetPosition) {
      FocusCameraOnTarget(targetPosition, Transform.rotation, MobileTouchCamera.CamZoom);
    }

    private float GetTiltFromRotation(Quaternion camRotation) {
      Vector3 camForwardDir = camRotation * Vector3.forward;
      Vector3 camUp = MobileTouchCamera.CameraAxes == CameraPlaneAxes.XZ_TOP_DOWN ? Vector3.up : Vector3.back;
      Vector3 camRightEnd = Vector3.Cross(camUp, camForwardDir);
      Vector3 camForwardOnPlaneEnd = Vector3.Cross(camRightEnd, camUp);
      float camTilt = Vector3.Angle(camForwardOnPlaneEnd, camForwardDir);
      return camTilt;
    }

    public void FocusCameraOnTarget(Vector3 targetPosition, Quaternion targetRotation, float targetZoom) {

      timeTransitionStart = Time.time;
      posTransitionStart = Transform.position;
      rotTransitionStart = Transform.rotation;
      zoomTransitionStart = MobileTouchCamera.CamZoom;
      rotTransitionEnd = targetRotation;
      zoomTransitionEnd = targetZoom;

      MobileTouchCamera.Transform.rotation = targetRotation;
      MobileTouchCamera.CamZoom = targetZoom;
      Vector3 intersectionScreenCenterEnd = MobileTouchCamera.GetIntersectionPoint(MobileTouchCamera.Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)));
      Vector3 focusVector = targetPosition - intersectionScreenCenterEnd;
      posTransitionEnd = MobileTouchCamera.GetClampToBoundaries(posTransitionStart + focusVector, true);
      MobileTouchCamera.Transform.rotation = rotTransitionStart;
      MobileTouchCamera.CamZoom = zoomTransitionStart;

      if (Mathf.Approximately(transitionDuration, 0)) {
        SetTransform(posTransitionEnd, targetRotation, targetZoom);
        return;
      }

      isTransitionStarted = true;
    }

    private void SetTransform(Vector3 newPosition, Quaternion newRotation, float newZoom) {
      Transform.position = newPosition;
      Transform.rotation = newRotation;
      MobileTouchCamera.CamZoom = newZoom;
    }
  }
}
