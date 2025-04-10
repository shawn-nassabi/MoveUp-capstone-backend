using health_app_backend.FunctionMessageClasses;
using health_app_backend.Helpers;
using health_app_backend.Repositories;

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
    private readonly IUserRepository _userRepository;

    public BlockchainService(IConfiguration config, IUserRepository userRepository)
    {
        _privateKey = config["Blockchain:PrivateKey"];
        _rewardSystemAddress = config["Blockchain:HealthRewardSystemAddress"];
        _tokenAddress = config["Blockchain:HealthDataTokenAddress"];
        _dataStorageAddress = config["Blockchain:HealthDataStorageAddress"];
        _backendWalletAddress = config["Blockchain:BackendWalletAddress"];
        _web3 = new Web3(new Account(_privateKey), config["Blockchain:PolygonRpcUrl"]);
        
        _userRepository = userRepository;
    }
    
    // Submit Daily Data on Behalf of User
    public async Task SubmitDailyDataAsync(string userAddress, int dataTypes, bool hasCondition)
    {
        try
        {
            var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

            var functionMessage = new SubmitDailyDataOnBehalfFunction()
            {
                User = userAddress,
                DataTypes = dataTypes,
                HasCondition = hasCondition
            };

            var txHash = await contractHandler.SendRequestAsync(functionMessage);
            Console.WriteLine($"✅ Daily Data Submitted! Tx: {txHash}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to submit daily data for user {userAddress}. Error: {ex.Message}");

            if (ex.Message.Contains("User already submitted today", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You have already submitted your daily health data within the last 24 hours.");
            }

            throw new ApplicationException("Blockchain transaction failed during daily data submission.", ex);
        }
    }
    
    // Get User Points from the Smart Contract
    // Get User Points from the Smart Contract
    public async Task<BigInteger> GetUserPointsAsync(string userAddress)
    {
        try
        {
            // Create a contract handler for the reward system contract
            var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

            // Create a function message instance for querying the userPoints mapping
            var query = new QueryUserPointsFunction()
            {
                User = userAddress
            };

            // Query the contract using the strongly typed function message
            var result = await contractHandler.QueryAsync<QueryUserPointsFunction, BigInteger>(query);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to get user points for {userAddress}. Error: {ex.Message}");
            throw new ApplicationException("Could not fetch user points from the blockchain.", ex);
        }
    }
    
    // Submit hash of data to the blockchain
    public async Task SubmitDataHashAsync(Guid userId, string dataHash)
    {
        var userWalletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
        if (string.IsNullOrEmpty(userWalletAddress))
        {
            throw new Exception("User does not have a blockchain wallet address.");
        }

        var contractHandler = _web3.Eth.GetContractHandler(_dataStorageAddress);

        var functionMessage = new SubmitDataOnBehalfFunction()
        {
            User = userWalletAddress,
            DataHash = HashingUtils.HexStringToByteArray(dataHash) // CRITICAL FIX, convert the string to byte32 format for proper storage on chain
        };

        var txHash = await contractHandler.SendRequestAsync(functionMessage);
        Console.WriteLine($"✅ Data Hash Submitted! Tx: {txHash}");
    }
}