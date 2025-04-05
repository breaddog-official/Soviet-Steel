using Mirror;
using Mirror.Discovery;
using Scripts.Gameplay;
using System;
using System.Linq;
using System.Net;
using static Scripts.Network.ServerDiscovery;

namespace Scripts.Network
{
    public class ServerDiscovery : NetworkDiscoveryBase<ClientRequest, Response>
    {

        public string ServerName { get; protected set; } = "noname";
        public string ClientName { get; protected set; } = "noname";


        public static ServerDiscovery Instance { get; protected set; }


        private void Awake()
        {
            Instance = this;
        }


        #region Password

        /*[SerializeField] protected EncryptionTransport encryption;
        public string Password { get; protected set; }

        private void OnEnable()
        {
            encryption.onClientValidateServerPubKey += ServerValidatePassword;
        }

        private void OnDisable()
        {
            encryption.onClientValidateServerPubKey -= ServerValidatePassword;
        }


        public bool ServerValidatePassword(PubKeyInfo key)
        {
            print($"Password: {Password};  Serialized Key: {key.Serialized.ToString()};  {Password == key.Serialized.ToString()}");
            return Password == key.Serialized.ToString();
        }*/

        #endregion



        #region Server Discovery

        protected override void ProcessClientRequest(ClientRequest request, IPEndPoint endpoint)
        {
            print("processing request from client");
            base.ProcessClientRequest(request, endpoint);
        }

        protected override Response ProcessRequest(ClientRequest request, IPEndPoint endpoint)
        {
            print("sending response to client");
            return new Response
            {
                name = ServerName,
                mapHash = GameManager.GameMode.map.MapHash,

                uri = transport.ServerUri(),

                playersCount = NetworkServer.connections.Count,
                maxPlayers = NetworkServer.maxConnections,

                supportedCarsHashes = NetworkManagerExt.instance.registeredCars.Select(c => c.car.CarHash).ToArray(),

                serverId = ServerId
            };
        }

        #endregion

        #region Client Discovery

        protected override ClientRequest GetRequest()
        {
            print("sending request to server");
            return new ClientRequest
            {
                name = ClientName
            };
        }

        protected override void ProcessResponse(Response response, IPEndPoint endpoint)
        {
            print("processing response from server");
            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.uri)
            {
                Host = endpoint.Address.ToString()
            };
            response.uri = realUri.Uri;

            OnServerFound.Invoke(response);
        }

        #endregion


        public void SetServerName(string name) => ServerName = name;
        public void SetClientName(string name) => ClientName = name;



        public struct ClientRequest : NetworkMessage
        {
            public string name;
        }

        public struct Response : NetworkMessage
        {
            public string name;
            public string mapHash;

            public Uri uri;

            public int playersCount;
            public int maxPlayers;

            public string[] supportedCarsHashes;

            public long serverId;
        }
    }
}