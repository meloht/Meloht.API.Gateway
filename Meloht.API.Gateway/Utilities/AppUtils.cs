using Meloht.API.Gateway.LoadBalancing;
using Meloht.API.Gateway.ServerProviders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Meloht.API.Gateway.Utilities
{
    internal static class AppUtils
    {
        public static int ConvertInt(string s, int defaultValue = 0)
        {
            int num = 0;
            if (int.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }
        public static long ConvertLongInt(string s, long defaultValue = 0)
        {
            long num = 0;
            if (long.TryParse(s, out num))
            {
                return num;
            }
            return defaultValue;
        }

        public static string CountSize(long size)
        {
            string mStrSize = "";
            long factSize = 0;
            factSize = size;
            if (factSize < 1024.00)
                mStrSize = factSize.ToString("F2") + " Byte";
            else if (factSize >= 1024.00 && factSize < 1048576)
                mStrSize = (factSize / 1024.00).ToString("F2") + " K";
            else if (factSize >= 1048576 && factSize < 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (factSize >= 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return mStrSize;
        }

        public static List<ServerNode> UpdateData(List<ServerNodeConfig> servers, Dictionary<string, ServerNode> serversDict)
        {
            HashSet<string> updatedAddresses = new HashSet<string>();
            List<ServerNode> serversList = new List<ServerNode>();
            foreach (var item in servers)
            {
                string address = GetAddress(item.Host, item.Port);
                updatedAddresses.Add(address);

                if (serversDict.TryGetValue(address, out var node))
                {
                    node.Weight = GetWeight(item.Weight);
                    node.UniqueName = item.UniqueName;
                    node.Host = item.Host;
                    node.Port = item.Port;
                    node.Address = GetAddress(item.Host, item.Port);
                    node.Id = item.Id;
                    serversList.Add(node);
                }
                else
                {
                    var nodeNew = new ServerNode
                    {
                        Id = item.Id,
                        UniqueName = item.UniqueName,
                        Host = item.Host,
                        Port = item.Port,
                        Address = GetAddress(item.Host, item.Port),
                        Weight = GetWeight(item.Weight)
                    };
                    serversList.Add(nodeNew);
                    serversDict.Add(address, nodeNew);
                }
            }
            List<string> toRemove = new List<string>();
            foreach (var item in serversDict.Keys)
            {
                if (!updatedAddresses.Contains(item))
                {
                    toRemove.Add(item);
                }
            }

            foreach (var item in toRemove)
            {
                serversDict.Remove(item);
            }

            return serversList;
        }
        public static string GetAddress(string host, int port)
        {
            return $"{host}:{port}";
        }
        public static int GetWeight(int weight)
        {
            if (weight <= 0)
            {
                return 1;
            }
            if (weight > 100)
            {
                return 100;
            }
            return weight;
        }
    }
}
