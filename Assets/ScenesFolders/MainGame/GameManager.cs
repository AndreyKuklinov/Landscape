using System;
using System.Drawing;
using UnityEngine;

namespace ScenesFolders.MainGame
{
    public class GameManager : MonoBehaviour
    {
        private enum Actions
        {
            Place,
            Move
        }

        private ((Point currentCoordinates, Point coordinatesBefore), Actions) previouseAction;

        public TilesEnum.Tiles[] GetPossibleTiles(int diceValue)
        {
            if (diceValue == 6)
                return new[]
                {
                    TilesEnum.Tiles.Mountain, TilesEnum.Tiles.Forest, TilesEnum.Tiles.Plain, TilesEnum.Tiles.Lake,
                    TilesEnum.Tiles.Village
                };
            return new[] {(TilesEnum.Tiles) (diceValue - 1)};
        }

        public void PlaceTile(int x, int y, TilesEnum.Tiles tile)
        {
            previouseAction = ((new Point(x, y), Point.Empty), Actions.Place);
            throw new NotImplementedException();
        }

        public void MoveTile(int startX, int startY, int targetX, int targetY)
        {
            previouseAction = ((new Point(targetX, targetY), new Point(startX, startY)), Actions.Move);
            throw new NotImplementedException();
        }

        public void Undo()
        {
            if (previouseAction.Item2 == Actions.Move)
            {
                throw new NotImplementedException();
            }

            if (previouseAction.Item2 == Actions.Place)
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSpotFree(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}