using System.Threading;
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

            var res1 = await Await3();
            var res2 = await Await4();

            Assert.Equal("JOIN THE ASCENDENCY!", res1);
            Assert.Equal("WHAT'S UP DANGER", res2);
        }

        [Fact]
        public async Task RebelTask_Awaits_CompleteTask()
        {
            var threadID = Thread.CurrentThread.ManagedThreadId;

            await RebelTask.CompletedTask;

            var currentThreadID = Thread.CurrentThread.ManagedThreadId;
            Assert.Equal(threadID, currentThreadID);
        }

        [Fact]
        public async Task RebelTask_Returns_WrappedResult()
        {
            var strExp = "JOIN THE ASCENDENCY!";
            var strRes = await RebelTask.FromResult(strExp);

            Assert.Equal(strExp, strRes);

            var boolExp = true;
            var boolRes = await RebelTask.FromResult(boolExp);

            Assert.Equal(boolExp, boolRes);
        }

        [Fact]
        public async Task RebelTask_Returns_Result()
        {
            var strExp = "JOIN THE ASCENDENCY!";
            var strRes = await new RebelTask<string>(Task.Run(() => strExp));

            Assert.Equal(strExp, strRes);

            var boolExp = true;
            var boolRes = await new RebelTask<bool>(Task.Run(() => boolExp));

            Assert.Equal(boolExp, boolRes);
        }

        [Fact]
        public async Task RebelTask_Supports_Default()
        {
            var task = default(RebelTask);
            await task;
        }

        [Fact]
        public async Task GenericRebelTask_Supports_Default()
        {
            var task = default(RebelTask<int>);
            var result = await task;

            Assert.Equal(0, result);
        }

        #region Tools

        private static RebelTask Await1()
        {
            return Task.Delay(1);
        }

        private static async RebelTask Await2()
        {
            await Task.Yield();
        }

        private static RebelTask<string> Await3()
        {
            return RebelTask.FromResult("JOIN THE ASCENDENCY!");
        }

        private static async RebelTask<string> Await4()
        {
            await Task.Yield();
            return "WHAT'S UP DANGER";
        }

        #endregion Tools
    }
}