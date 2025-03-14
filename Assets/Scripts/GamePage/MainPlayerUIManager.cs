using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainPlayerUIManager : MonoBehaviour
{
    [Header("UI 배치 설정")]
    public RectTransform handPanel;        // 손패 UI 패널
    public int tileCount = 13;             // 유지할 손패 개수
    public float tileSpacing = 25f;        // 타일 간 간격(px)

    [Header("2D 타일 매핑")]
    public Tile2DMapping[] tile2DMappings; // suit/value -> 2D 프리팹 배열

    [Header("Discard Manager 참조")]
    public DiscardManager discardManager;  // 3D 버림패 매니저

    // 현재 손패에 있는 타일 오브젝트들
    private List<GameObject> handTiles = new List<GameObject>();

    void Start()
    {
        // 초기 손패 생성
        for (int i = 0; i < tileCount; i++)
        {
            AddTileToHand(CreateRandomTileData());
        }
        UpdateHandDisplay();
    }

    /// <summary>
    /// TileData를 바탕으로 2D 타일 프리팹을 찾아 Instantiate한 뒤 손패에 추가
    /// </summary>
    public void AddTileToHand(TileData data)
    {
        GameObject prefab2D = Get2DPrefab(data.suit, data.value);
        if (prefab2D == null)
        {
            Debug.LogWarning($"2D 프리팹 없음: suit={data.suit}, value={data.value}");
            return;
        }

        // 손패 패널 아래에 프리팹 생성
        GameObject newTile = Instantiate(prefab2D, handPanel);

        // TileController에 TileData와 uiManager 할당
        TileController tc = newTile.GetComponent<TileController>();
        if (tc != null)
        {
            tc.tileData = data;
            tc.uiManager = this;
        }

        handTiles.Add(newTile);
        UpdateHandDisplay();
    }

    /// <summary>
    /// 손패 UI를 가로 정렬
    /// </summary>
    private void UpdateHandDisplay()
    {
        float totalWidth = (handTiles.Count - 1) * tileSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < handTiles.Count; i++)
        {
            RectTransform rect = handTiles[i].GetComponent<RectTransform>();
            float xPos = startX + i * tileSpacing;
            rect.anchoredPosition = new Vector2(xPos, 0f);
        }
    }

    /// <summary>
    /// 타일 클릭 시 호출되는 함수 (TileController -> uiManager.OnTileClicked(this))
    /// </summary>
    public void OnTileClicked(TileController tile)
    {
        Debug.Log($"[MainPlayerUIManager] 타일 클릭: {tile.tileData.suit}{tile.tileData.value}");

        // 1) DiscardManager에 버림
        if (discardManager != null)
        {
            discardManager.DiscardTile(PlayerSeat.E, tile.tileData);
        }
        else
        {
            Debug.LogWarning("DiscardManager가 할당되지 않았습니다!");
        }

        // 2) 손패에서 제거
        GameObject tileObj = tile.gameObject;
        if (handTiles.Contains(tileObj))
        {
            handTiles.Remove(tileObj);
            Destroy(tileObj);
        }

        // 3) 새 타일 뽑아 손패 유지 (예: 다시 13장 맞추기)
        //    여기서는 단순히 "한 장" 추가해서 다시 13장 맞추도록
        if (handTiles.Count < tileCount)
        {
            AddTileToHand(CreateRandomTileData());
        }

        // 4) 정렬
        UpdateHandDisplay();
    }

    /// <summary>
    /// suit/value에 해당하는 2D 프리팹을 Tile2DMapping에서 찾는다
    /// </summary>
    private GameObject Get2DPrefab(string suit, int value)
    {
        foreach (var mapping in tile2DMappings)
        {
            if (mapping.suit.ToLower() == suit.ToLower() && mapping.value == value)
            {
                return mapping.prefab2D;
            }
        }
        return null;
    }

    /// <summary>
    /// 만수(“m”) 1~9만 랜덤 생성 (테스트용)
    /// </summary>
    private TileData CreateRandomTileData()
    {
        // suit를 "m"으로 고정
        string suit = "m";
        int value = Random.Range(1, 10); // 1~9
        return new TileData { suit = suit, value = value };
    }
}
