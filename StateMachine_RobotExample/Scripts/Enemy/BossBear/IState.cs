using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    public void Enter();
    public void Process();
    public void Exit();
    public List<GameObject> CollectGarbage();
    
}
