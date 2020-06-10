using SocketIO;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IUnityObjectProxy>().To<RealUnityObjectProxy>().AsSingle();
        Container.Bind<IUnityGameObjectProxy>().To<RealUnityGameObjectProxy>().AsSingle();
        Container.Bind<IUnityDebugProxy>().To<RealUnityDebugProxy>().AsSingle();
        // var instance = GetComponent<Game>();
        // Assert.IsNotNull(instance);
        // Container.Bind<ISocketIOComponent>().FromInstance(instance).AsSingle();
        Container.Bind<IPlayersManagementController>().To<PlayersManagementController>().AsSingle();
    }
}