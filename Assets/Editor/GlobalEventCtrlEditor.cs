using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalEventController))]
public class GlobalEventCtrlEditor : Editor {

    GlobalEventController _controller;

    bool[] _elementToggles = new bool[7];

    public override void OnInspectorGUI()
    {
        _controller = (GlobalEventController)target;

        //Goals
        _controller.goals = (Goals)EditorGUILayout.ObjectField("Goals", _controller.goals, typeof(Goals), true);

        //Dialog
        _controller.eventDialog = (GlobalEventDialog)EditorGUILayout.ObjectField("Event Dialog",_controller.eventDialog, typeof(GlobalEventDialog), true);

        //Count
        _controller.elementCount = EditorGUILayout.IntField("Element Count",_controller.elementCount);

        //survival
        _elementToggles[0] = EditorGUILayout.Foldout(_elementToggles[0], "Survival & Growth");

        if (_elementToggles[0])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[0] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[0], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[0] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[0].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[0].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[0].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //resource
        _elementToggles[1] = EditorGUILayout.Foldout(_elementToggles[1], "Resource");

        if (_elementToggles[1])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[1] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[1], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[1] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[1].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[1].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[1].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //industry
        _elementToggles[2] = EditorGUILayout.Foldout(_elementToggles[2], "Industry");

        if (_elementToggles[2])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[2] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[2], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[2] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[2].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[2].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[2].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //economy
        _elementToggles[3] = EditorGUILayout.Foldout(_elementToggles[3], "Economy");

        if (_elementToggles[3])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[3] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[3], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[3] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[3].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[3].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[3].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //law
        _elementToggles[4] = EditorGUILayout.Foldout(_elementToggles[4], "Law");

        if (_elementToggles[4])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[4] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[4], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[4] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[4].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[4].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[4].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //environment
        _elementToggles[5] = EditorGUILayout.Foldout(_elementToggles[4], "Environment");

        if (_elementToggles[5])
        {
            ++EditorGUI.indentLevel;
            _controller.eleEventControllerList[5] = (ElementEventController)EditorGUILayout.ObjectField(_controller.eleEventControllerList[5], typeof(ElementEventController), true);
            if (_controller.eleEventControllerList[5] != null)
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < _controller.eleEventControllerList[5].eventList.Length; ++i)
                {
                    _controller.eleEventControllerList[5].eventList[i] = (GlobalEvent)EditorGUILayout.ObjectField(_controller.eleEventControllerList[5].eventList[i], typeof(GlobalEvent), false);
                }
                --EditorGUI.indentLevel;
            }
            --EditorGUI.indentLevel;
        }

        //Editor.
    }
}
