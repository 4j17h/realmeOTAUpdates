using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;

namespace realmeOTAUpdates
{
    class Client
    {
        public static HttpClient client = new HttpClient();
        public static string Hostname;
        public static Boolean Next;
        public static string SignedFileURL;
        public static string DeviceModelName;
        public class reqJSON
        {
            public string @params { get; set; }
        }

        public class csv
        {
            public string ModelName { get; set; }
            public string Device { get; set; }
            public string OTAVersion { get; set; }
            public string AndroidVersion { get; set; }
            public string SignedOZIP { get; set; }
            public string OTAOZIP { get; set; }
            public string Changelog { get; set; }
            public long Size { get; set; }
            public string MD5 { get; set; }
        }

        public static async Task GetResponse(bool CN, string data, string prN, string ROMV)
        {
            if (CN) { Hostname = "iota.coloros.com"; } else Hostname = "ifota.realmemobile.com";
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10; " + prN + " " + ROMV + "; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/87.0.4280.141 Mobile Safari/537.36");
            reqJSON p = new reqJSON { @params = data };
            string jsonS = JsonConvert.SerializeObject(p);
            HttpContent content = new StringContent(jsonS, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://" + Hostname + "/post/Query_Update", content);
            if (response.IsSuccessStatusCode)
            {
                string decryptResp = Crypto.Decrypt(response.Content.ReadAsStringAsync().Result.Substring(10).Replace('"', ' ').Replace('}', ' ').Trim());
                await ParseResponse(decryptResp);
            }
        }

        public static async Task VerifySign(string PackageName)
        {
            client.DefaultRequestHeaders.Remove("Accept");
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "http://download.c.realme.com/osupdate/" + PackageName));
            if (response.StatusCode.ToString() == "OK")
            {
                SignedFileURL = "http://download.c.realme.com/osupdate/" + PackageName;
            }
            else SignedFileURL = "Not available yet, check again later";
        }

        public static async Task ParseResponse(string DecryptedResponse)
        {
            JObject respD = JObject.Parse(DecryptedResponse);
            //Console.WriteLine(respD);
            string mPackageName = (string)respD["patch_name"];
            var URLSplit = mPackageName.Split(new char[] { '_' });
            await VerifySign(mPackageName.Substring(0, mPackageName.Length - URLSplit[6].Length - 1) + ".ozip");
            var recordsN = new List<csv>
            {
            new csv {
            ModelName = DeviceModelName,
            Device = ((string)respD["new_version"]).Split("_")[0],
            OTAVersion = (string)respD["new_version"],
            AndroidVersion = (string)respD["newColorOSVersion"] + " ["+(string)respD["newAndroidVersion"]+"]",
            SignedOZIP = SignedFileURL,
            OTAOZIP = (string)respD["active_url"],
            Changelog = (string)respD["description"],
            Size = (long)respD["patch_size"],
            MD5 = (string)respD["patch_md5"]
                }
            };
            using (var stream = File.Open("AllDevices_LatestUpdates.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (!Next)
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.WriteRecords(recordsN);
                    Next = true;
                }
                else
                {
                    csv.Configuration.HasHeaderRecord = false;
                    csv.WriteRecords(recordsN);
                }
            }
        }
    }
}
