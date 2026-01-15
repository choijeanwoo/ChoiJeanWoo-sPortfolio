using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private TurnStateMachine _machine;

    [SerializeField] private TurnStateRecorder _recorder;
    [SerializeField] private PlayerDataSO _playerData;
    [SerializeField] private PlayerDataSO _enemyData;

    private PlayerActionSystem _playerActionSystem;
    private PlayerActionSystem _enemyActionSystem;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _playerActionSystem = new PlayerActionSystem(_playerData);
        _enemyActionSystem  = new PlayerActionSystem(_enemyData);

        _machine = new TurnStateMachine(_recorder);
        _machine.OnStateChanged += HandleStateChanged;

        _machine.ChangeState(GameState.PlayerWaiting);
    }

    public void HandleStateChanged(GameState from, GameState to)
    {
        Debug.Log($"{from} -> {to}");
        switch (to)
        {
            //추후 에니메이션 출력 부분 어떻게 할지 정해야함
            case GameState.PlayerActing:
            {
                _playerActionSystem.OnTurnStart();
                _machine.ChangeState(GameState.EnemyWaiting);
                break;
            }

            case GameState.EnemyActing:
            {
                _enemyActionSystem.OnTurnStart();
                _machine.ChangeState(GameState.PlayerWaiting);
                break;
            }
        }
    }

}
