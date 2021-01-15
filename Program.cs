using System;
using System.IO;
using System.Threading.Tasks;

namespace realmeOTAUpdates
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args[0] == "--updates")
                {
                    if (File.Exists("AllDevices_LatestUpdates.csv"))
                    {
                        File.Delete("AllDevices_LatestUpdates.csv");
                    }
                    Console.WriteLine("Checking for updates........");
                    FetchUpdates().GetAwaiter().GetResult();
                    Console.WriteLine("Process Completed.");
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("Use argument --updates");
            }

        }

        public static async Task FetchUpdates()
        {
            string[] devices = {
                "realme 2 Pro_RMX1801EX_C","realme C1/realme 2_RMX1805EX_A","realme 3i/realme 3_RMX1821EX_C","realme U1_RMX1831EX_C","realme X Youth Edition_RMX1851CN_C","realme 3 Pro_RMX1851EX_C","realme X CN_RMX1901CN_C","realme X_RMX1901EX_C","realme 5 EU_RMX1911EU_C","realme 5s/realme 5_RMX1911EX_C","realme XT EU_RMX1921EUEX_C","realme XT_RMX1921EX_C","realme 6 RU_RMX1927RU_A","realme X2 Pro CN_RMX1931CN_C","realme X2 Pro_RMX1931EX_C","realme C2_RMX1941EX_A","realme Q_RMX1971CN_C","realme 5 Pro_RMX1971EX_C","realme X2 CN_RMX1991CN_C","realme X2 AEX_RMX1992AEX_C","realme X2_RMX1992EX_C","realme X2_RMX1993EX_C","realme 6/realme 6i/realme Narzo/realme 6s_RMX2001_B","realme 6s/realme 6 EU_RMX2001EU_A","realme Narzo 10A/realme C3/realme C3i_RMX2020_A","realme C3 EU_RMX2020EU_A","realme 5i EU_RMX2030EX_A","realme Narzo 10/realme 6i_RMX2040_A","realme 6i EU_RMX2040EU_A","realme narzo 20A_RMX2050EX_A","realme X50 5G CN_RMX2051CN_B","realme 6 Pro_RMX2061_A","realme 6 Pro EU_RMX2061EU_A","realme  X50 Pro Player (5G) CN_RMX2071CN_A","realme X50 Pro EU_RMX2076EU_A","realme X50 Pro_RMX2076PU_A","realme X3 SuperZoom EU_RMX2081EU","realme X3 SuperZoom_RMX2081PU_A","realme C17_RMX2101PU_A","realme 7i_RMX2103PU_A","realme Q2 5G/realme V5 5G_RMX2111CN_A","realme 7 5G EU_RMX2111EU_A","realme  X7 Pro 5G CN_RMX2121CN_A","realme X50 5G CN_RMX2141CN_A","realme X50 EU_RMX2144EU_A","realme 7_RMX2151EU_A","realme narzo 20 Pro/realme 7_RMX2151PU_A","realme 7 Pro EU_RMX2170EU_A","realme 7 Pro_RMX2170PU_A",/*"RMX2175[exc]",*/"RMX2185_A",/*"RMX2191[exc]",*/"realme C15 Qualcomm Edition_RMX2195PU_A","realme V3 5G CN_RMX2200CN_A"
            };
            foreach (string i in devices)
            {
                Client.DeviceModelName = null;
                if (i.Contains("EX") && !i.Contains("EUEX") && !i.Contains("RU"))
                {
                    Client.DeviceModelName = i.Split("_")[0];
                    String ProductNameL = i.Split("_")[1];
                    String ProductNameS = ProductNameL.Substring(0, 7);
                    String ProductI = i.Split("_")[2];
                    await PostReqData(ProductNameL, ProductNameS, ProductI, false);
                }

                if (i.Contains("CN"))
                {
                    Client.DeviceModelName = i.Split("_")[0];
                    String ProductNameL = i.Split("_")[1];
                    String ProductNameS = ProductNameL.Substring(0, 7);
                    String ProductI = i.Split("_")[2];
                    await PostReqData(ProductNameS, ProductNameS, ProductI, true);
                }

                if (i.Contains("RMX2076EU_A"))
                {
                    Client.DeviceModelName = i.Split("_")[0];
                    String ProductNameL = i.Split("_")[1];
                    String ProductNameS = "RMX2075EEA";
                    String ProductI = i.Split("_")[2];
                    await PostReqData(ProductNameL, ProductNameS, ProductI, false);
                }

                if (i.Contains("RMX1927RU_A"))
                {
                    Client.DeviceModelName = i.Split("_")[0];
                    String ProductNameL = "RMX1927EX";
                    String ProductNameS = "RMX1927RU";
                    String ProductI = i.Split("_")[2];
                    await PostReqData(ProductNameL, ProductNameS, ProductI, false);
                }

                if (i.Contains("RMX1911EU_C") || i.Contains("RMX1921EUEX_C") || i.Contains("RMX2111EU_A") || i.Contains("RMX2144EU_A"))
                {
                    Client.DeviceModelName = i.Split("_")[0];
                    String ProductNameL = i.Split("_")[1];
                    String ProductNameS = ProductNameL.Substring(0, 7) + "EEA";
                    String ProductI = i.Split("_")[2];
                    await PostReqData(ProductNameL, ProductNameS, ProductI, false);
                }
            }
        }

        public static async Task PostReqData(string productNameL, string productNameS, string productI, bool CN)
        {
            string VERSION = productNameL + "_11." + productI + ".00_0000_000000000000";
            string IMEI = "000000000000000";
            string ROMVERSION = VERSION.Substring(0, VERSION.Length - 18);
            string PRODUCTNAME = productNameS;
            long TIME = DateTimeOffset.Now.ToUnixTimeSeconds();
            string data = System.Net.WebUtility.UrlDecode("%7B%22version%22%3A+%222%22%2C+%22otaVersion%22%3A+%22" + VERSION + "%22%2C+%22imei%22%3A+%22" + IMEI + "%22%2C+%22mode%22%3A+%220%22%2C+%22language%22%3A+%22en-US%22%2C+%22productName%22%3A+%22" + PRODUCTNAME + "%22%2C+%22type%22%3A+%221%22%2C+%22romVersion%22%3A+%22" + ROMVERSION + "%22%2C+%22colorOSVersion%22%3A+%22null%22%2C+%22androidVersion%22%3A+%22null%22%2C+%22time%22%3A+" + TIME + "%2C+%22registrationId%22%3A+%22UNKNOWN%22%2C+%22operator%22%3A+%22NULL%22%2C+%22trackRegion%22%3A+%22null%22%2C+%22uRegion%22%3A+%22null%22%2C+%22ota_system_root_state%22%3A+%221%22%2C+%22ota_register_trigger_id%22%3A+%22UNKNOWN%22%2C+%22isRooted%22%3A+%221%22%2C+%22canCheckSelf%22%3A+%221%22%2C+%22otaPrefix%22%3A+%22" + ROMVERSION + "%22+%7D");
            //Console.WriteLine("ROMVersion: "+ROMVERSION+" ProductName: "+PRODUCTNAME+" ProductIdentifier: "+productI +"\n");
            String EncryptedData = Crypto.Encrypt(data) + "=404H8RDaae6HE8j";
            await Client.GetResponse(CN, EncryptedData, PRODUCTNAME, ROMVERSION);
        }
    }
}
