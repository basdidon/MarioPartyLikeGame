using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using BasDidon.Editor.UiElements;

[CreateAssetMenu(menuName = "ScriptableObjects/Board/Node")]
public class NodeData : ScriptableObject
{
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Texture2D Texture { get; private set; }
}