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
            //Инициализируем GameManager
            var objectives = new[] {new Objective(Criteria.Deserts)};
            var gm = new GameManager(objectives);
            
            //Размещаем клетки по полю
            gm.MakeTurn(0, 0, TileTypes.Plain);
            gm.MakeTurn(0, 2, TileTypes.Plain);
            gm.MakeTurn(0, 3, TileTypes.Plain);
            gm.MakeTurn(1, 2, TileTypes.Lake);
            gm.MakeTurn(1, 3, TileTypes.Forest);
            gm.MakeTurn(3, 0, TileTypes.Plain);

            //Проверяем, что число очков равно числу зелёных квадратиков
            Assert.AreEqual(2, gm.Score);
        }
        
        [Test]
        public void TestCountAdjacentOfType()
        {
            throw new NotImplementedException();
        }
    }
}