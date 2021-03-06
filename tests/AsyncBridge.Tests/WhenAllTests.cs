using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET45
using TaskEx = System.Threading.Tasks.Task;
#endif

#if NET45
namespace ReferenceAsync.Tests
#elif ATP
namespace AsyncTargetingPack.Tests
#else
namespace AsyncBridge.Tests
#endif
{
    [TestClass]
    public class WhenAllTests
    {
        [TestMethod]
        public void GenericIEnumerableWithSomeContents()
        {
            TestUtils.RunAsync(async () =>
            {
                var taskCompletionSources = new[]
                {
                    new TaskCompletionSource<int>(),
                    new TaskCompletionSource<int>(),
                    new TaskCompletionSource<int>()
                };
                var whenAllTask = TaskEx.WhenAll(taskCompletionSources.Select(tcs => tcs.Task));
                taskCompletionSources[0].SetResult(1);
                taskCompletionSources[1].SetResult(2);
                taskCompletionSources[2].SetResult(3);
                var results = await whenAllTask;
                CollectionAssert.AreEquivalent(new[] {1, 2, 3}, results.ToList());
            });
        }

        [TestMethod]
        public void DontCompleteOne()
        {
            var taskCompletionSources = new []
                                        {
                                            new TaskCompletionSource<int>(),
                                            new TaskCompletionSource<int>()
                                        };
            var whenAllTask = TaskEx.WhenAll(taskCompletionSources.Select(tcs => tcs.Task));
            taskCompletionSources[0].SetResult(1);
            Assert.IsFalse(whenAllTask.IsCompleted);
        }

        [TestMethod]
        public void NonGenericVersion()
        {
            TestUtils.RunAsync(async () =>
            {
                var taskCompletionSources = new[]
                {
                    new TaskCompletionSource<int>(),
                    new TaskCompletionSource<int>()
                };
                var whenAllTask = TaskEx.WhenAll(taskCompletionSources.Select(tcs => tcs.Task).Cast<Task>());
                taskCompletionSources[0].SetResult(1);
                Assert.IsFalse(whenAllTask.IsCompleted);
                taskCompletionSources[1].SetResult(1);
                await whenAllTask;
            });
        }

        [TestMethod]
        public void ArrayVersion()
        {
            TestUtils.RunAsync(async () =>
            {
                var taskCompletionSources = new[]
                {
                    new TaskCompletionSource<int>(),
                    new TaskCompletionSource<int>()
                };
                Task whenAllTask = TaskEx.WhenAll(taskCompletionSources.Select(tcs => tcs.Task).ToArray());
                taskCompletionSources[0].SetResult(1);
                Assert.IsFalse(whenAllTask.IsCompleted);
                taskCompletionSources[1].SetResult(1);
                await whenAllTask;
            });
        }
    }
}
