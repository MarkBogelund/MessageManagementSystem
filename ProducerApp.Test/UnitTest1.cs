using NUnit.Framework;
using ProducerApp;
using System.Text;
namespace ProducerApp.Test
{
    public class ProgramTests
    {
        [Test]
        public void MainTestAdd()
        {
            Program program = new Program();
            int result = program.Add(1, 2);
            Assert.That(result, Is.EqualTo(3));
        }
    }
}