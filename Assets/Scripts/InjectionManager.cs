using UnityEngine;
using Zenject;

public class InjectionManager : MonoInstaller
{
    private MatrixContainer matrixContainer = new();
    public override void InstallBindings()
    {
        Container.BindInstance(matrixContainer).AsSingle();
    }
}
