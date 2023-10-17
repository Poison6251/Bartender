using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 추상 클래스  => 자식 클래스
public abstract class Flow : MonoBehaviour, IFlow
{
    // 추상 메서드
    public abstract void Enter();
    public abstract void Exit();
   
}


public interface IFlow
{
    public void Enter();
    public void Exit();
}

