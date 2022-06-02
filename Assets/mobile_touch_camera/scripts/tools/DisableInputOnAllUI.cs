using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BitBenderGames {

  /// <summary>
  /// Helper class that will block Mobile Touch Camera Input while the pointer is over any
  /// UI element that has the RaycastTarget checkbox activated. Using this script may yield to
  /// too many false blockings. It's more precise to mark each necessary UI element as described
  /// in the documentation for the asset.
  /// 
  /// Usage: Add this to just one active GameObject in your scene. For example to your main camera
  ///        or to a stand-alone GameObject that's always activated.
  /// </summary>
  public class DisableInputOnAllUI : MonoBehaviour {

    [SerializeField]
    private TouchInputController touchInputController;

    private bool wasTouchingLastFrame;

    ///////////////////////////////////////////////////////////////////////////
    // Methods
    ///////////////////////////////////////////////////////////////////////////

    public void Reset() {
      touchInputController = FindObjectOfType<TouchInputController>();
    }

    public void Awake() {
      if (touchInputController == null) {
        touchInputController = FindObjectOfType<TouchInputController>();
        if (touchInputController == null) {
          Debug.LogError("Failed to find TouchInputController. Make sure the reference is assigned via inspector or by ensuring the Find method works.");
          this.enabled = false;
        }
      }
    }

    public void Update() {

      if (IsPointerOverGameObject()) {
        if (TouchWrapper.IsFingerDown == true && wasTouchingLastFrame == false) {
          touchInputController.IsInputOnLockedArea = true;
        }
      }

      wasTouchingLastFrame = TouchWrapper.IsFingerDown;
    }

    private bool IsPointerOverGameObject() {

      if (EventSystem.current == null) {
        return false;
      }

      // Check mouse
        if (EventSystem.current.IsPointerOverGameObject()) {
        return true;
      }

      // Check touches
      for (int i = 0; i < Input.touchCount; i++) {
        var touch = Input.GetTouch(i);
        if (touch.phase == TouchPhase.Began) {
          if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
            return true;
          }
        }
      }

      return false;
    }
  }
}
