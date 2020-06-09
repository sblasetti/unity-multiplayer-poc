using NUnit.Framework;
using Zenject;
using Moq;

namespace Tests
{
    [TestFixture]
    public class PlayersManagementControllerTest : ZenjectUnitTestFixture
    {
        private IPlayersManagementController controller;
        private Mock<IUnityObjectProxy> unityObjectProxy = new Mock<IUnityObjectProxy>();
        private Mock<IUnityDebugProxy> unityDebugProxy = new Mock<IUnityDebugProxy>();

        [SetUp]
        public void Init()
        {
            controller = new PlayersManagementController(unityObjectProxy.Object, unityDebugProxy.Object);
        }

        [Test]
        public void RunTest1()
        {
            // Given
            // When
            // Then
            Assert.AreEqual(1, 1);
        }
    }

}
