using NUnit.Framework;
using ScenesFolders.MainGame;

namespace TestProject1;

public class TwoSidedListTests
{
    [Test]
    public void AddTest()
    {
        var list = new TwoSidedList<int>();
        list.AddRight(0);
        list.AddRight(1);
        list.AddRight(2);
        list.AddLeft(-1);
        list.AddLeft(-2);
        for(var i = -2; i<=2; i++)
            Assert.AreEqual(i, list[i]);
    }
    
    [Test]
    public void SetIndexTest()
    {
        var list = new TwoSidedList<int>();
        list.AddLeft(0);
        Assert.AreEqual(0, list[-1]);
        list[-1] = 1;
        Assert.AreEqual(1, list[-1]);
    }

    [Test]
    public void ForeachTest()
    {
        var list = new TwoSidedList<int>();
        list.AddLeft(-1);
        list.AddRight(0);
        list.AddRight(1);
        var i = -1;
        foreach (var num in list.Values)
        {
            Assert.AreEqual(i, num);
            i++;
        }
    }
}