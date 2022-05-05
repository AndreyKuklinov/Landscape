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

        [Test]
        public void TestMeadows()
        {
            var objectives = new[] { new Objective(Criteria.Meadows) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Forest);
            gm.MakeTurn(0, 2, TileTypes.Plain);
            gm.MakeTurn(0, 3, TileTypes.Forest);
            gm.MakeTurn(1, 3, TileTypes.Plain);
            gm.MakeTurn(3, 0, TileTypes.Forest);
            gm.MakeTurn(3, 1, TileTypes.Plain);
            gm.MakeTurn(3, 2, TileTypes.Forest);

            Assert.AreEqual(2, gm.Score);
        }
        [Test]
        public void TestCliffs()
        {
            var objectives = new[] { new Objective(Criteria.Cliffs) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 2, TileTypes.Mountain);
            gm.MakeTurn(0, 3, TileTypes.Mountain);
            gm.MakeTurn(0, 4, TileTypes.Lake);
            gm.MakeTurn(1, 3, TileTypes.Plain);
            gm.MakeTurn(3, 1, TileTypes.Forest);
            gm.MakeTurn(4, 0, TileTypes.Lake);
            gm.MakeTurn(4, 1, TileTypes.Mountain);
            gm.MakeTurn(4, 2, TileTypes.Plain);
            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestFoothils()
        {
            var objectives = new[] { new Objective(Criteria.Foothills) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(2, 0, TileTypes.Plain);
            gm.MakeTurn(3, 0, TileTypes.Mountain);
            gm.MakeTurn(4, 0, TileTypes.Mountain);
            gm.MakeTurn(3, 1, TileTypes.Lake);
            gm.MakeTurn(4, 1, TileTypes.Mountain);
            gm.MakeTurn(0, 4, TileTypes.Lake);
            gm.MakeTurn(1, 4, TileTypes.Mountain);
            gm.MakeTurn(2, 4, TileTypes.Lake);

            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestPonds()
        {
            var objectives = new[] { new Objective(Criteria.Ponds) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 0, TileTypes.Forest);
            gm.MakeTurn(0, 0, TileTypes.Lake);
            gm.MakeTurn(0, 1, TileTypes.Forest);


            Assert.AreEqual(1,gm.Score);
        }

        [Test]
        public void TestHollows()
        {
            var objectives = new[] { new Objective(Criteria.Hollows) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Mountain);
            gm.MakeTurn(0, 2, TileTypes.Mountain);

            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestExpanses()
        {
            var objectives = new[] { new Objective(Criteria.Expanses) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 2, TileTypes.Lake);
            gm.MakeTurn(0, 1, TileTypes.Plain);
            gm.MakeTurn(1, 2, TileTypes.Plain);
            gm.MakeTurn(0, 3, TileTypes.Plain);
            gm.MakeTurn(4, 0, TileTypes.Forest);
            gm.MakeTurn(4, 1, TileTypes.Plain);
            gm.MakeTurn(3, 0, TileTypes.Plain);
            gm.MakeTurn(3, 1, TileTypes.Mountain);

            Assert.AreEqual(3, gm.Score);
        }

        [Test]
        public void TestIslands()
        {
            var objectives = new[] { new Objective(Criteria.Islands) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 4, TileTypes.Forest);
            gm.MakeTurn(0, 3, TileTypes.Lake);
            gm.MakeTurn(1, 4, TileTypes.Lake);
            gm.MakeTurn(3, 0, TileTypes.Lake);
            gm.MakeTurn(4, 0, TileTypes.Lake);
            gm.MakeTurn(4, 3, TileTypes.Plain);
            gm.MakeTurn(3, 3, TileTypes.Lake);
            gm.MakeTurn(4, 4, TileTypes.Lake);
            gm.MakeTurn(4, 2, TileTypes.Lake);
            Assert.AreEqual(2, gm.Score);
        }

        [Test]
        public void TestBridges()
        {
            var objectives = new[] { new Objective(Criteria.Bridges) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 3, TileTypes.Village);
            gm.MakeTurn(4, 3, TileTypes.Village);
            gm.MakeTurn(1, 3, TileTypes.Lake);
            gm.MakeTurn(4, 0, TileTypes.Village);
            gm.MakeTurn(4, 1, TileTypes.Lake);
            gm.MakeTurn(2, 3, TileTypes.Mountain);
            gm.MakeTurn(4, 2, TileTypes.Forest);
            gm.MakeTurn(0, 0, TileTypes.Lake);
            Assert.AreEqual(2, gm.Score);
        }

        [Test]
        public void TestBridges1()
        {
            var objectives = new[] { new Objective(Criteria.Bridges) };
            var gm = new GameManager(objectives);
            gm.MakeTurn(0, 2, TileTypes.Village);
            gm.MakeTurn(4, 2, TileTypes.Village);
            gm.MakeTurn(2, 0, TileTypes.Village);
            gm.MakeTurn(2, 4, TileTypes.Village);
            gm.MakeTurn(2, 2, TileTypes.Lake);
            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestParks()
        {
            var objectives = new[] { new Objective(Criteria.Parks) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 0, TileTypes.Village);
            gm.MakeTurn(1, 1, TileTypes.Forest);
            gm.MakeTurn(1, 3, TileTypes.Village);
            gm.MakeTurn(2, 2, TileTypes.Forest);
            gm.MakeTurn(2, 3, TileTypes.Forest);
            gm.MakeTurn(3, 0, TileTypes.Forest);
            gm.MakeTurn(3, 1, TileTypes.Forest);

            Assert.AreEqual(2, gm.Score);
        }

        [Test]
        public void TestFarmlands()
        {
            var objectives = new[] { new Objective(Criteria.Farmlands) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(1, 1, TileTypes.Plain);
            gm.MakeTurn(1, 2, TileTypes.Plain);
            gm.MakeTurn(2, 1, TileTypes.Plain);
            gm.MakeTurn(2, 2, TileTypes.Village);
            gm.MakeTurn(2, 3, TileTypes.Plain);
            gm.MakeTurn(3, 2, TileTypes.Plain);
            gm.MakeTurn(3, 3, TileTypes.Village);

            Assert.AreEqual(4, gm.Score);
        }

        [Test]
        public void TestMines()
        {
            var objectives = new[] { new Objective(Criteria.Mines) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Village);
            gm.MakeTurn(0, 3, TileTypes.Mountain);
            gm.MakeTurn(1, 1, TileTypes.Village);
            gm.MakeTurn(2, 1, TileTypes.Mountain);
            gm.MakeTurn(2, 2, TileTypes.Village);
            gm.MakeTurn(3, 0, TileTypes.Village);
            gm.MakeTurn(3, 3, TileTypes.Village);
            gm.MakeTurn(3, 1, TileTypes.Plain);

            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestCrossroads()
        {
            var objectives = new[] { new Objective(Criteria.Crossroads) };
            var gm = new GameManager(objectives);
            gm.MakeTurn(0, 2, TileTypes.Village);
            gm.MakeTurn(4, 2, TileTypes.Village);
            gm.MakeTurn(2, 0,TileTypes.Village);
            gm.MakeTurn(2, 4, TileTypes.Village);

            Assert.AreEqual(gm.Score, 1);
        }

        [Test]
        public void TestTowns()
        {
            var objectives = new[] { new Objective(Criteria.Towns) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Village);
            gm.MakeTurn(0, 2, TileTypes.Village);
            gm.MakeTurn(0, 3, TileTypes.Village);
            gm.MakeTurn(2, 0, TileTypes.Village);
            gm.MakeTurn(3, 0, TileTypes.Village);
            gm.MakeTurn(3, 1, TileTypes.Village);

            Assert.AreEqual(5, gm.Score);
        }

        [Test]
        public void TestCastles()
        {
            var objectives = new[] { new Objective(Criteria.Castles)};
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 0, TileTypes.Village);
            gm.MakeTurn(0, 2, TileTypes.Village);
            gm.MakeTurn(0, 3, TileTypes.Village);
            gm.MakeTurn(2, 0, TileTypes.Village);
            gm.MakeTurn(3, 0, TileTypes.Village);
            gm.MakeTurn(3, 1, TileTypes.Village);

            Assert.AreEqual(1, gm.Score);
        }

        [Test]
        public void TestStations()
        {
            var objectives = new[] { new Objective(Criteria.Stations) };
            var gm = new GameManager(objectives);

            gm.MakeTurn(0, 3, TileTypes.Village);
            gm.MakeTurn(1, 0, TileTypes.Village);
            gm.MakeTurn(3, 0, TileTypes.Village);
            gm.MakeTurn(3, 1, TileTypes.Village);

            Assert.AreEqual(1, gm.Score);
        }
    }
}