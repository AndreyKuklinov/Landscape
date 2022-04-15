using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ScenesFolders.MainGame;

namespace ScenesFolders.MainGame.Testing
{
    [TestFixture]
    public class GameManagerTests
    {
        [Test]
        public void TestGetNeighbours()
        {
            var gm = new GameManager();
            gm.MakeTurn(1, 0, TileTypes.Mountain);
            gm.MakeTurn(2, 1, TileTypes.Lake);
            gm.MakeTurn(0, 1, TileTypes.Village);
            gm.MakeTurn(1, 2, TileTypes.Forest);
            Assert.AreEqual(4, gm.GetNeighbours(1, 1).ToArray().Length);
            Assert.AreEqual(2, gm.GetNeighbours(4, 4).ToArray().Length);
        }

        [Test]
        public void TestCountAdjacentOfType()
        {
            var gm = new GameManager();
            gm.MakeTurn(1, 0, TileTypes.Mountain);
            gm.MakeTurn(2, 1, TileTypes.Mountain);
            gm.MakeTurn(0, 1, TileTypes.Forest);
            Assert.AreEqual(2, gm.CountAdjacentOfType(1, 1, TileTypes.Mountain));
            Assert.AreEqual(1, gm.CountAdjacentOfType(1, 1, TileTypes.Forest));
            Assert.AreEqual(0, gm.CountAdjacentOfType(1, 1, TileTypes.Village));
        }

        [Test]
        public void TestGetTileAt()
        {
            var gm = new GameManager();
            gm.MakeTurn(1, 1, TileTypes.Forest);
            Assert.AreEqual(gm.GetTileAt(1, 1), new Tile(TileTypes.Forest));
            Assert.AreEqual(gm.GetTileAt(5, 5), new Tile());
        }
    }
}