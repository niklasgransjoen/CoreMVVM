using System.Threading.Tasks;
using Xunit;

namespace CoreMVVM.Threading.Tests
{
    public sealed class RebelTaskTests
    {
        [Fact]
        public async Task RebelTask_Executes()
        {
            await Await1();
            await Await2();

            string res1 = await Await3();
            string res2 = await Await4();

            Assert.Equal("JOIN THE ASCENDENCY!", res1);
            Assert.Equal("WHAT'S UP DANGER", res2);
        }

        private RebelTask Await1()
        {
            return Task.Delay(1);
        }

        private async RebelTask Await2()
        {
            await Task.Yield();
        }

        private RebelTask<string> Await3()
        {
            return RebelTask.FromResult("JOIN THE ASCENDENCY!");
        }

        private async RebelTask<string> Await4()
        {
            await Task.Yield();
            return "WHAT'S UP DANGER";
        }
    }
}