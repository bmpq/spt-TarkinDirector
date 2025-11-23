using tarkin.BSP.Shared;
using UnityEditor;
using UnityEngine;

namespace tarkin.BSP.EditorNamespace
{
    [CustomEditor(typeof(Tripwire))]
    public class TripwireEditor : Editor
    {
        private void OnSceneGUI()
        {
            Tripwire tripwire = (Tripwire)target;


            bool AreVectorsClose(Vector3 a, Vector3 b, float epsilon)
            {
                return (a - b).sqrMagnitude < epsilon * epsilon;
            }
            Vector3 PosFromGround = RaycastToGround(tripwire.PosFrom);
            Vector3 PosToGround = RaycastToGround(tripwire.PosTo);

            bool mouseUp = (Event.current.type == EventType.MouseUp && Event.current.button == 0);

            if (!AreVectorsClose(tripwire.PosFrom, PosFromGround, 0.01f))
            {
                if (mouseUp)
                    tripwire.PosFrom = PosFromGround;
                else
                {
                    Handles.color = Color.red;
                    Handles.DrawDottedLine(tripwire.PosFrom, PosFromGround, 0.3f);
                    Handles.color = Color.white;
                }
            }

            if (!AreVectorsClose(tripwire.PosTo, PosToGround, 0.01f))
            {
                if (mouseUp)
                    tripwire.PosTo = PosToGround;
                else
                {
                    Handles.color = Color.red;
                    Handles.DrawDottedLine(tripwire.PosTo, PosToGround, 0.3f);
                    Handles.color = Color.white;
                }
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newEndPos = Handles.PositionHandle(tripwire.PosTo, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tripwire.end, "Move Tripwire End");
                tripwire.PosTo = newEndPos;
            }
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