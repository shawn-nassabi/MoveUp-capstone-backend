using health_app_backend.FunctionMessageClasses;
using health_app_backend.Helpers;
using health_app_backend.Repositories;
using Nethereum.RPC.Eth.DTOs;

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
    
    // Method for converting points to erc20 HDT tokens if given user has enough points
    public async Task ConvertPointsToTokensAsync(Guid userId)
    {
        var userWalletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
        if (string.IsNullOrEmpty(userWalletAddress))
        {
            throw new Exception("User does not have a blockchain wallet address.");
        }

        var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

        var functionMessage = new ConvertPointsToTokensOnBehalfFunction()
        {
            User = userWalletAddress
        };

        try
        {
            var txHash = await contractHandler.SendRequestAsync(functionMessage);
            Console.WriteLine($"✅ Points Converted to Tokens! Tx: {txHash}");
            
            // Wait briefly for the blockchain to register the event
            await Task.Delay(5000); // 5 seconds - optional tuning

            // Listen for event to know how many tokens to mint
            var tokensToMint = await GetTokensMintedEventAsync(userWalletAddress);

            // Mint Tokens Now
            await MintTokensAsync(userWalletAddress, tokensToMint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to convert points to tokens for user {userWalletAddress}. Error: {ex.Message}");
            if (ex.Message.Contains("Not enough points to convert", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You do not have enough points to convert to tokens.");
            }

            throw new ApplicationException("Blockchain transaction failed during token conversion.", ex);
        }
        
    }
    
    // Method for listening for tokens minted event
    public async Task<BigInteger> GetTokensMintedEventAsync(string userAddress)
    {
        var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

        var eventHandler = contractHandler.GetEvent<TokensMintedEventDTO>();

        var filterInput = eventHandler.CreateFilterInput(fromBlock: BlockParameter.CreateLatest());

        var logs = await eventHandler.GetAllChangesAsync(filterInput);

        foreach (var log in logs)
        {
            if (log.Event.User.Equals(userAddress, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Event Found: {log.Event.Tokens} tokens to mint for user {userAddress}");
                return log.Event.Tokens;
            }
        }

        throw new Exception("TokensMinted event not found for the user.");
    }
    
    // Method for minting tokens
    public async Task MintTokensAsync(string userAddress, BigInteger amount)
    {
        var contractHandler = _web3.Eth.GetContractHandler(_tokenAddress);

        var functionMessage = new MintFunction()
        {
            To = userAddress,
            Amount = amount
        };

        var txHash = await contractHandler.SendRequestAsync(functionMessage);
        Console.WriteLine($"✅ Tokens Minted! Tx: {txHash}");
    }
    
    public async Task<BigInteger> GetPointsPerTokenAsync()
    {
        try
        {
            var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

            var result = await contractHandler.QueryAsync<QueryPointsPerTokenFunction, BigInteger>(new QueryPointsPerTokenFunction());

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to get pointsPerToken rate. Error: {ex.Message}");
            throw new ApplicationException("Could not fetch pointsPerToken from the blockchain.", ex);
        }
    }
    
    public async Task<decimal> GetUserTokenBalanceAsync(Guid userId)
    {
        try
        {
            var userWalletAddress = await GetUserWalletAddressAsync(userId);
            var contractHandler = _web3.Eth.GetContractHandler(_tokenAddress);

            var query = new QueryTokenBalanceFunction()
            {
                Account = userWalletAddress
            };

            var result = await contractHandler.QueryAsync<QueryTokenBalanceFunction, BigInteger>(query);

            // Convert from Wei to Token value (18 decimals)
            var tokenBalance = Web3.Convert.FromWei(result, 18); 

            return tokenBalance;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to get HDT token balance for {userId}. Error: {ex.Message}");
            throw new ApplicationException("Could not fetch user token balance from the blockchain.", ex);
        }
    }
    
    private async Task<string> GetUserWalletAddressAsync(Guid userId)
    {
        var userWalletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
        if (string.IsNullOrEmpty(userWalletAddress))
        {
            throw new Exception("User does not have a blockchain wallet address.");
        }
        return userWalletAddress;
    }
}