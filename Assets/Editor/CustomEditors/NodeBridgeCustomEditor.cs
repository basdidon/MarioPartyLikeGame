using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace BD_Editor
{
    using Utils.UiElements;

    [CustomEditor(typeof(NodeBridge)), CanEditMultipleObjects]
    public class NodeBridgeCustomEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            var fromNodeField = new PropertyField(serializedObject.FindProperty("from"));
            var toNodeField = new PropertyField(serializedObject.FindProperty("to"));
            var isOneWaySP = serializedObject.FindProperty("isOneWay");
            var isOneWayField = new PropertyField(isOneWaySP);
            var isActiveField = new PropertyField(serializedObject.FindProperty("isActive"));

            var toCenterBtn = new Button() { text = "ToCenter" };

            container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));
            // btn[s]
            container.Add(toCenterBtn);
            // property field
            container.Add(fromNodeField);
            container.Add(toNodeField);
            container.Add(isOneWayField);
            container.Add(isActiveField);

            // Events
            toCenterBtn.clicked += () => {
                (target as NodeBridge).ToCenter();
            };

            isOneWayField.RegisterValueChangeCallback(evt => {
                if (PrefabUtility.IsPartOfPrefabAsset(target))  // don't do anything if object is partPrefabAsset
                    return;

                /// isOneWay is already changed
                /// ,but it change value directly to property
                /// ,so we need to set value again by setter
                /// ,to let it raise event

                if (target is NodeBridge nodeBridge)
                    nodeBridge.IsOneWay = evt.changedProperty.boolValue;
            });

            return container;
        }
    }
}
