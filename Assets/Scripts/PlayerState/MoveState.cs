using BasDidon.PathFinder.NodeBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveState : IState<Player>
{
    public Player StateActor { get; }
    public IEnumerator<Node> Enumerator { get; }
    public NodePath<Node> NodePath { get; }

    float MOVE_SPEED => 8;
    Vector3 startPos;
    Vector3 targetPos;
    float duration;

    float timeElapsed;

    public MoveState(Player player,IEnumerator<Node> enumerator)
    {
        StateActor = player;
        Enumerator = enumerator;
    }

    public void EnterState()
    {
        if (Enumerator.MoveNext())
        {
            startPos = StateActor.transform.position;
            targetPos = Enumerator.Current.transform.position;
            duration = Vector3.Distance(startPos,targetPos) / MOVE_SPEED;

            timeElapsed = 0;
        }
        else
        {
            StateActor.State = null;  // return to idlestate
        }
    }

    public void UpdateState()
    {
        if(timeElapsed >= duration)
        {
            StateActor.CurrentNode = Enumerator.Current;
            StateActor.State = this;
        }
        else
        {
            StateActor.transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
        }
    }

    public void ExitState()
    {

    }
}
