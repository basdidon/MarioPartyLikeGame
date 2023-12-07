using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ListUiElement
{
    public static VisualElement DrawListUiElement<T>(string title, List<T> sourceList,bool allowSceneObjects = true) where T:UnityEngine.Object
    {
        var listView = new ListView(sourceList)
        {
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            showFoldoutHeader = true,
            headerTitle = title,
            showAddRemoveFooter = true,
            showBorder = true,
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            makeItem = () => new ObjectField
            {
                objectType = typeof(T),
                allowSceneObjects = allowSceneObjects,

            },
            bindItem = (element, i) =>
            {
                ((ObjectField)element).value = sourceList[i];
                ((ObjectField)element).RegisterValueChangedCallback((value) =>
                {
                    sourceList[i] = (T)value.newValue;
                });
            },

        };

        return listView;
    }
}
