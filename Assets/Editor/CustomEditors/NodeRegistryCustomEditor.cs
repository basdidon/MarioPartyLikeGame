using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;

namespace BD_Editor
{
    using Utils.UiElements;

    [CustomEditor(typeof(NodeRegistry))]
    public class NodeRigistryCustomEditor : Editor
    {
        enum EdgeDirections { Normal, Inverese, Both }
        EdgeDirections edgeDirection = EdgeDirections.Normal;

        Node l_Node;
        Node r_Node;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            var startNodePF = new PropertyField(serializedObject.FindProperty("startNode"));

            var nodesTranformPF = new PropertyField(serializedObject.FindProperty("nodesTransform"));
            var nodeBridgesTranformPF = new PropertyField(serializedObject.FindProperty("nodeBridgesTransform"));

            var setupBtn = new Button() { text = "Setup" };

            var nodesPF = new PropertyField(serializedObject.FindProperty("nodes"));
            var nodeBridgesPF = new PropertyField(serializedObject.FindProperty("nodeBridges"));

            var poolManagerPF = new PropertyField(serializedObject.FindProperty("poolManager"));

            // Connect to exits node
            var connectNodesContainer = new VisualElement();
            var L_NodeOF = new ObjectField() { objectType = typeof(Node), value = l_Node };
            var R_NodeOF = new ObjectField() { objectType = typeof(Node), value = r_Node };
            var swapBtn = new Button() { text = GetBtnIcon() };
            var createBridgeBtn = new Button() { text = "Create Bridge" };

            connectNodesContainer.style.flexDirection = FlexDirection.Row;
            connectNodesContainer.Add(L_NodeOF);
            connectNodesContainer.Add(swapBtn);
            connectNodesContainer.Add(R_NodeOF);
            connectNodesContainer.Add(createBridgeBtn);

            L_NodeOF.RegisterValueChangedCallback(evt =>
            {
                l_Node = evt.newValue as Node;
            });

            R_NodeOF.RegisterValueChangedCallback(evt =>
            {
                r_Node = evt.newValue as Node;
            });

            swapBtn.clicked += () =>
            {
                var enumIdx = (int)edgeDirection;
                enumIdx = (enumIdx + 1) % Enum.GetValues(typeof(EdgeDirections)).Length;
                edgeDirection = (EdgeDirections)enumIdx;

                swapBtn.text = GetBtnIcon();
            };

            createBridgeBtn.clicked += () => OnCreateEdgeBtnClicked();

            container.Add(BD_PropertyField.GetDefaultScriptRef(serializedObject));
            container.Add(startNodePF);
            container.Add(nodesTranformPF);
            container.Add(nodeBridgesTranformPF);
            container.Add(poolManagerPF);
            container.Add(setupBtn);
            container.Add(connectNodesContainer);
            container.Add(nodesPF);
            container.Add(nodeBridgesPF);

            setupBtn.clicked += (target as NodeRegistry).Setup;

            return container;
        }

        private void OnCreateEdgeBtnClicked()
        {
            if (target is NodeRegistry nodeRegistry && l_Node != null && r_Node != null)
            {
                if (edgeDirection == EdgeDirections.Normal)
                {
                    NodeFactory.CreateNodeBridge(l_Node, r_Node);
                }
                else if (edgeDirection == EdgeDirections.Inverese)
                {
                    NodeFactory.CreateNodeBridge(r_Node, l_Node);
                }
                else
                {
                    NodeFactory.CreateNodeBridge(l_Node, r_Node, false);
                }
            }
        }


        string GetBtnIcon()
        {
            if (edgeDirection == EdgeDirections.Normal)
            {
                return "-->";
            }
            else if (edgeDirection == EdgeDirections.Inverese)
            {
                return "<--";
            }
            else
            {
                return "<O>";
            }
        }
    }
}