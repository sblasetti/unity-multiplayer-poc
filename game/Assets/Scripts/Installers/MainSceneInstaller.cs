using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IUnityObjectProxy>().To<RealUnityObjectProxy>().AsSingle();
        Container.Bind<IUnityDebugProxy>().To<RealUnityDebugProxy>().AsSingle();
        Container.Bind<IPlayersManagementController>().To<PlayersManagementController>().AsSingle();
    }
}