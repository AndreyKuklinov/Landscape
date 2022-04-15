using ScenesFolders.MainGame;
using NUnit.Framework;
using System;

namespace ScenesFolders.MainGame.Testing
{
    [TestFixture]
    public class CriteriaTests
    {
        [Test]
        public void TestDeserts()
        {
            var objectives = new[] {new Objective(Criteria.Deserts)};
            var gm = new GameManager(objectives);
            
            gm.MakeTurn(0, 0, TileTypes.Plain);
            gm.MakeTurn(0, 2, TileTypes.Plain);
            gm.MakeTurn(0, 3, TileTypes.Plain);
            gm.MakeTurn(1, 2, TileTypes.Lake);
            gm.MakeTurn(1, 3, TileTypes.Forest);
            gm.MakeTurn(3, 0, TileTypes.Plain);
            
            Assert.AreEqual(2, gm.Score);
        }

        [Test]
        public void TestTwins()
        {
            var objectives = new[] { new Objective(Criteria.Twins) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Mountain);
            gm.MakeTurn(0, 2, TileTypes.Mountain);
            gm.MakeTurn(0, 3, TileTypes.Mountain);
            gm.MakeTurn(1, 3, TileTypes.Mountain);
            gm.MakeTurn(3, 1, TileTypes.Mountain);
            gm.MakeTurn(3, 2, TileTypes.Mountain);

            Assert.AreEqual(4, gm.Score);
        }

        [Test]
        public void TestSwamps()
        {
            var objectives = new[] { new Objective(Criteria.Swamps) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 1, TileTypes.Forest);
            gm.MakeTurn(1, 2, TileTypes.Forest);
            gm.MakeTurn(2, 1, TileTypes.Forest);
            gm.MakeTurn(2, 2, TileTypes.Lake);
            gm.MakeTurn(2, 3, TileTypes.Forest);
            gm.MakeTurn(3, 2, TileTypes.Forest);
            gm.MakeTurn(3, 3, TileTypes.Lake);

            Assert.AreEqual(4, gm.Score);
        }

        [Test]
        public void TestFields()
        {
            var objectives = new[] { new Objective(Criteria.Fields) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 0, TileTypes.Plain);
            gm.MakeTurn(1, 1, TileTypes.Plain);
            gm.MakeTurn(1, 2, TileTypes.Plain);
            gm.MakeTurn(2, 3, TileTypes.Lake);
            gm.MakeTurn(3, 0, TileTypes.Plain);

            Assert.AreEqual(3, gm.Score);
        }

        [Test]
        public void TestGroves()
        {
            var objectives = new[] { new Objective(Criteria.Groves) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 1, TileTypes.Forest);
            gm.MakeTurn(1, 2, TileTypes.Forest);
            gm.MakeTurn(2, 3, TileTypes.Forest);
            gm.MakeTurn(3, 0, TileTypes.Forest);

            Assert.AreEqual(2, gm.Score);
        }
    }
}