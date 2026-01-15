using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;

public enum GameState
{
    GameStart,//0
    MatchingStart,//1
    PrepPhase,//2
    BattlePhase,//3
    PlayerWaiting,//4
    PlayerActing,//5
    EnemyWaiting,//6
    EnemyActing,//7
    MathingEnd,//8
    GameEnd//9
}

public class TurnStateMachine
{
    private TurnStateRecorder _recorder;

    public TurnStateMachine(TurnStateRecorder recorder)
    {
        _recorder = recorder;
    }

    public Action<GameState, GameState> OnStateChanged;

    public void ChangeState(GameState newState)
    {
        if (_recorder.currentState == newState)
            return;

        GameState Prev = _recorder.currentState;
        _recorder.currentState = newState;

        OnStateChanged?.Invoke(Prev, newState);
    }
}
