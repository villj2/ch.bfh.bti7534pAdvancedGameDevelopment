using UnityEngine;
using UnityEditor;
using System;

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
        go.transform.localPosition = Vector3.zero;

        go.AddComponent<Waypoint>();

        script.waypoints.Add(go.transform);
    }

    private void RemoveWaypoint(int index)
    {
        DestroyImmediate(script.waypoints[index].gameObject);
        script.waypoints.Remove(script.waypoints[index]);
    }
}
