using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Character")]
public class CharacterData : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    public string Name => name;

    [field: SerializeField] public GameObject Model { get; private set; }

}
