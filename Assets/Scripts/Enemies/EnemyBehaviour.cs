using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemyBehaviour : NetworkBehaviour
{
    public event EventHandler onMoved;
    public event EventHandler onStopped;
    public event EventHandler onChangedDirection;

    protected void InvokeOnMoved(EventArgs e)
    {
        onMoved?.Invoke(this, e);
    }

    protected void InvokeOnStopped(EventArgs e)
    {
        onStopped?.Invoke(this, e);
    }

    protected void InvokeOnChangedDirection(EventArgs e)
    {
        onChangedDirection?.Invoke(this, e);
    }
}
