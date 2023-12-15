using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T> where T : IStateActor<T>
{
    T StateActor { get; }

    void EnterState();
    void UpdateState();
    void ExitState();
}

public interface IStateActor<T> where T : IStateActor<T>
{
    IState<T> State { get; set; }
}