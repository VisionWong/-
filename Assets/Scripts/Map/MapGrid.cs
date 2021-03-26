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
    public TerrainType TerrainType { get; set; }
    public bool CanMove { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    private SelectableGrid m_selectable;
    private Outline m_outline = null;

    private void Awake()
    {
        CanMove = true;
        m_outline = GetComponent<Outline>();
        m_selectable = GetComponent<SelectableGrid>();
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
        m_selectable.SelectedState = GridSelectedState.Idle;
    }

    /// <summary>
    /// 停留棋子，变得无法被选中和通过
    /// </summary>
    public void StayChess()
    {
        m_selectable.SelectedState = GridSelectedState.Unselectable;
        CanMove = false;
    }

    /// <summary>
    /// 棋子离开，重新可以被选中和通过
    /// </summary>
    public void ChessAway()
    {
        m_selectable.SelectedState = GridSelectedState.Idle;
        CanMove = true;
    }
}
