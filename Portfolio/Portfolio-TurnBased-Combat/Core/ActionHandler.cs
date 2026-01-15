using Interfaces;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private BattledataManager bm;
    private Queue<ICommand> commandQueue = new();
    private List<ICommand> history = new();

    public void Start()
    {
        bm = BattledataManager.Instance;
    }

    public void Enqueue(ICommand command)
    {
        commandQueue.Enqueue(command);
    }

    public void ActionUpdate()
    {
        if (commandQueue.Count == 0) return;

        ICommand cmd = commandQueue.Dequeue();

        cmd.Execute();

        bm.PieceEnable(true);

        history.Add(cmd);
    }

    public void Reset()
    {
        commandQueue.Clear();
        history.Clear();
    }
}
