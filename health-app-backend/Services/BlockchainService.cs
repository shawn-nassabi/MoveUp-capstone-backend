using health_app_backend.FunctionMessageClasses;

namespace health_app_backend.Services;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;

public class BlockchainService : IBlockchainService
{
    private readonly Web3 _web3;
    private readonly string _privateKey;
    private readonly string _rewardSystemAddress;
    private readonly string _tokenAddress;
    private readonly string _dataStorageAddress;
    private readonly string? _backendWalletAddress;

    public BlockchainService(IConfiguration config)
    {
        _privateKey = config["Blockchain:PrivateKey"];
        _rewardSystemAddress = config["Blockchain:HealthRewardSystemAddress"];
        _tokenAddress = config["Blockchain:HealthDataTokenAddress"];
        _dataStorageAddress = config["Blockchain:HealthDataStorageAddress"];
        _backendWalletAddress = config["Blockchain:BackendWalletAddress"];
        _web3 = new Web3(new Account(_privateKey), config["Blockchain:PolygonRpcUrl"]);
    }
    
    // Submit Daily Data on Behalf of User
    public async Task SubmitDailyDataAsync(string userAddress, int dataTypes, bool hasCondition)
    {
        var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

        var functionMessage = new SubmitDailyDataOnBehalfFunction()
        {
            User = userAddress,
            DataTypes = dataTypes,
            HasCondition = hasCondition
        };
        
        // Send the transaction using the backend account (which pays for gas)
        var txHash = await contractHandler.SendRequestAsync(functionMessage);
        Console.WriteLine($"Daily Data Submitted! Tx: {txHash}");
    }
    
    // Convert Points to Tokens for User
    // public async Task ConvertPointsToTokensAsync(string userAddress)
    // {
    //     var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);
    //     
    //     
    //
    //     var tx = await function.SendTransactionAsync(_web3.TransactionManager.Account.Address, new object[] { userAddress });
    //
    //     Console.WriteLine($"Points Converted to Tokens! Tx: {tx}");
    // }
    
    
}