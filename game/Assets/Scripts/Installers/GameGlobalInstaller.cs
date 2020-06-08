using UnityEngine;
using Zenject;

public class GameGlobalInstaller : MonoInstaller<GameGlobalInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IUnityObjectProxy>().To<RealUnityObjectProxy>().AsSingle();
        Container.Bind<IUnityDebugProxy>().To<RealUnityDebugProxy>().AsSingle();
    }
}