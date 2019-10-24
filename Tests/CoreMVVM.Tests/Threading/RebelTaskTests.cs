using System.Threading.Tasks;
using Xunit;

namespace CoreMVVM.Threading.Tests
{
    public sealed class RebelTaskTests
    {
        [Fact]
        public async RebelTask RebelTask_Executes()
        {
            await Task.Delay(100);
        }
    }
}