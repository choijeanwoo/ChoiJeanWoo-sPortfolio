using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using static UnityEditor.PlayerSettings;

/*
 *  ID를 저장해서 배틀단계에서의 상호작용을 관리하는 메니저
 */

[System.Serializable]
public class BattleObject
{
    public UnitID UnitID;
    public TileID TileID;

    public BattleObject(UnitID unitID, TileID tileID)
    {
        this.UnitID = unitID;
        this.TileID = tileID;
    }
}

public class BattledataManager : MonoBehaviour
{
    [SerializeField] private Button checkButton;
    private bool isClicked = false;
    [SerializeField]private PieceObj SelectedPieceObj;

    private Dictionary<string, BattleObject> BattleBoard = new();
    private string[] column =new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
    private string[] row =new string[] { "H", "G", "F", "E", "D", "C", "B", "A" };
    private List<string> selectAvailablePos = new List<string>();
    private List<PieceObj> pieceList = new List<PieceObj>();
    private bool pieceEnable = true;

    public static BattledataManager Instance { get; private set; }
    private DataManager dm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < column.Length; i++)
        {
            for(int j = 0; j < row.Length; j++)
            {
                string tempString = row[j] + column[i];
                BattleBoard.Add(tempString, new BattleObject(null, null));
            }
        }
    }

    private void Start()
    {
        dm = DataManager.Instance;
    }

    private bool CheckObjectExist(BattleObject Standard, UnitID unitID)
    {
        if (Standard.UnitID == unitID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckTileExist(BattleObject Standard, TileID tileID)
    {
        if (Standard.TileID == tileID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddPieceList(PieceObj pieceObj)
    {
        pieceList.Add(pieceObj);
    }

    public void RemovePieceList(PieceObj pieceObj)
    {
        pieceList.Remove(pieceObj);
    }

    public void ResetPieceList()
    {
        pieceList.Clear();
    }

    public void PieceEnable(bool enable)
    {
        foreach (PieceObj piece in pieceList)
        {
            CapsuleCollider collider = piece.gameObject.GetComponent<CapsuleCollider>();
            collider.enabled = enable;
        }
        pieceEnable = enable;
    }

    public bool GetPieceEnable()
    {
        return pieceEnable;
    }


    public void SetSelectedPiece(PieceObj piece)
    {
        SelectedPieceObj = piece;
    }

    public PieceObj GetSelectedPiece()
    {
        return SelectedPieceObj;
    }

    #region updateBM
    public Vector2Int CoordinateToIndex(string coord)
    {
        string col = coord.Substring(1, 1);
        string rw = coord.Substring(0, 1);

        int x = Array.IndexOf(column, col);
        int y = row.Length - 1 - Array.IndexOf(row, rw);

        return new Vector2Int(x, y);
    }

    public string IndexToCoordinate(Vector2Int location)
    {
        return row[7 - location.y] + column[location.x];
    }

    public void AddTileInBoard(Vector2Int location, TileID tileID)
    {
        string stringPos = IndexToCoordinate(location);
        if (CheckTileExist(BattleBoard[stringPos], tileID))
        {
            Debug.LogWarning("기존의 타일이 존재합니다.");
        }
        BattleBoard[stringPos].TileID = tileID;
    }

    public void AddUnitInBoard(Vector2Int location, UnitID unitID)
    {
        string stringPos = IndexToCoordinate(location);
        if (CheckObjectExist(BattleBoard[stringPos], unitID))
        {
            Debug.LogError("한 칸에 기물이 두개 이상 존재할 수 없습니다.");
            return;
        }
        BattleBoard[stringPos].UnitID = unitID;
        Debug.Log($"{BattleBoard[stringPos].UnitID}");
    }


    public bool MovableTyle(Vector2Int location)
    {
        string stringPos = IndexToCoordinate(location);
        if (stringPos == "H1")
            Debug.Log($"{BattleBoard[stringPos].UnitID}");
        if (BattleBoard[stringPos].UnitID != null)
        {
            Debug.LogError("한 칸에 기물이 두개 이상 존재할 수 없습니다.");
            return false;
        }

        return true;
    }

    public PieceObj GetPieceInBoard(Vector2Int location)
    {
        string stringPos = IndexToCoordinate(location);
        foreach (PieceObj piece in pieceList)
        {
            if (piece.PieceData.unitID == BattleBoard[stringPos].UnitID.unitID)
                return piece;
        }
        Debug.LogWarning("해당 타일에 대상이 존재하지 않습니다.");
        return null;
    }

    public void RemoveUnitInBoard(Vector2Int location, UnitID unitID)
    {
        string stringPos = IndexToCoordinate(location);
        if(BattleBoard[stringPos] == null || BattleBoard[stringPos].UnitID == null)
        {
            Debug.LogWarning($"해당 {location}은 비어 있습니다.");
            return;
        }

        if (BattleBoard[stringPos].UnitID== unitID)
        {
            BattleBoard[stringPos].UnitID = null;
        }
        else
        {
            Debug.LogWarning("제거할 대상이 존재하지 않습니다.");
        }
    }


    public void RemoveAll()
    {
        BattleBoard.Clear();
    }
    #endregion

    #region Move

    public Vector3 CoordToVector3(string coord)
    {
        Vector2Int location = CoordinateToIndex(coord);
        Vector3 v3 = new Vector3(location.x * 10, (float)4.25, location.y * 10);
        return v3;
    }

    public void SetSelectedAvailablePos(List<string> availablePos)
    {
        selectAvailablePos.Clear();
        selectAvailablePos = new List<string>(availablePos);
    }

    public bool CheckAvailablePos(Vector2Int pos)
    {
        string stringPos = IndexToCoordinate(pos);
        if (selectAvailablePos.Contains(stringPos))
        {
            return true;
        }

        return false;
    }

    private SingleTile tempSingleTile;
    public void SetMovableSingleTile(SingleTile singleTile)
    {
        tempSingleTile = null;
        tempSingleTile = singleTile;
    }

    public SingleTile GetMovableSingleTile()
    {
        return tempSingleTile;
    }
    #endregion

    #region Attack

    private PieceObj AttTargetObj;
    private SingleTile AttTargetTile;

    private bool CheckObjectExist(Vector2Int pos)
    {
        string stringPos = IndexToCoordinate(pos);
        if (BattleBoard[stringPos].UnitID != null)
        {
            return true;
        }

        return false;
    }

    private bool CheckAttackable(PieceObj targetUnit, PieceObj subjectUnit)
    {
        if (targetUnit.IsWhite != subjectUnit.IsWhite)
            return true;
        return false;
    }

    public PieceObj StartSetAttSeq(PieceObj subjectUnit, Vector2Int pos)
    {
        AttTargetObj = null;
        if (CheckObjectExist(pos))
        {
            AttTargetObj = GetPieceInBoard(pos);
            if (CheckAttackable(AttTargetObj, subjectUnit))
            {
                Debug.Log($"공격 대상이 {pos.x}, {pos.y}에 존재합니다.");
                return AttTargetObj;
            }
            else
            {
                Debug.LogWarning("공격 가능한 오브젝트가 아닙니다.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("공격 가능한 위치에 오브젝트가 존재하지 않습니다.");
            return null;
        }
    }


    #endregion
}
