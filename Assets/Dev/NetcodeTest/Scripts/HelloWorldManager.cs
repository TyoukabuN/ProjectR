using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace PJR.Dev.Lab.NetcodeTest
{
    public class HelloWorldManager : MonoBehaviour
    {
        private NetworkManager m_NetworkManager;
        public NetworkManager NetworkManager => m_NetworkManager ??= GetComponent<NetworkManager>();
        void Awake()
        {
            CheckValid();
        }

        private bool CheckValid()
        {
            m_NetworkManager ??= GetComponent<NetworkManager>();
            return m_NetworkManager != null;
        }
        
        void Update()
        {
            if (m_NetworkManager?.IsHost ?? false)
            {
                if (NetworkManager.Singleton != null)
                {
                    Debug.Log(
                        $"Listening={NetworkManager.Singleton.IsListening}, " +
                        $"ConnectedClients={NetworkManager.Singleton.ConnectedClients.Count}"
                    );
                }
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (GUILayout.Button("Host"))
            {
                RegisterNonClientNetworkManagerCallback();
                m_NetworkManager.StartHost();
            }
            if (GUILayout.Button("Client")) 
                m_NetworkManager.StartClient();
            if (GUILayout.Button("Server"))
            {
                RegisterNonClientNetworkManagerCallback();
                m_NetworkManager.StartServer();
            }
        }

        void RegisterNonClientNetworkManagerCallback()
        {
            m_NetworkManager.ConnectionApprovalCallback = OnConnectionApprovalCallback;
        }
        void OnConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }
        bool OnNetworkObjectOnValidatableCallback()
        {
            return !ParrelSync.ClonesManager.IsClone();
        }

        void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ?
                "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        void SubmitNewPosition()
        {
            if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
            {
                if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient )
                {
                    foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
                        m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                else
                {
                    var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
    
    static class TestStaticSetup
    {
        [InitializeOnLoadMethod]
        private static void OnInitializeOnLoadMethod()
        { 
            if (ParrelSync.ClonesManager.IsClone())
            {
                NetworkObject.BAN_NETWORKOBJECT_VALIDATE = true;
                Debug.LogWarning(
                    $"setting {nameof(NetworkObject.BAN_NETWORKOBJECT_VALIDATE)} to true in a clone project ");
            }
        }
    }
}
