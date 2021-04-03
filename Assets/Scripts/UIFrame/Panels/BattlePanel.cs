using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

public class BattlePanel : BasePanel
{
    private Transform _gridInfo;
    private Transform _chessInfo;

    private Transform _gridDes;
    private Text txtGridDes;
    private Text txtGridType;

    public override void OnEnter()
    {
        FindAll();
        HideAll();
        RegisterAll();
    }

    private void FindAll()
    {
        //棋子信息
        _chessInfo = transform.Find("ChessInfo");

        //棋格信息
        _gridInfo = transform.Find("GridInfo");
        _gridDes = _gridInfo.Find("Description");
        txtGridType = _gridInfo.Find("txtGridType").GetComponent<Text>();
        txtGridDes = _gridDes.Find("txtGridDes").GetComponent<Text>();
    }

    private void RegisterAll()
    {
        MessageCenter.Instance.AddListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectGrid);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener<MapGrid>(MessageType.OnSelectIdleGrid, OnSelectGrid);
    }

    private void Update()
    {
        if (_gridInfo.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_gridDes.gameObject.activeSelf)
                    _gridDes.gameObject.SetActive(false);
                else _gridDes.gameObject.SetActive(true);
            }
        }
    }

    private void HideAll()
    {
        _gridDes.gameObject.SetActive(false);
        _gridInfo.gameObject.SetActive(false);

        _chessInfo.gameObject.SetActive(false);
    }

    private void OnSelectGrid(MapGrid grid)
    {
        if (!_gridInfo.gameObject.activeSelf)
            _gridInfo.gameObject.SetActive(true);
        txtGridType.text = grid.TypeName;
        txtGridDes.text = grid.Description;
    }

    private void ShowGridDes()
    {
        _gridDes.gameObject.SetActive(true);
    }

    private void HideGridDes()
    {
        _gridDes.gameObject.SetActive(false);
    }
}
