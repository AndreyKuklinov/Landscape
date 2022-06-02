// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BitBenderGames {

  [RequireComponent(typeof(MobileTouchCamera))]
  public class MobilePickingController : MonoBehaviour {

    public enum SelectionAction {
      Select,
      Deselect,
    }

    #region inspector
    [SerializeField]
    [Tooltip("When set to true, the position of dragged items snaps to discrete units.")]
    private bool snapToGrid = true;
    [SerializeField]
    [Tooltip("Size of the snap units when snapToGrid is enabled.")]
    private float snapUnitSize = 1;
    [SerializeField]
    [Tooltip("When snapping is enabled, this value defines a position offset that is added to the center of the object when dragging. When a top-down camera is used, these 2 values are applied to the X/Z position.")]
    private Vector2 snapOffset = Vector2.zero;
    [SerializeField]
    [Tooltip("When set to Straight, picked items will be snapped to a perfectly horizontal and vertical grid in world space. Diagonal snaps the items on a 45 degree grid.")]
    private SnapAngle snapAngle = SnapAngle.Straight_0_Degrees;
    [Header("Advanced")]
    [SerializeField]
    [Tooltip("When this flag is enabled, more than one item can be selected and moved at the same time.")]
    private bool isMultiSelectionEnabled = false;
    [SerializeField]
    [Tooltip("When setting this variable to true, pickables can only be moved by long tapping on them first.")]
    private bool requireLongTapForMove = false;
    [Header("Event Callbacks")]
#pragma warning disable 649
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is selected.")]
    private UnityEventWithTransform OnPickableTransformSelected;
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is selected through a long tap.")]
    private UnityEventWithPickableSelected OnPickableTransformSelectedExtended;
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is deselected.")]
    private UnityEventWithTransform OnPickableTransformDeselected;
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when the moving of a pickable transform is started.")]
    private UnityEventWithTransform OnPickableTransformMoveStarted;
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is moved to a new position.")]
    private UnityEventWithTransform OnPickableTransformMoved;
    [SerializeField]
    [Tooltip("Here you can set up callbacks to be invoked when the moving of a pickable transform is ended. The event requires 2 parameters. The first is the start position of the drag. The second is the dragged transform. The start position can be used to reset the transform in case the drag has ended on an invalid position.")]
    private UnityEventWithPositionAndTransform OnPickableTransformMoveEnded;
#pragma warning restore 649
    #endregion

    #region expert mode tweakables
    [Header("Expert Mode")]
    [SerializeField]
    private bool expertModeEnabled;
    [SerializeField]
    [Tooltip("When setting this to false, pickables will not become deselected when the user clicks somewhere on the screen, except when he clicks on another pickable.")]
    private bool deselectPreviousColliderOnClick = true;
    [SerializeField]
    [Tooltip("When setting this to false, the OnPickableTransformSelect event will only be sent once when clicking on the same pickable repeatedly.")]
    private bool repeatEventSelectedOnClick = true;
    [SerializeField]
    [Tooltip("Previous versions of this asset may have fired the OnPickableTransformMoveStarted too early, when it hasn't actually been moved.")]
    private bool useLegacyTransformMovedEventOrder = false;
    #endregion

    private TouchInputController touchInputController;

    private MobileTouchCamera mobileTouchCam;

    private Component SelectedCollider {
      get {
        if(SelectedColliders.Count == 0) {
          return null;
        }
        return SelectedColliders[SelectedColliders.Count - 1];
      }
    }

    public List<Component> SelectedColliders { get; private set; }

    private bool isSelectedViaLongTap = false;

    public MobileTouchPickable CurrentlyDraggedPickable { get; private set; }

    private Transform CurrentlyDraggedTransform {
      get {
        if (CurrentlyDraggedPickable != null) {
          return (CurrentlyDraggedPickable.PickableTransform);
        } else {
          return null;
        }
      }
    }

    private Vector3 draggedTransformOffset = Vector3.zero;

    private Vector3 draggedTransformHeightOffset = Vector3.zero;

    private Vector3 draggedItemCustomOffset = Vector3.zero;

    public bool SnapToGrid {
      get { return snapToGrid; }
      set { snapToGrid = value; }
    }

    public SnapAngle SnapAngle {
      get { return (snapAngle); }
      set { snapAngle = value; }
    }

    public float SnapUnitSize {
      get { return(snapUnitSize); }
      set { snapUnitSize = value; }
    }

    public Vector2 SnapOffset {
      get { return (snapOffset); }
      set { snapOffset = value; }
    }

    public const float snapAngleDiagonal = 45 * Mathf.Deg2Rad;

    private Vector3 currentlyDraggedTransformPosition = Vector3.zero;

    private const float transformMovedDistanceThreshold = 0.001f;

    private Vector3 currentDragStartPos = Vector3.zero;

    private bool invokeMoveStartedOnDrag = false;
    private bool invokeMoveEndedOnDrag = false;

    private Vector3 itemInitialDragOffsetWorld;
    private bool isManualSelectionRequest;

    public bool IsMultiSelectionEnabled {
      get {
        return isMultiSelectionEnabled;
      }
      set {
        isMultiSelectionEnabled = value;
        if(value == false) {
          DeselectAll();
        }
      }
    }

    private Dictionary<Component, Vector3> selectionPositionOffsets = new Dictionary<Component, Vector3>();

    public void Awake() {
      SelectedColliders = new List<Component>();
      mobileTouchCam = FindObjectOfType<MobileTouchCamera>();
      if (mobileTouchCam == null) {
        Debug.LogError("No MobileTouchCamera found in scene. This script will not work without this.");
      }
      touchInputController = mobileTouchCam.GetComponent<TouchInputController>();
      if (touchInputController == null) {
        Debug.LogError("No TouchInputController found in scene. Make sure this component exists and is attached to the MobileTouchCamera gameObject.");
      }
    }

    public void Start() {
      touchInputController.OnInputClick += InputControllerOnInputClick;
      touchInputController.OnFingerDown += InputControllerOnFingerDown;
      touchInputController.OnFingerUp += InputControllerOnFingerUp;
      touchInputController.OnDragStart += InputControllerOnDragStart;
      touchInputController.OnDragUpdate += InputControllerOnDragUpdate;
      touchInputController.OnDragStop += InputControllerOnDragStop;
    }

    public void OnDestroy() {
      touchInputController.OnInputClick -= InputControllerOnInputClick;
      touchInputController.OnFingerDown -= InputControllerOnFingerDown;
      touchInputController.OnFingerUp -= InputControllerOnFingerUp;
      touchInputController.OnDragStart -= InputControllerOnDragStart;
      touchInputController.OnDragUpdate -= InputControllerOnDragUpdate;
      touchInputController.OnDragStop -= InputControllerOnDragStop;
    }

    public void LateUpdate() {
      if(isManualSelectionRequest == true && TouchWrapper.TouchCount == 0) {
        isManualSelectionRequest = false;
      }
    }

    #region public interface
    /// <summary>
    /// Method that allows to set the currently selected collider for the picking controller by code.
    /// Useful for example for auto-selecting newly spawned items or for selecting items via a menu button.
    /// Use this method when you want to select just one item.
    /// </summary>
    public void SelectCollider(Component collider) {
      if (IsMultiSelectionEnabled == true) {
        Select(collider, false, false);
      } else {
        SelectColliderInternal(collider, false, false);
        isManualSelectionRequest = true;
      }
    }

    /// <summary>
    /// Method to deselect the last selected collider.
    /// </summary>
    public void DeselectSelectedCollider() {
      Deselect(SelectedCollider);
    }

    /// <summary>
    /// In case multi-selection is enabled, this method allows to deselect
    /// all colliders at once.
    /// </summary>
    public void DeselectAllSelectedColliders() {
      var collidersToRemove = new List<Component>(SelectedColliders);
      foreach (var colliderToRemove in collidersToRemove) {
        Deselect(colliderToRemove);
      }
    }

    /// <summary>
    /// Method to deselect the given collider.
    /// In case the collider hasn't been selected before, the method returns false.
    /// </summary>
    private bool Deselect(Component colliderComponent) {
      bool wasRemoved = SelectedColliders.Remove(colliderComponent);
      if (wasRemoved == true) {
        OnSelectedColliderChanged(SelectionAction.Deselect, colliderComponent.GetComponent<MobileTouchPickable>());
      }
      return wasRemoved;
    }

    /// <summary>
    /// Method to deselect all currently selected colliders.
    /// </summary>
    /// <returns></returns>
    public int DeselectAll() {
      SelectedColliders.RemoveAll(item => item == null);
      int colliderCount = SelectedColliders.Count;
      foreach (Component colliderComponent in SelectedColliders) {
        OnSelectedColliderChanged(SelectionAction.Deselect, colliderComponent.GetComponent<MobileTouchPickable>());
      }
      SelectedColliders.Clear();
      return colliderCount;
    }

    public Component GetClosestColliderAtScreenPoint(Vector3 screenPoint, out Vector3 intersectionPoint) {

      Component hitCollider = null;
      float hitDistance = float.MaxValue;
      Ray camRay = mobileTouchCam.Cam.ScreenPointToRay(screenPoint);
      RaycastHit hitInfo;
      intersectionPoint = Vector3.zero;
      if (Physics.Raycast(camRay, out hitInfo) == true) {
        hitDistance = hitInfo.distance;
        hitCollider = hitInfo.collider;
        intersectionPoint = hitInfo.point;
      }
      RaycastHit2D hitInfo2D = Physics2D.Raycast(camRay.origin, camRay.direction);
      if (hitInfo2D == true) {
        if (hitInfo2D.distance < hitDistance) {
          hitCollider = hitInfo2D.collider;
          intersectionPoint = hitInfo2D.point;
        }
      }
      return (hitCollider);
    }

    public void RequestDragPickable(Component colliderComponent) {
      if (TouchWrapper.TouchCount == 1) {
        SelectColliderInternal(colliderComponent, false, false);
        isManualSelectionRequest = true;
        Vector3 fingerDownPos = TouchWrapper.Touch0.Position;
        Vector3 intersectionPoint;
        Ray dragRay = mobileTouchCam.Cam.ScreenPointToRay(fingerDownPos);
        bool hitSuccess = mobileTouchCam.RaycastGround(dragRay, out intersectionPoint);
        if (hitSuccess == false) {
          intersectionPoint = colliderComponent.transform.position;
        }
        if (requireLongTapForMove == true) {
          isSelectedViaLongTap = true; //This line ensures that dragging via scrip also works when 'requireLongTapForDrag' is set to true.
        }
        RequestDragPickable(colliderComponent, fingerDownPos, intersectionPoint);
        invokeMoveEndedOnDrag = true;
      } else {
        Debug.LogError("A drag request can only be invoked when the user has placed exactly 1 finger on the screen.");
      }
    }

    public Vector3 GetFinger0PosWorld() {
      return mobileTouchCam.GetFinger0PosWorld();
    }
    #endregion

    private void SelectColliderInternal( Component colliderComponent, bool isDoubleClick, bool isLongTap ) {

      if(deselectPreviousColliderOnClick == false) {
        if(colliderComponent == null || colliderComponent.GetComponent<MobileTouchPickable>() == null) {
          return; //Skip selection change in case the user requested to deselect only in case another pickable is clicked.
        }
      }

      if(isManualSelectionRequest == true) {
        return; //Skip selection when the user has already requested a manual selection with the same click.
      }

      Component previouslySelectedCollider = SelectedCollider;
      bool skipSelect = false;

      if (isMultiSelectionEnabled == false) {
        if (previouslySelectedCollider != null && previouslySelectedCollider != colliderComponent) {
          Deselect(previouslySelectedCollider);
        }
      } else {
        skipSelect = Deselect(colliderComponent);
      }

      if (skipSelect == false) {
        if (colliderComponent != null) {
          if (colliderComponent != previouslySelectedCollider || repeatEventSelectedOnClick == true) {
            Select(colliderComponent, isDoubleClick, isLongTap);
            isSelectedViaLongTap = isLongTap;
          }
        }
      }
    }

    private void InputControllerOnInputClick(Vector3 clickPosition, bool isDoubleClick, bool isLongTap) {
      Vector3 intersectionPoint;
      var newCollider = GetClosestColliderAtScreenPoint(clickPosition, out intersectionPoint);
      SelectColliderInternal(newCollider, isDoubleClick, isLongTap);
    }

    private void RequestDragPickable(Vector3 fingerDownPos) {
      Vector3 intersectionPoint = Vector3.zero;
      Component pickedCollider = GetClosestColliderAtScreenPoint(fingerDownPos, out intersectionPoint);
      if(pickedCollider != null && SelectedColliders.Contains(pickedCollider)) {
        RequestDragPickable(pickedCollider, fingerDownPos, intersectionPoint);
      }
    }

    private void RequestDragPickable(Component colliderComponent, Vector2 fingerDownPos, Vector3 intersectionPoint) {

      if (requireLongTapForMove == true && isSelectedViaLongTap == false) {
        return;
      }

      CurrentlyDraggedPickable = null;
      bool isDragStartedOnSelection = colliderComponent != null && SelectedColliders.Contains(colliderComponent);
      if (isDragStartedOnSelection == true) {
        MobileTouchPickable mobileTouchPickable = colliderComponent.GetComponent<MobileTouchPickable>();
        if (mobileTouchPickable != null) {
          mobileTouchCam.OnDragSceneObject(); //Lock camera movement.
          CurrentlyDraggedPickable = mobileTouchPickable;
          currentlyDraggedTransformPosition = CurrentlyDraggedTransform.position;

          invokeMoveStartedOnDrag = true;
          currentDragStartPos = CurrentlyDraggedTransform.position;
          selectionPositionOffsets.Clear();
          foreach (Component selectionComponent in SelectedColliders) {
            selectionPositionOffsets.Add(selectionComponent, currentDragStartPos - selectionComponent.transform.position);
          }

          draggedTransformOffset = Vector3.zero;
          draggedTransformHeightOffset = Vector3.zero;
          draggedItemCustomOffset = Vector3.zero;

          //Find offset of item transform relative to ground.
          Vector3 groundPosCenter = Vector3.zero;
          Ray groundScanRayCenter = new Ray(CurrentlyDraggedTransform.position, -mobileTouchCam.RefPlane.normal);
          bool rayHitSuccess = mobileTouchCam.RaycastGround(groundScanRayCenter, out groundPosCenter);
          if(rayHitSuccess == true) {
            draggedTransformHeightOffset = CurrentlyDraggedTransform.position - groundPosCenter;
          } else {
            groundPosCenter = CurrentlyDraggedTransform.position;
          }

          draggedTransformOffset = groundPosCenter - intersectionPoint;
          itemInitialDragOffsetWorld = (ComputeDragPosition(fingerDownPos, SnapToGrid) - CurrentlyDraggedTransform.position);
        }
      }
    }

    private void InputControllerOnFingerDown(Vector3 fingerDownPos) {
      if(requireLongTapForMove == false || isSelectedViaLongTap == true) {
        RequestDragPickable(fingerDownPos);
      }
    }

    private void InputControllerOnFingerUp() {
      EndPickableTransformMove();
    }

    private Vector3 ComputeDragPosition(Vector3 dragPosCurrent, bool clampToGrid) {

      Vector3 dragPosWorld = Vector3.zero;
      Ray dragRay = mobileTouchCam.Cam.ScreenPointToRay(dragPosCurrent);

      dragRay.origin += draggedTransformOffset;
      bool hitSuccess = mobileTouchCam.RaycastGround(dragRay, out dragPosWorld);
      if(hitSuccess == false) { //This case really should never be met. But in case it is for some unknown reason, return the current item position. That way at least it will remain static and not move somewhere into nirvana.
        return CurrentlyDraggedTransform.position;
      }

      dragPosWorld += draggedTransformHeightOffset;
      dragPosWorld += draggedItemCustomOffset;

      if(clampToGrid == true) {
        dragPosWorld = ClampDragPosition(CurrentlyDraggedPickable, dragPosWorld);
      }
      return dragPosWorld;
    }

    private void InputControllerOnDragStart(Vector3 clickPosition, bool isLongTap) {

      if(isLongTap == true && touchInputController.LongTapStartsDrag == true) {
        Vector3 intersectionPoint;
        Component newCollider = GetClosestColliderAtScreenPoint(clickPosition, out intersectionPoint);
        if(newCollider != null) {
          MobileTouchPickable newPickable = newCollider.GetComponent<MobileTouchPickable>();
          if(newPickable != null) {
            if (SelectedColliders.Contains(newCollider) == false) {
              SelectColliderInternal(newCollider, false, isLongTap);
            } else {
              isSelectedViaLongTap = isLongTap;
            }
            RequestDragPickable(clickPosition);
          }
        }
      }
    }

    private void InputControllerOnDragUpdate(Vector3 dragPosStart, Vector3 dragPosCurrent, Vector3 correctionOffset) {

      if (CurrentlyDraggedTransform != null) {

        if(invokeMoveStartedOnDrag == true && useLegacyTransformMovedEventOrder == true) {
          InvokePickableMoveStart();
        }

        draggedItemCustomOffset += CurrentlyDraggedTransform.position - currentlyDraggedTransformPosition; //Accomodate for custom movements by user code that happen while an item is being dragged. E.g. this allows users to lift items slightly during a drag.

        Vector3 dragPosWorld = ComputeDragPosition(dragPosCurrent, SnapToGrid);
        CurrentlyDraggedTransform.position = dragPosWorld - itemInitialDragOffsetWorld;

        if(SelectedColliders.Count > 1) {
          foreach(KeyValuePair<Component, Vector3> colliderOffsetPair in selectionPositionOffsets) {
            if(colliderOffsetPair.Key != null && colliderOffsetPair.Key.transform != CurrentlyDraggedTransform) {
              colliderOffsetPair.Key.transform.position = CurrentlyDraggedTransform.position - colliderOffsetPair.Value;
            }
          }
        }

        bool hasMoved = false;
        if(mobileTouchCam.CameraAxes == CameraPlaneAxes.XY_2D_SIDESCROLL) {
          hasMoved = ComputeDistance2d(CurrentlyDraggedTransform.position.x, CurrentlyDraggedTransform.position.y,
                                        currentlyDraggedTransformPosition.x, currentlyDraggedTransformPosition.y) > transformMovedDistanceThreshold;
        } else {
          hasMoved = ComputeDistance2d(CurrentlyDraggedTransform.position.x, CurrentlyDraggedTransform.position.z,
                                       currentlyDraggedTransformPosition.x, currentlyDraggedTransformPosition.z) > transformMovedDistanceThreshold;
        }
        if (hasMoved == true) {
          if(invokeMoveStartedOnDrag == true && useLegacyTransformMovedEventOrder == false) {
            InvokePickableMoveStart();
          }

          InvokeTransformActionSafe(OnPickableTransformMoved, CurrentlyDraggedTransform);
        }

        currentlyDraggedTransformPosition = CurrentlyDraggedTransform.position;
      }
    }

    private void InvokePickableMoveStart() {
      InvokeTransformActionSafe(OnPickableTransformMoveStarted, CurrentlyDraggedTransform);
      invokeMoveStartedOnDrag = false;
      invokeMoveEndedOnDrag = true;
    }

    private float ComputeDistance2d(float x0, float y0, float x1, float y1) {
      return(Mathf.Sqrt((x1-x0)*(x1-x0) + (y1-y0)*(y1-y0)));
    }

    private void InputControllerOnDragStop(Vector3 dragStopPos, Vector3 dragFinalMomentum) {
      EndPickableTransformMove();
    }

    private void EndPickableTransformMove() {
      if (CurrentlyDraggedTransform != null) {
        if (OnPickableTransformMoveEnded != null) {
          if(invokeMoveEndedOnDrag == true) {
            OnPickableTransformMoveEnded.Invoke(currentDragStartPos, CurrentlyDraggedTransform);
          }
        }
      }
      CurrentlyDraggedPickable = null;
      invokeMoveStartedOnDrag = false;
      invokeMoveEndedOnDrag = false;
    }
      
    private Vector3 ClampDragPosition(MobileTouchPickable draggedPickable, Vector3 position) {

      if (mobileTouchCam.CameraAxes == CameraPlaneAxes.XY_2D_SIDESCROLL) {
        if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
          RotateVector2(ref position.x, ref position.y, -snapAngleDiagonal);
        }
        position.x = GetPositionSnapped(position.x, draggedPickable.LocalSnapOffset.x + snapOffset.x);
        position.y = GetPositionSnapped(position.y, draggedPickable.LocalSnapOffset.y + snapOffset.y);
        if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
          RotateVector2(ref position.x, ref position.y, snapAngleDiagonal);
        }
      } else {
        if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
          RotateVector2(ref position.x, ref position.z, -snapAngleDiagonal);
        }
        position.x = GetPositionSnapped(position.x, draggedPickable.LocalSnapOffset.x + snapOffset.x);
        position.z = GetPositionSnapped(position.z, draggedPickable.LocalSnapOffset.y + snapOffset.y);
        if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
          RotateVector2(ref position.x, ref position.z, snapAngleDiagonal);
        }
      }
      return (position);
    }

    private void RotateVector2(ref float x, ref float y, float degrees) {
      if (Mathf.Approximately(degrees, 0)) {
        return;
      }
      float newX = x * Mathf.Cos(degrees) - y * Mathf.Sin(degrees);
      float newY = x * Mathf.Sin(degrees) + y * Mathf.Cos(degrees);
      x = newX;
      y = newY;
    }

    private float GetPositionSnapped(float position, float snapOffset) {
      if (snapToGrid == true) {
        return (Mathf.RoundToInt(position / snapUnitSize) * snapUnitSize) + snapOffset;
      } else {
        return(position);
      }
    }
      
    private void OnSelectedColliderChanged(SelectionAction selectionAction, MobileTouchPickable mobileTouchPickable) {
      if (mobileTouchPickable != null) {
        if (selectionAction == SelectionAction.Select) {
          InvokeTransformActionSafe(OnPickableTransformSelected, mobileTouchPickable.PickableTransform);
        } else if (selectionAction == SelectionAction.Deselect) {
          InvokeTransformActionSafe(OnPickableTransformDeselected, mobileTouchPickable.PickableTransform);
        }
      }
    }

    private void OnSelectedColliderChangedExtended(SelectionAction selectionAction, MobileTouchPickable mobileTouchPickable, bool isDoubleClick, bool isLongTap) {
      if (mobileTouchPickable != null) {
        if (selectionAction == SelectionAction.Select) {
          PickableSelectedData pickableSelectedData = new PickableSelectedData() {
            SelectedTransform = mobileTouchPickable.PickableTransform,
            IsDoubleClick = isDoubleClick,
            IsLongTap = isLongTap };
          InvokeGenericActionSafe(OnPickableTransformSelectedExtended, pickableSelectedData);
        }
      }
    }

    private void InvokeTransformActionSafe(UnityEventWithTransform eventAction, Transform selectionTransform) {
      if (eventAction != null) {
        eventAction.Invoke(selectionTransform);
      }
    }

    private void InvokeGenericActionSafe<T1, T2>(T1 eventAction, T2 eventArgs) where T1 : UnityEvent<T2> {
      if (eventAction != null) {
        eventAction.Invoke(eventArgs);
      }
    }

    private void Select(Component colliderComponent, bool isDoubleClick, bool isLongTap) {
      MobileTouchPickable mobileTouchPickable = colliderComponent.GetComponent<MobileTouchPickable>();
      if (mobileTouchPickable != null) {
        if (SelectedColliders.Contains(colliderComponent) == false) {
          SelectedColliders.Add(colliderComponent);
        }
      }
      OnSelectedColliderChanged(SelectionAction.Select, mobileTouchPickable);
      OnSelectedColliderChangedExtended(SelectionAction.Select, mobileTouchPickable, isDoubleClick, isLongTap);
    }
  }
}
