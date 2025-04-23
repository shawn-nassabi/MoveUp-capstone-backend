namespace health_app_backend.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;              // Install-Package Newtonsoft.Json
using Nethereum.Signer;            // Install-Package Nethereum.Signer

public class WalletInfo
{
    public string Address { get; set; }
    public string PrivateKey { get; set; }
}

public class WalletExporter
{
    public static void Main()
    {
        var wallets = new List<WalletInfo>();
        for (int i = 1; i <= 40; i++)
        {
            var ecKey = EthECKey.GenerateKey();
            wallets.Add(new WalletInfo
            {
                Address    = ecKey.GetPublicAddress(),
                PrivateKey = ecKey.GetPrivateKey()
            });
        }

        var json = JsonConvert.SerializeObject(wallets, Formatting.Indented);
        File.WriteAllText("wallets-test-users.json", json);
        Console.WriteLine("✅ Generated 20 wallets and wrote wallets-test-users.json");
    }
}