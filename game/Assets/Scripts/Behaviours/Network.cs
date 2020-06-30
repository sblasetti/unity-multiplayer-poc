using Assets.Scripts.Controllers;
using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Network : MonoBehaviour
{
    [Inject]
    private INetworkController controller;

    private void Awake()
    {
        controller.SetSocket(GetComponent<ISocketIOComponent>());
    }
}
