using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MFarm.GridMap
{
    /// <summary>
    /// 瓦片地图
    /// 编辑模式下运行
    /// </summary>
    [ExecuteInEditMode]
    public class GridMap : MonoBehaviour
    {
        public MapData_SO mapData;
        public E_GridType gridType;
        private Tilemap curTilemap;

        private void OnEnable()
        {
            if (!Application.IsPlaying(this))
            {
                curTilemap = GetComponent<Tilemap>();

                if (mapData != null)
                {
                    mapData.tilePropertieList.Clear();
                }
            }
        }
        private void OnDisable()
        {
            if (!Application.IsPlaying(this))   //是否在运行模式下，如果没有则
            {
                curTilemap = GetComponent<Tilemap>();

                UpdateTileProperties();
#if UNITY_EDITOR
                if (mapData != null)
                {
                    EditorUtility.SetDirty(mapData);
                }
#endif
            }
        }

        /// <summary>
        /// 更新瓦片网格信息
        /// </summary>
        private void UpdateTileProperties()
        {
            curTilemap.CompressBounds();    //将tilemap边界缩小为当前指定tilemap指定的边界，即有内容的真实的边界

            if (!Application.IsPlaying(this))
            {
                if (mapData != null)
                {
                    Vector3Int startPos = curTilemap.cellBounds.min;    //所绘制区域的左下角坐标
                    Vector3Int endPos = curTilemap.cellBounds.max;     //所绘制区域的右上角坐标.
                    for (int x = startPos.x; x < endPos.x; x++)
                    {
                        for (int y = startPos.y; y < endPos.y; y++)
                        {
                            TileBase tile = curTilemap.GetTile(new Vector3Int(x, y, 0));

                            if (tile != null)
                            {
                                TileProperty newTile = new TileProperty
                                {
                                    tileCoordinate = new Vector2Int(x, y),
                                    gridType = this.gridType,
                                    boolTypeValue = true
                                };

                                mapData.tilePropertieList.Add(newTile);
                            }
                        }
                    }
                }
            }
        }
    }
}
