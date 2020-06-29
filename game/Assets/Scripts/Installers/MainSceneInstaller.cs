using Assets.Scripts.Controllers;
using Assets.Scripts.Proxies;
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
        Container.Bind<IUnityTimeProxy>().To<RealUnityTimeProxy>().AsSingle();
        Container.Bind<IUnityInputProxy>().To<RealUnityInputProxy>().AsSingle();
        Container.Bind<IPlayersManagementController>().To<PlayersManagementController>().AsSingle();
        Container.Bind<IRemoteMovementController>().To<RemoteMovementController>().AsSingle();
        Container.Bind<ILocalMovementController>().To<LocalMovementController>().AsSingle();
        Container.Bind<INetworkController>().To<NetworkController>().AsSingle();
    }
}