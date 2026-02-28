using UnityEditor;
using UnityEngine;

namespace tarkin.Director.EditorTools
{
    [CustomEditor(typeof(Tripwire))]
    public class TripwireEditor : Editor
    {
        private bool _draggingFrom;
        private bool _draggingTo;
        private int _previousHotControl;

        private void OnEnable()
        {
            Tools.hidden = true;
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        private void OnSceneGUI()
        {
            Tripwire tripwire = (Tripwire)target;

            Event e = Event.current;
            bool shiftHeld = e.shift;

            // Suppress shift modifier so PositionHandle doesn't enter screen-space mode
            if (shiftHeld)
                e.modifiers &= ~EventModifiers.Shift;

            Vector3 prevFrom = tripwire.transform.position;
            Vector3 prevTo = tripwire.PosTo;

            // Position handle for PosFrom (transform.position)
            EditorGUI.BeginChangeCheck();
            Vector3 newFrom = Handles.PositionHandle(tripwire.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tripwire.transform, "Move Tripwire From");
                tripwire.transform.position = newFrom;
                _draggingFrom = true;

                if (shiftHeld)
                {
                    Vector3 delta = newFrom - prevFrom;
                    Undo.RecordObject(tripwire, "Move Tripwire To (Shift)");
                    tripwire.PosTo += delta;
                    _draggingTo = true;
                }
            }

            // Position handle for PosTo
            EditorGUI.BeginChangeCheck();
            Vector3 newTo = Handles.PositionHandle(tripwire.PosTo, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tripwire, "Move Tripwire To");
                tripwire.PosTo = newTo;
                _draggingTo = true;

                if (shiftHeld)
                {
                    Vector3 delta = newTo - prevTo;
                    Undo.RecordObject(tripwire.transform, "Move Tripwire From (Shift)");
                    tripwire.transform.position += delta;
                    _draggingFrom = true;
                }
            }

            // Restore shift modifier
            if (shiftHeld)
                e.modifiers |= EventModifiers.Shift;

            // Detect handle release: hotControl transitions from non-zero to zero
            if (_previousHotControl != 0 && GUIUtility.hotControl == 0)
            {
                if (_draggingFrom)
                {
                    Undo.RecordObject(tripwire.transform, "Snap Tripwire From");
                    tripwire.transform.position = RaycastToGround(tripwire.transform.position);
                    _draggingFrom = false;
                    EditorUtility.SetDirty(tripwire);
                }

                if (_draggingTo)
                {
                    Undo.RecordObject(tripwire, "Snap Tripwire To");
                    tripwire.PosTo = RaycastToGround(tripwire.PosTo);
                    _draggingTo = false;
                    EditorUtility.SetDirty(tripwire);
                }
            }

            _previousHotControl = GUIUtility.hotControl;

            if (_draggingFrom)
            {
                DrawRaycastPreview(tripwire.transform.position);
            }

            if (_draggingTo)
            {
                DrawRaycastPreview(tripwire.PosTo);
            }

            Handles.Label(tripwire.transform.position + Vector3.up * 0.35f, "From");
            Handles.Label(tripwire.PosTo + Vector3.up * 0.35f, "To");
        }

        private void DrawRaycastPreview(Vector3 position)
        {
            float height = 0.2f;
            Vector3 rayOrigin = position + new Vector3(0, height, 0);
            float rayLength = 2f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength, 1 << 12))
            {
                Handles.color = Color.green;
                Handles.DrawDottedLine(rayOrigin, hit.point, 3f);

                float crossSize = 0.08f;
                Handles.DrawLine(hit.point - new Vector3(crossSize, 0, 0), hit.point + new Vector3(crossSize, 0, 0));
                Handles.DrawLine(hit.point - new Vector3(0, 0, crossSize), hit.point + new Vector3(0, 0, crossSize));
                Handles.DrawWireDisc(hit.point, hit.normal, 0.05f);
            }
            else
            {
                Handles.color = Color.red;
                Handles.DrawDottedLine(rayOrigin, rayOrigin + Vector3.down * rayLength, 3f);

                Handles.Label(rayOrigin + Vector3.down * rayLength, "No ground hit!");
            }

            SceneView.RepaintAll();
        }

        private Vector3 RaycastToGround(Vector3 position)
        {
            float height = 0.2f;

            if (Physics.Raycast(position + new Vector3(0, height, 0), Vector3.down, out RaycastHit hit, 2f, 1 << 12))
            {
                return hit.point;
            }
            return position;
        }
    }
}