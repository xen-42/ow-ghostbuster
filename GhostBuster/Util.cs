using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GhostBuster
{
    public static class Util
    {
        public static void Log(string msg)
        {
            GhostBuster.Instance.ModHelper.Console.WriteLine(msg, OWML.Common.MessageType.Info);
        }

        public static void LogError(string msg)
        {
            GhostBuster.Instance.ModHelper.Console.WriteLine("Error: " + msg, OWML.Common.MessageType.Error);
        }
    }
}
