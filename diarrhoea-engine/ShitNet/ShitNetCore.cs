using DiarrhoeaEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiarrhoeaEngine.ShitNet
{
    public static class ShitNetCore
    {
        public static Action<NetMSGType, string> onReceiveMSG;

        public static Server CreateServer(int PORT, Action Executor)
        {
            return new Server(PORT, Executor);
        }

        public static Client CreateClient(string IP, int PORT, Action Executor)
        {
            return new Client(IP, PORT, Executor);
        }

        public static void ConvertNetMSG(string content, NetID sender)
        {
            Regex func = new Regex("[a-z-A-Z-_]+[^(]");
            Match f_match = func.Match(content);

            if (!Enum.TryParse(typeof(NetMSGType), f_match.Value, out object res)) return;
            onReceiveMSG?.Invoke((NetMSGType)res, content.Split(f_match.Value)[1]);
        }

        public static string GetContentValue(string content, string value)
        {
            Console.WriteLine(content + " -- "+value);
            return content.Split(value + ": ")[1].Split(new char[] { '|', ')' })[0];
        }
    }

    public enum NetMSGType
    {
        SPAWN,
        MOVE,
        RECENTER,
        ROTATE,
        TXT,
        SET_ID,
        SET_USERNAME
    }

    public class NetID
    {
        public string id { get; private set; }
        public string GetShortID(int start = 0, int end = 6)
        {
            return id.Substring(start, end);
        }

        public string username = string.Empty;
        public TcpClient? client = null;

        public NetID(string id)
        {
            this.id = id;
        }
    }

    public enum GetNetIDType
    {
        ID,
        USERNAME,
        CLIENT
    }

}