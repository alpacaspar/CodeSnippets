using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private FiniteStateMachine stateMachine;
    private FiniteStateMachine.State idle, performAction;

    [SerializeField] private Action[] actionPattern;
    private readonly Queue<Action> actions = new();

    private void Start()
    {
        stateMachine = new FiniteStateMachine();

        CreateIdleState();
        CreateActionState();

        stateMachine.PushState(idle);

        foreach (var action in actionPattern)
        {
            actions.Enqueue(action);
        }
    }

    private void Update()
    {
        stateMachine.Update(gameObject);
    }

    private void CreateIdleState()
    {
        idle = (fsm, gameObj) =>
        {
            if (actions.Any())
            {
                fsm.PopState();
                fsm.PushState(performAction);
            }
        };
    }

    private void CreateActionState()
    {
        performAction = (fsm, gameObj) =>
        {
            var action = actions.Peek();
            
            var finished = !action.CanPerform() || action.PerformAction();
            
            if (finished)
            {
                action.DoReset();
                Transfer(action);

                fsm.PopState();
                fsm.PushState(idle);
            }
        };
    }

    private void Transfer(Action action)
    {
        actions.Dequeue();
        actions.Enqueue(action);
    }
}
