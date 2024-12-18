using Microsoft.VisualStudio.TestTools.UnitTesting;
using LSIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void AddTest()
        {
            var mainWindow = new MainWindow();
            int a = 1;
            int b = 2;
            int expected = 3;

            // Act - 실제 메소드 실행
            int result = mainWindow.Add(a, b);

            // Assert - 예상 결과와 실제 결과 비교
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AddTest2()
        {
            var mainWindow = new MainWindow();
            int a = 2;
            int b = 1;
            int expected = 3;

            // Act - 실제 메소드 실행
            int result = mainWindow.Add(a, b);

            // Assert - 예상 결과와 실제 결과 비교
            Assert.AreEqual(expected, result);
        }
        [TestMethod()]
        public void AddTest3()
        {
            var mainWindow = new MainWindow();
            int a = 2;
            int b = 2;
            int expected = 4;

            // Act - 실제 메소드 실행
            int result = mainWindow.Add(a, b);

            // Assert - 예상 결과와 실제 결과 비교
            Assert.AreEqual(expected, result);
        }
    }
}