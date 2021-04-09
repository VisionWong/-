using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
    None,
    Obstacle,
    Grass,
    Water,
    Magma, //岩浆
}



public class MapGrid : MonoBehaviour
{
    public string TypeName { get; set; }
    public string Description { get; set; }
    public TerrainType TerrainType { get; set; }
    public bool CanMove { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public IChess StayedChess { get; set; }

    private SelectableGrid m_selectable;
    private Outline m_outline = null;

    private void Awake()
    {
        CanMove = true;
        m_outline = GetComponent<Outline>();
        m_selectable = GetComponent<SelectableGrid>();

        //TODO
        TypeName = TerrainType.ToString();
        Description = "没有任何效果";
    }

    public void SetCoord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void HighlightWalkable()
    {
        m_outline.enabled = true;
        m_outline.color = 1;
        m_selectable.SelectedState = GridSelectedState.Moveable;
    }

    public void HighlightAttackable()
    {
        m_outline.enabled = true;
        m_outline.color = 0;
        m_selectable.SelectedState = GridSelectedState.Attackable;
    }

    public void CancelHighlight()
    {
        m_outline.enabled = false;
        if (StayedChess != null)
        {
            m_selectable.SelectedState = GridSelectedState.Unselectable;
        }
        else m_selectable.SelectedState = GridSelectedState.Idle;
    }

    /// <summary>
    /// 停留棋子，变得无法被选中和通过
    /// </summary>
    public void StayChess(IChess chess)
    {
        m_selectable.SelectedState = GridSelectedState.Unselectable;
        CanMove = false;
        StayedChess = chess;
        OnEnter(chess);
    }

    /// <summary>
    /// 棋子离开，重新可以被选中和通过
    /// </summary>
    public void ChessAway()
    {
        OnExit(StayedChess);
        m_selectable.SelectedState = GridSelectedState.Idle;
        CanMove = true;
        StayedChess = null;
    }

    public virtual void OnEnter(IChess chess) { }
    public virtual void OnStay() { }
    public virtual void OnExit(IChess chess) { }
}
