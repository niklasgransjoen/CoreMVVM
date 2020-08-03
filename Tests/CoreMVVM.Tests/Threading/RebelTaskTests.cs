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

            string res1 = await Await3();
            string res2 = await Await4();

            Assert.Equal("JOIN THE ASCENDENCY!", res1);
            Assert.Equal("WHAT'S UP DANGER", res2);
        }

        [Fact]
        public async Task RebelTask_Awaits_CompleteTask()
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;

            await RebelTask.CompletedTask;

            int currentThreadID = Thread.CurrentThread.ManagedThreadId;
            Assert.Equal(threadID, currentThreadID);
        }

        [Fact]
        public async Task RebelTask_Returns_WrappedResult()
        {
            string strExp = "JOIN THE ASCENDENCY!";
            string strRes = await RebelTask.FromResult(strExp);

            Assert.Equal(strExp, strRes);

            bool boolExp = true;
            bool boolRes = await RebelTask.FromResult(boolExp);

            Assert.Equal(boolExp, boolRes);
        }

        [Fact]
        public async Task RebelTask_Returns_Result()
        {
            string strExp = "JOIN THE ASCENDENCY!";
            string strRes = await new RebelTask<string>(Task.Run(() => strExp));

            Assert.Equal(strExp, strRes);

            bool boolExp = true;
            bool boolRes = await new RebelTask<bool>(Task.Run(() => boolExp));

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
            int result = await task;

            Assert.Equal(0, result);
        }

        #region Tools

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

        #endregion Tools
    }
}