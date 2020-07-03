using Assets.Scripts.Builders;
using Assets.Scripts.Commands;
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
        InstallProxies();
        InstallControllers();
        InstallCommands();
        InstallBuilders();
    }

    private void InstallBuilders()
    {
        Container.Bind<IGameEventBuilder>().To<GameEventBuilder>().AsSingle();
    }

    private void InstallCommands()
    {
        Container.Bind<IRotationCommand>().To<RigidbodyRotationCommand>().AsSingle();
        Container.Bind<IMovementCommand>().To<RigidbodyMovementCommand>().AsSingle();
    }

    private void InstallControllers()
    {
        Container.Bind<IPlayersManagementController>().To<PlayersManagementController>().AsSingle();
        Container.Bind<IRemoteMovementController>().To<RemoteMovementController>().AsSingle();
        Container.Bind<ILocalMovementController>().To<LocalMovementController>().AsSingle();
        Container.Bind<INetworkController>().To<NetworkController>().AsSingle();
    }

    private void InstallProxies()
    {
        Container.Bind<IUnityObjectProxy>().To<RealUnityObjectProxy>().AsSingle();
        Container.Bind<IUnityGameObjectProxy>().To<RealUnityGameObjectProxy>().AsSingle();
        Container.Bind<IUnityDebugProxy>().To<RealUnityDebugProxy>().AsSingle();
        Container.Bind<IUnityTimeProxy>().To<RealUnityTimeProxy>().AsSingle();
        Container.Bind<IUnityInputProxy>().To<RealUnityInputProxy>().AsSingle();
        Container.Bind<IUnityPhysicsProxy>().To<RealUnityPhysicsProxy>().AsSingle();
    }
}