using System.Collections.Generic;
using UnityEngine;

namespace Utility.LevelEditor
{
    public static class LevelGenerator
    {
        public static Level Generate(
            Level level,
            string elementID,
            LevelElement styleLevelElement,
            int width,
            int height,
            float shiftX,
            float shiftY)
        {
            Vector3 cellSize = styleLevelElement.transform.localScale;

            float halfW = (width - 1) * cellSize.x * 0.5f;
            float halfH = (height - 1) * cellSize.z * 0.5f;

            List<LevelElement> elements = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    LevelElement levelElement = Object.Instantiate(styleLevelElement, level.transform);

                    float localShiftX = y % 2 == 0 ? shiftX * cellSize.x : 0f;
                    float localShiftY = x % 2 == 0 ? shiftY * cellSize.z : 0f;

                    Vector3 localPos = new(
                        x * cellSize.x + localShiftX - halfW,
                        0f,
                        y * cellSize.z + localShiftY - halfH);

                    levelElement.transform.localPosition = localPos;
                    levelElement.name = styleLevelElement.name;
                    levelElement.SetElementID(elementID);
                    elements.Add(levelElement);
                }
            }

            level.AddElement(elements);
            return level;
        }
    }
}
