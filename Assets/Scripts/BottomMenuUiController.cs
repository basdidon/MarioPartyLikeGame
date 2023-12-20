using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BottomMenuUiController : MonoBehaviour
{
    VisualElement Root { get; set; }

    Button RollButton { get; set; }

    private void Awake()
    {
        if(TryGetComponent(out UIDocument uiDoc))
        {
            Root = uiDoc.rootVisualElement;

            RollButton = Root.Q<Button>("roll-btn");
            RollButton.clicked += RollButton_clicked;
        }
    }

    private void RollButton_clicked()
    {
        Debug.Log("Roll_Btn clicked");
        ActionMapper.Instance.GetActionRaiser(ActionMapper.EventName.Roll).Invoke();
    }
}
