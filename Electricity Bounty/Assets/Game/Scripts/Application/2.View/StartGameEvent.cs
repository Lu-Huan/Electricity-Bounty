using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameEvent : MonoBehaviour {

    private Animator Animator;
    private int IsComing;
    private void Start()
    {
        Animator = GetComponent<Animator>();
        IsComing = Animator.StringToHash("IsComing");
    }

    public void ActionEvent1(string _arg1)
    {
        if (_arg1== "StartGame2")
        {
            //开始关卡
            Game.Instance.LoadScene(3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Animator.SetBool(IsComing, true);
    }
    private void OnTriggerExit(Collider other)
    {
        Animator.SetBool(IsComing, false);
    }
}
