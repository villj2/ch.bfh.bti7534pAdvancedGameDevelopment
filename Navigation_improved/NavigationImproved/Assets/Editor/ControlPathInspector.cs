using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

// Damit Objekte mit der Klasse "PatrolPath" einen eigenen Inspector kriegen
[CustomEditor(typeof(PatrolPath))]
public class ControlPathInspector : Editor
{
    private PatrolPath script { get { return target as PatrolPath; } }

    SerializedProperty direction;
    SerializedProperty closedPath;
    SerializedProperty waypoints;

    void OnEnable()
    {
        direction = serializedObject.FindProperty("direction");
        closedPath = serializedObject.FindProperty("closedPath");
        waypoints = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawDefaultInspector();

        EditorGUILayout.PropertyField(direction);
        EditorGUILayout.PropertyField(closedPath);
        
        for(int i = 0; i < script.waypoints.Count; i++)
        {
            GUILayout.BeginHorizontal();

            var result = EditorGUILayout.ObjectField(script.waypoints[i], typeof(Transform), true) as Transform;
            if (GUI.changed)
            {
                script.waypoints[i] = result;
            }

            var oldEnabled = GUI.enabled;

            GUI.enabled &= CanMoveUp(i);
            if (GUILayout.Button("U", GUILayout.Width(25.0f)))
            {
                SwapWaypoints(i, i - 1);
            }

            GUI.enabled = oldEnabled && CanMoveDown(i);
            if (GUILayout.Button("D", GUILayout.Width(25.0f)))
            {
                SwapWaypoints(i, i + 1);
            }

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            GUI.enabled = oldEnabled;

            if (GUILayout.Button("-", GUILayout.Width(25f)))
            {
                RemoveWaypoint(i);
            }

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Waypoint"))
        {
            AddWaypoint();
        }

        // TODO down / up button pro waypoint hinzufügen

        serializedObject.ApplyModifiedProperties();
    }

    private void AddWaypoint()
    {
        GameObject go = new GameObject("Waypoint");
        go.transform.parent = script.transform;
        // TODO setze Position vom letzten Waypoint in der Liste
        go.transform.localPosition = script.waypoints.Last().localPosition;

        go.AddComponent<Waypoint>();

        script.waypoints.Add(go.transform);
    }

    private void RemoveWaypoint(int index)
    {
        DestroyImmediate(script.waypoints[index].gameObject);
        script.waypoints.Remove(script.waypoints[index]);
    }

    void SwapWaypoints(int i0, int i1)
    {
        var waypointTemp = script.waypoints[i0];
        script.waypoints[i0] = script.waypoints[i1];
        script.waypoints[i1] = waypointTemp;
    }

    bool CanMoveUp(int index)
    {
        return index > 0;
    }

    bool CanMoveDown(int index)
    {
        return index < (script.waypoints.Count - 1);
    }
}
