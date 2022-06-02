using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BitBenderGames {

  /// <summary>
  /// Helper class that will block Mobile Touch Camera Input while the pointer is over the
  /// UI element that this script is added to. You can add it for example to a Unity UI Button.
  /// 
  /// Usage: Add to a Unity UI object that you want to block the touch camera from moving when clicked.
  /// </summary>
  public class DisableInputOnPointerDown : MonoBehaviour, IPointerDownHandler {

    [SerializeField]
    private TouchInputController touchInputController;

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
          Debug.LogError("Failed to find TouchInputController. Make sure the reference is assigned via inspector or by ensuring the Find method works.", gameObject);
          this.enabled = false;
        }
      }
    }

    public void OnPointerDown(PointerEventData eventData) {
      if(touchInputController != null) {
        touchInputController.OnEventTriggerPointerDown(eventData);
      }
    }
  }
}
