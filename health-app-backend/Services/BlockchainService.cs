using health_app_backend.DTOs;
using health_app_backend.FunctionMessageClasses;
using health_app_backend.Helpers;
using health_app_backend.Models;
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
    private readonly IPointsRewardHistoryRepository _pointsRewardHistoryRepo;
    private readonly ITokenRewardHistoryRepository _tokenRewardHistoryRepo;

    public BlockchainService(IConfiguration config, IUserRepository userRepository,
        IPointsRewardHistoryRepository pointsRewardHistoryRepo,
        ITokenRewardHistoryRepository tokenRewardHistoryRepo)
    {
        _privateKey = config["Blockchain:PrivateKey"];
        _rewardSystemAddress = config["Blockchain:HealthRewardSystemAddress"];
        _tokenAddress = config["Blockchain:HealthDataTokenAddress"];
        _dataStorageAddress = config["Blockchain:HealthDataStorageAddress"];
        _backendWalletAddress = config["Blockchain:BackendWalletAddress"];
        _web3 = new Web3(new Account(_privateKey), config["Blockchain:PolygonRpcUrl"]);
        
        _userRepository = userRepository;
        _pointsRewardHistoryRepo = pointsRewardHistoryRepo;
        _tokenRewardHistoryRepo = tokenRewardHistoryRepo;
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
    public async Task<BigInteger> GetUserPointsAsync(Guid userId)
    {
        try
        {
            var userWalletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
            if (string.IsNullOrEmpty(userWalletAddress))
            {
                throw new Exception("User does not have a blockchain wallet address.");
            }

            var contractHandler = _web3.Eth.GetContractHandler(_rewardSystemAddress);

            var query = new QueryUserPointsFunction()
            {
                User = userWalletAddress
            };

            var result = await contractHandler.QueryAsync<QueryUserPointsFunction, BigInteger>(query);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to get user points for userId {userId}. Error: {ex.Message}");
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
            Amount = Web3.Convert.ToWei(amount, 18) // Scale to match 18 decimals
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
    
    public async Task<IEnumerable<PointsRewardHistoryDto>> GetPointsRewardHistoryAsync(Guid userId)
    {
        Console.WriteLine($"➡️ Fetching points reward history for user {userId}");
        try
        {
            var walletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
            if (string.IsNullOrEmpty(walletAddress))
            {
                Console.WriteLine($"❌ Wallet address not found for user {userId}");
                throw new Exception("User does not have a blockchain wallet address.");
            }
 
            var history = await _pointsRewardHistoryRepo.GetByWalletAddressAsync(walletAddress);
            Console.WriteLine($"✅ Retrieved {history?.Count() ?? 0} points reward records for {walletAddress}");
            
            // Convert to DTOs
            return history.Select(h => new PointsRewardHistoryDto
            {
                Id = h.Id,
                WalletAddress = h.WalletAddress,
                Points = (long)h.Points,
                Timestamp = h.Timestamp
            });
            // return history;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving points reward history for user {userId}: {ex.Message}");
            throw new ApplicationException("Failed to retrieve points reward history.", ex);
        }
    }
 
    public async Task<IEnumerable<TokenRewardHistoryDto>> GetTokenRewardHistoryAsync(Guid userId)
    {
        Console.WriteLine($"➡️ Fetching token reward history for user {userId}");
        try
        {
            var walletAddress = await _userRepository.GetWalletAddressByUserIdAsync(userId);
            if (string.IsNullOrEmpty(walletAddress))
            {
                Console.WriteLine($"❌ Wallet address not found for user {userId}");
                throw new Exception("User does not have a blockchain wallet address.");
            }
 
            var history = await _tokenRewardHistoryRepo.GetByWalletAddressAsync(walletAddress);
            Console.WriteLine($"✅ Retrieved {history?.Count() ?? 0} token reward records for {walletAddress}");
            
            // Convert to DTOs
            return history.Select(h => new TokenRewardHistoryDto
            {
                Id = h.Id,
                WalletAddress = h.WalletAddress,
                Tokens = (long)h.Tokens,
                Timestamp = h.Timestamp
            });
            //return history;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving token reward history for user {userId}: {ex.Message}");
            throw new ApplicationException("Failed to retrieve token reward history.", ex);
        }
    }
    
    public async Task SyncPointsRewardHistoryFromChainAsync()
    {
        // 1) Create the event handler for PointsEarned
        var eventHandler = _web3.Eth.GetEvent<PointsEarnedEventDTO>(_rewardSystemAddress);

        // 2) Build a filter from the first block (or your last‐synced block) to latest
        var filterAll = eventHandler.CreateFilterInput(
            BlockParameter.CreateEarliest(),
            BlockParameter.CreateLatest()
        );

        // 3) Retrieve all matching logs
        var logs = await eventHandler.GetAllChangesAsync(filterAll);

        Console.WriteLine($"🔄 Found {logs.Count} new PointsEarned events");

        // 4) For each log, upsert into your DB
        foreach (var log in logs)
        {
            var wallet = log.Event.User;
            var points = log.Event.Points;
            //var timestamp = DateTimeOffset.FromUnixTimeSeconds((long)log.Log.BlockNumber.Value).UtcDateTime;
            
            // Fetch the block to get its real timestamp
            var block = await _web3.Eth.Blocks
                .GetBlockWithTransactionsByNumber
                .SendRequestAsync(log.Log.BlockNumber);
            var timestamp = DateTimeOffset
                .FromUnixTimeSeconds((long)block.Timestamp.Value)
                .UtcDateTime;

            // Avoid duplicates by checking if it already exists
            var exists = (await _pointsRewardHistoryRepo
                .GetByWalletAddressAsync(wallet))
                .Any(r => r.Timestamp == timestamp && r.Points == points);

            if (!exists)
            {
                var record = new PointsRewardHistory
                {
                    WalletAddress = wallet,
                    Points        = points,
                    Timestamp     = timestamp
                };
                await _pointsRewardHistoryRepo.AddAsync(record);
            }
        }

        // 5) Persist them all at once
        await _pointsRewardHistoryRepo.UnitOfWork.SaveChangesAsync();
    }

    public async Task SyncTokenRewardHistoryFromChainAsync()
    {
        var eventHandler = _web3.Eth.GetEvent<TokensMintedEventDTO>(_rewardSystemAddress);
        var filterAll    = eventHandler.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
        var logs         = await eventHandler.GetAllChangesAsync(filterAll);

        Console.WriteLine($"🔄 Found {logs.Count} new TokensMinted events");

        foreach (var log in logs)
        {
            var wallet    = log.Event.User;
            var tokens    = log.Event.Tokens;
            //var timestamp = DateTimeOffset.FromUnixTimeSeconds((long)log.Log.BlockNumber.Value).UtcDateTime;
            
            // Fetch the block to get its real timestamp
            var block = await _web3.Eth.Blocks
                .GetBlockWithTransactionsByNumber
                .SendRequestAsync(log.Log.BlockNumber);
            var timestamp = DateTimeOffset
                .FromUnixTimeSeconds((long)block.Timestamp.Value)
                .UtcDateTime;

            var exists = (await _tokenRewardHistoryRepo
                .GetByWalletAddressAsync(wallet))
                .Any(r => r.Timestamp == timestamp && r.Tokens == tokens);

            if (!exists)
            {
                var record = new TokenRewardHistory
                {
                    WalletAddress = wallet,
                    Tokens        = tokens,
                    Timestamp     = timestamp
                };
                await _tokenRewardHistoryRepo.AddAsync(record);
            }
        }

        await _tokenRewardHistoryRepo.UnitOfWork.SaveChangesAsync();
    }
}