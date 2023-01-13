using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiarrhoeaEngine.ShitNet
{
    public class Network
    {
        private string command = string.Empty;
        public const int PACKET_SIZE = 1024;

        public void SetCommand(string command)
        {
            this.command = command;
        }

        public async Task<string> GetCommand()
        {
            while (command == "" || command == string.Empty || command == null) { }

            string result = command;
            command = string.Empty;

            return result;
        }

        public async Task<byte[]> ReadStream(NetworkStream stream)
        {
            try
            {
                List<byte> finalResult = new List<byte>();
                //CancellationTokenSource src = new CancellationTokenSource(500000);

                try
                {
                    while (true)
                    {
                        int length = 0;
                        byte[] buffer = new byte[1024];

                        try
                        {
                            length = await stream.ReadAsync(buffer, 0, buffer.Length);
                        }
                        catch (IOException e)
                        {
                            return new byte[0];
                        }

                        if (length == 0 ) break;

                        byte[] received = new byte[length];

                        Array.Copy(buffer, received, length);
                        Array.Clear(buffer, 0, buffer.Length);

                        finalResult.AddRange(received);
                        Array.Clear(received, 0, received.Length);

                        if (length < PACKET_SIZE) break;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    return finalResult.ToArray();
                }
                catch
                {
                    finalResult.Clear();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    return new byte[0];
                }
            }
            catch
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return new byte[0];
            }
        }

        public async Task SendMessage(NetworkStream stream, byte[] data)
        {
            try
            {
                if (data.Length <= PACKET_SIZE)
                {
                    Task task = stream.WriteAsync(data, 0, data.Length);
                    await task;

                    while (task.Status != TaskStatus.RanToCompletion) { Console.WriteLine("Writing..."); }

                    await stream.FlushAsync();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    return;
                }
                else
                {
                    int rounds = data.Length / PACKET_SIZE;
                    int total = 0;

                    for (int i = 0; i <= rounds; i++)
                    {
                        int length = PACKET_SIZE;
                        if (rounds == i)
                            length = data.Length - (PACKET_SIZE * i);

                        total += length;

                        Task task = stream.WriteAsync(data, PACKET_SIZE * i, length);
                        await task;
                        while (task.Status != TaskStatus.RanToCompletion) { Console.WriteLine("Writing..."); }

                        await stream.FlushAsync();
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            int count = this.GetType().GetFields().Length;

            Console.WriteLine(this.GetType().ToString());
            Console.WriteLine("Amount of variables: " + count);

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(this.GetType().GetFields().GetValue(i).ToString());
                this.GetType().GetFields().SetValue(null, 0);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public class Server : Network
    {
        private TcpListener server;
        public List<NetID> clients;
        
        public NetID GetNetID(object value, GetNetIDType type)
        {
            switch(type)
            {
                case GetNetIDType.ID:
                    {
                        return clients.Find(x => x.id == (string)value);
                    }
                case GetNetIDType.USERNAME:
                    {
                        return clients.Find(x => x.username == (string)value);
                    }
                case GetNetIDType.CLIENT:
                    {
                        return clients.Find(x => x.client == (TcpClient)value);
                    }
            }

            return null;
        }

        public List<TcpClient> GetClients()
        {
            List<TcpClient> result = new List<TcpClient>();
            clients.ForEach(x => result.Add(x.client));
            return result;
        }

        public Server(int port, Action executer)
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                clients = new List<NetID>();

                server.Server.ReceiveBufferSize = PACKET_SIZE;
                server.Server.SendBufferSize = PACKET_SIZE;

                server.Start();

                ThreadStart accept = new ThreadStart(AcceptClient);
                Thread acceptThread = new Thread(accept);
                acceptThread.Start();

                ThreadStart execute = new ThreadStart(executer);
                Thread executeThread = new Thread(execute);
                executeThread.Start();
            }
            catch
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private async void AcceptClient()
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            NetID id = new NetID(Guid.NewGuid().ToString("N"));
            id.client = client;

            Console.WriteLine("New Connection Created: " + client.Client.LocalEndPoint + " with ID: " + id.id);
            clients?.Add(id);

            string setID = $"SET_ID(ID: {id.id})";
            await SendTo(setID, client);

            string introduce = $"TXT(MSG: '{id.id} HAS CONNECTED', FROM: SERVER)";
            await SendToAllBut(introduce, id);

            AcceptMessage(client);
            AcceptClient();
        }


        public async void AcceptMessage(TcpClient client)
        {
            byte[] data = new byte[0];
            try { data = await ReadStream(client.GetStream()); } catch (ObjectDisposedException e) { Console.WriteLine(e.Message); return; };
            if (data.Length == 0) { AcceptMessage(client); }
            else
            {
                string msg = Encoding.UTF8.GetString(data);
                ShitNetCore.ConvertNetMSG(msg, GetNetID(client, GetNetIDType.CLIENT));

                GC.Collect();
                GC.WaitForPendingFinalizers();

                AcceptMessage(client);
            }
        }

        public async Task SendTo(string msg, TcpClient client)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            await SendMessage(client.GetStream(), data);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task SendTo(string msg, NetID id)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);

            foreach (NetID client in clients)
            {
                if (client.id == id.id)
                    await SendMessage(client.client.GetStream(), data);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task SendToAll(string msg)
        {
            foreach (TcpClient client in GetClients())
            {
                await SendTo(msg, client);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public async Task SendToAllBut(string msg, NetID id)
        {
            foreach (NetID client in clients)
            {
                if (client.id != id.id)
                    await SendTo(msg, client.client);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public class Client : Network
    {
        public NetID id;
        public async Task<NetID> GetID()
        {
            while (id == null) { }
            while (id.id == string.Empty || id.id == "") { }

            return id;
        }

        private TcpClient client;
        public TcpClient GetClient()
        {
            return client;
        }
        public NetworkStream GetStream()
        {
            return client.GetStream();
        }

        public string address;

        public Client(string address, int port, Action executer)
        {
            try
            {
                client = new TcpClient();

                client.ReceiveBufferSize = PACKET_SIZE;
                client.SendBufferSize = PACKET_SIZE;
                client.ExclusiveAddressUse= true;

                Connect(address, port);
                
                this.address = address + ":" + port;

                ThreadStart accept = new ThreadStart(AcceptMessage);
                Thread acceptThread = new Thread(accept);
                acceptThread.Start();

                ThreadStart execute = new ThreadStart(executer);
                Thread executeThread = new Thread(execute);
                executeThread.Start();
            }
            catch (Exception e)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private async void Connect(string address, int port)
        {
            await client.ConnectAsync(address, port);
        }

        public async void AcceptMessage()
        {
            try
            {
                byte[] data = await ReadStream(GetStream());
                string msg = Encoding.UTF8.GetString(data);

                ShitNetCore.ConvertNetMSG(msg, null);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                AcceptMessage();
            }
            catch
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public async Task Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            await SendMessage(GetStream(), data);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void OnProcessExit(object sender, EventArgs e)
        {
            Disconnect();
        }

        public void Disconnect()
        {
            BeginDisconnect().GetAwaiter().GetResult();
        }

        public void Kick()
        {
            client.Close();
            client.Dispose();
        }

        private async Task BeginDisconnect()
        {
            await Send($"DISCONNECT(ID: {id.id})");// new DisconnectMsg(id));

            client.Close();
            client.Dispose();

            Environment.Exit(0);
        }
    }
}