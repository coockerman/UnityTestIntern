using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardTemp
{
    private int boardSizeX = 5;
    private int boardSizeY = 1;

    private Cell[] m_cells;

    private Transform m_root;

    private GameManager gameManager;

    public BoardTemp(Transform transform, GameManager gameManager)
    {
        m_root = transform;

        this.gameManager = gameManager;

        this.boardSizeX = 5;

        m_cells = new Cell[boardSizeX];

        CreateBoard();
    }
    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.SetParent(m_root);
            go.transform.localPosition = origin + new Vector3(x, 0, 0f);
            

            Cell cell = go.GetComponent<Cell>();
            cell.Setup(x, 1);

            m_cells[x] = cell;
        }
    }

    public void AddCell(Cell cellAdd)
    {
        Item itemAdd = cellAdd.Item;
        cellAdd.Free();

        for (int x = 0; x < boardSizeX; x++)
        {
            if (m_cells[x].Item == null)
            {
                m_cells[x].Free();
                m_cells[x].Assign(itemAdd);
                
                itemAdd.View.DOMove(m_cells[x].transform.position, 0.3f);
                break;
            }
        }
        CheckThirdCell();
        CheckEndGame();

    }
    
    void CheckThirdCell()
    {
        Dictionary<string, List<int>> itemGroups = new Dictionary<string, List<int>>();
        Debug.Log("Check Third cell");
        // Bước 1: Nhóm các Cell theo loại Item
        for (int i = 0; i < m_cells.Length; i++)
        {
            if (m_cells[i] == null || m_cells[i].IsEmpty) continue;
            Debug.Log("m_cell no empty");
            string item = m_cells[i].Item.PrefabName;

            // Nếu chưa có nhóm cho loại Item này, tạo mới
            if (!itemGroups.ContainsKey(item))
            {
                Debug.Log("Chua co nhom nen tao moi");
                itemGroups[item] = new List<int>();
            }

            // Lưu index của ô vào danh sách
            itemGroups[item].Add(i);
        }

        // Bước 2: Tìm nhóm có ít nhất 3 ô và xóa chúng
        foreach (var group in itemGroups)
        {
            if (group.Value.Count >= 3)
            {
                // Xóa 3 ô đầu tiên trong nhóm
                for (int i = 0; i < 3; i++)
                {
                    int index = group.Value[i];
                    m_cells[index].ExplodeItem(); // Kích hoạt hiệu ứng nổ
                    m_cells[index].Free(); // Đánh dấu là ô trống
                }
                break; // Chỉ xóa một nhóm trong một lần gọi
            }
        }
    }
    void CheckEndGame()
    {
        // Kiểm tra xem còn ô trống nào không
        bool hasEmptyCell = m_cells.Any(cell => cell.IsEmpty);
        if (!hasEmptyCell)
        {
            gameManager.SetState(GameManager.eStateGame.GAME_OVER);
            Debug.Log("End Game");
        }
    }
}
