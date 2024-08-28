using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private readonly Stack<State> states = new();
    public delegate void State(FiniteStateMachine fsm, GameObject gameObject); 
    
    public void Update (GameObject gameObject) 
    {
        states.Peek()?.Invoke(this, gameObject);
    }

    public void PushState(State state)
    {
        states.Push(state);
    }

    public void PopState()
    {
        states.Pop();
    }
}
