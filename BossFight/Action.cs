using UnityEngine;

public abstract class Action : MonoBehaviour
{
    public virtual bool CanPerform()
    {
        return true;
    }
    
    public abstract bool PerformAction();

    public void DoReset()
    {
        Reset();
    }

    protected abstract void Reset();
}