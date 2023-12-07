using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Board
{
    public class Graph : MonoBehaviour
    {
        [field: ReadOnly] public int Id { get; set; }
        [field:ReadOnly] public Node Node { get; set; }
    }
}
