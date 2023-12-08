using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public enum IngredientUnit { Spoon, Cup, Bowl, Piece }

// Custom serializable class
[System.Serializable]
public class Ingredient
{
    public string name;
    public int amount = 1;
    public IngredientUnit unit;
}

public class Recipe : MonoBehaviour
{
    public Ingredient potionResult;
    public Ingredient[] potionIngredients;
}

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(Ingredient))]
public class IngredientDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var amountField = new PropertyField(property.FindPropertyRelative("amount"));
        var unitField = new PropertyField(property.FindPropertyRelative("unit"));
        var nameField = new PropertyField(property.FindPropertyRelative("name"), "Fancy Name");

        // Add fields to the container.
        container.Add(amountField);
        container.Add(unitField);
        container.Add(nameField);

        return container;
    }
}
