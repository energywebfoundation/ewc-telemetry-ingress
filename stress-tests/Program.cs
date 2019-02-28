using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using webapi;

namespace stresstests
{
    class Results
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    class Program
    {
        private static readonly string _nodeId = "node-3";

        static async Task<string> SendDataToIngress2(HttpRequestsManager hm, InfluxTelemetry inobj)
        {
            string statusCode = await hm.sendData(inobj);
            return await hm.sendData(inobj);

        }

        static void Main(string[] args)
        {
            RSACryptoManager cMgr = new RSACryptoManager("cspblob.bin");
            HttpRequestsManager hMgr = new HttpRequestsManager("http://localhost:5000/api/ingress/influx/", true);
            ConcurrentDictionary<string, int> results = new ConcurrentDictionary<string, int>();

            Stopwatch timer2 = new Stopwatch();
            timer2.Start();

            int legitTraffic = 500;
            int sigFailTraffic = 200;
            int invalidPayloadTraffic = 200;
            //var tasks = Enumerable.Range(0, taskCount).Select(p => SendDataToIngress2(cMgr, hMgr, results));
            //Task.WhenAll(tasks).Wait();
            for (int i = 1; i <= legitTraffic; i++)
            {
                InfluxTelemetry inobj = generateRandomData(cMgr, true, true);
                Task<string> task = Task.Run<string>(async () => await SendDataToIngress2(hMgr, inobj));
                recordTestResults(task, results);
            }
            for (int i = 1; i <= sigFailTraffic; i++)
            {
                InfluxTelemetry inobj = generateRandomData(cMgr, true, false);
                Task<string> task = Task.Run<string>(async () => await SendDataToIngress2(hMgr, inobj));
                recordTestResults(task, results);
            }
            for (int i = 1; i <= invalidPayloadTraffic; i++)
            {
                InfluxTelemetry inobj = generateRandomData(cMgr, false, false);
                Task<string> task = Task.Run<string>(async () => await SendDataToIngress2(hMgr, inobj));
                recordTestResults(task, results);
            }

            timer2.Stop();
            TimeSpan timeTaken2 = timer2.Elapsed;
            Console.WriteLine(timeTaken2.ToString());

            printTestResults(results);

        }

        public static void recordTestResults(Task<string> task, ConcurrentDictionary<string, int> results)
        {
            if (results.ContainsKey(task.Result))
            {
                results[task.Result] += 1;
            }
            else
            {
                results.TryAdd(task.Result, 1);
            }
        }

        public static void printTestResults(ConcurrentDictionary<string, int> results)
        {
            foreach (KeyValuePair<string, int> kvp in results)
            {
                Console.WriteLine("Status = {0}, Counts = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("___");
        }

        public static InfluxTelemetry generateRandomData(RSACryptoManager cm, bool validData, bool validSig)
        {

            List<string> lst = new List<string>();
            Random r = new Random();
            int rInt = r.Next(100, 999);

            lst.Add("weather,location=us-midwest temperature=" + rInt + " 1465839830100400" + rInt);
            lst.Add("weather,location=us-east temperature=" + rInt + " 1465839830100400" + rInt);

            string payload = string.Join("", lst);
            
            return  (validData?
                new InfluxTelemetry() { NodeId = _nodeId, Payload = lst, Signature = validSig ? cm.signData(payload) : "" } :
                new InfluxTelemetry() { NodeId = "", Signature = validSig ? cm.signData(payload) : "" });
        }

    }
}