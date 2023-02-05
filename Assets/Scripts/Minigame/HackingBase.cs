using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackingBase : MonoBehaviour
{
    public abstract void Initialize();
    public virtual void Finish()
    {
        GameManager.Instance.Hacking.CompleteHacking();
    }
    public abstract void Cancel();
}
