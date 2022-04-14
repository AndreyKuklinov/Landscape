using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ScenesFolders.MainGame;

namespace ScenesFolders.MainGame.Testing
{
    [TestFixture]
    public class GameManagerTests
    {
        [Test]
        public void TestPossibleTiles()
        {
            var gm = GameManager;
        }

        [Test]
        public void TestGetNeighbours()
        {
            var gm = GameManager;
            Assert.AreEqual(gm.GetNeighbours(1, 1), GameBoard[1, 1]);
        }

        [Test]
        public void TestCountAdjacentOfType(int x, int y)
        {
            return x + y;
        }

        [Test]
        public void TestGetTileAt()
        {
            var gm = new GameManager();
            Assert.AreEqual(gm.GetTileAt(1, 1), GameBoard[1, 1]);
            Assert.AreEqual(gm.GetTileAt(5, 1), Tile);
            Assert.AreEqual(gm.GetTileAt(1, -1), Tile) ;

        }
    }
}
