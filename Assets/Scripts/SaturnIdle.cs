using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturnIdle : StateMachineBehaviour
{
    [SerializeField]
    private float _timeUntilBored;

    [SerializeField]
    private int _numberOfBoredAnimation;

    private bool _isBored;

    [SerializeField]
    private float _idleTime;

    private int _boredAnimation;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f) 
            { 
                _isBored = true;
                _boredAnimation = Random.Range(1, _numberOfBoredAnimation + 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 >= 0.98)
        {
            ResetIdle();
        }

        animator.SetFloat("BoredAnimation", _boredAnimation);
    }

    private void ResetIdle()
    {
        _isBored = false;
        _idleTime = 0;
        _boredAnimation = 0;

    }
}
