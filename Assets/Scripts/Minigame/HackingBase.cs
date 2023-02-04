using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HackingBase : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void Finish();
    public abstract void Cancel();
}
