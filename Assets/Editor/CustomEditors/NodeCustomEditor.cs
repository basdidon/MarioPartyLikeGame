using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace BD_Editor
{
    using Utils.UiElements;

#if UNITY_EDITOR
    [CustomEditor(typeof(Node))]
    public class NodeCustomEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));

            var nodeTypePF = new PropertyField(serializedObject.FindProperty("nodeType"));
            nodeTypePF.RegisterValueChangeCallback(cb =>
            {
                Debug.Log($"{cb.changedProperty.enumNames[cb.changedProperty.enumValueIndex]}");
                (target as Node).NodeType = (NodeTypes)cb.changedProperty.enumValueIndex;
            });
            var selectablePF = new PropertyField(serializedObject.FindProperty("selectable"));

            // List of bridges
            var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));
            //
            container.Add(nodeTypePF);
            container.Add(selectablePF);
            container.Add(nodeBridgesPF);
            return container;
        }

        private void OnSceneGUI()
        {
            Handles.color = Color.red;

            if (target is Node node)
            {
                var moveSnap = EditorSnapSettings.move;

                var rightPos = node.transform.position + Vector3.right * moveSnap.x;
                var leftPos = node.transform.position - Vector3.right * moveSnap.x;
                var forwardPos = node.transform.position + Vector3.forward * moveSnap.z;
                var backPos = node.transform.position - Vector3.forward * moveSnap.z;

                DrawCreateNodeBtn(rightPos, node);
                DrawCreateNodeBtn(leftPos, node);
                DrawCreateNodeBtn(forwardPos, node);
                DrawCreateNodeBtn(backPos, node);

                if (Selection.activeGameObject == node.gameObject && node.transform.hasChanged)
                {
                    node.AdjustAllNodeBridges();

                    node.transform.hasChanged = false;
                }
            }
        }

        void DrawCreateNodeBtn(Vector3 position, Node fromNode)
        {
            if (Handles.Button(position, Quaternion.identity, .2f, .2f, Handles.SphereHandleCap))
            {
                NodeFactory.CreateOutputNode(position, fromNode);
            }
        }
    }
#endif
}