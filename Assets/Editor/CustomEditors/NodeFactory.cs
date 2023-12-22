using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BD_Editor
{
    public static class NodeFactory
    {
        static NodeRegistry NodeRegistry => NodeRegistry.Instance;  // make sure nodeRegistry is executeInEditMode

        static GameObject nodePrefab;
        static GameObject NodePrefab
        {
            get
            {
                if (nodePrefab == null)
                    nodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/NodeObjects/Node.prefab");

                return nodePrefab;
            }
        }
        static GameObject nodeBridgePrefab;
        static GameObject NodeBridgePrefab
        {
            get
            {
                if (nodeBridgePrefab == null)
                    nodeBridgePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/NodeObjects/NodeBridge.prefab");

                return nodeBridgePrefab;
            }
        }

        public static Node CreateNode()
        {
            var nodeGo = PrefabUtility.InstantiatePrefab(NodePrefab, NodeRegistry.NodesTransform) as GameObject;
            if (!nodeGo.TryGetComponent(out Node node))
            {
                node = nodeGo.AddComponent<Node>();
            }

            NodeRegistry.AddNode(node);

            return node;
        }

        public static NodeBridge CreateNodeBridge(Node fromNode, Node toNode, bool isOneWay = true, bool isActive = true)
        {
            var nodeBridgeGO = PrefabUtility.InstantiatePrefab(NodeBridgePrefab, NodeRegistry.NodeBridgesTransform) as GameObject;

            if (!nodeBridgeGO.TryGetComponent(out NodeBridge nodeBridge))
            {
                nodeBridge = nodeBridgeGO.AddComponent<NodeBridge>();
            }

            nodeBridge.Initialize(fromNode, toNode, isOneWay, isActive);

            NodeRegistry.AddNodeBridge(nodeBridge);

            return nodeBridge;
        }

        public static void CreateOutputNode(Node fromNode)
        {
            CreateOutputNode(fromNode.transform.position, fromNode);
        }

        public static void CreateOutputNode(Vector3 targetPos, Node fromNode)
        {
            var toNode = CreateNode();
            toNode.transform.position = targetPos;

            CreateNodeBridge(fromNode, toNode);

            Selection.activeObject = toNode;
        }
    }
}