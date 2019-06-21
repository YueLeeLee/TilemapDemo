using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "Face Brush")]
    [CreateAssetMenu(fileName = "New Face Brush", menuName = "Brushes/Face Brush")]
    public class FaceBrush : GridBrush
    {
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            Vector2Int pos = new Vector2Int(position.x,position.y);
            foreach (var point in GetPointsOnFace(pos))
            {
                Vector3Int paintPos = new Vector3Int(point.x, point.y, position.z);
                base.Paint(grid, brushTarget, paintPos);
            }
        }

        IEnumerable<Vector2Int> GetPointsOnFace(Vector2Int pos)
        {

            for (int x = pos.x - 2; x <= pos.x + 2; x++) 
            {
                if (x != pos.x) 
                {
                    yield return new Vector2Int(x, pos.y + 2);
                }
            }

            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                yield return new Vector2Int(x, pos.y);
            }

        }

         public static IEnumerable<Vector2Int> GetSmilePointsOnFace(Vector3Int pos)
        {
            
            for (int x = pos.x - 3; x <= pos.x + 3; x++)
            {
                if (x == pos.x - 2 || x == pos.x + 2) 
                {
                    yield return new Vector2Int(x, pos.y + 3);
                }
                else if(x != pos.x)
                {
                    yield return new Vector2Int(x, pos.y + 2);
                }
            }

            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                yield return new Vector2Int(x, pos.y);
            }

        }
    }


    [CustomEditor(typeof(FaceBrush))]
    public class FaceBrushEditor : GridBrushEditor
    {
        private Tilemap lastTilemap;
        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);

            lastTilemap = brushTarget.GetComponent<Tilemap>();

            foreach (var point in FaceBrush.GetSmilePointsOnFace(position.position))
            {
                Vector3Int paintPos = new Vector3Int(point.x, point.y, position.z);
                PaintPreview(gridLayout, brushTarget, paintPos);
            }

            //PaintPreview(gridLayout, brushTarget, position.min);
        }


        public override void ClearPreview()
        {
            base.ClearPreview();
            if (lastTilemap != null)
            {
                lastTilemap.ClearAllEditorPreviewTiles();
                lastTilemap = null;
            }
        }
    }
}

