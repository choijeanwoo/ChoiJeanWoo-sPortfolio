using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPieceData", menuName = "ScriptableObject/ScriptableObjectData/PieceObjData")]
public class CardPieceDataTemplate : ScriptableObject
{
    [SerializeField] public int dataID;
    [SerializeField] public string pieceName;
    [SerializeField] public float pieceHP;
    [SerializeField] public float atk;
    [SerializeField] public float def;
    [SerializeField] public List<int> skillIDs;
    [SerializeField] public GameObject prefab;
    [SerializeField] public string pieceClass;

    public void SetPieceData(DataBase.CardPieceDataParam data)
    {
        dataID = data.dataID;
        pieceName = data.pieceName;
        pieceHP = data.pieceHP;
        atk = data.atk;
        def = data.def;
    }
}
