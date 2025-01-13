using HomeAssistant.Contracts.DTOs;

namespace HomeAssistant.Contracts.Repositories;

public interface IWaterheaterRepository
{
    Task<IWaterheater> AddWaterheaterAsync(IWaterheater waterheater);
    Task<IWaterheater> UpdateWaterheaterAsync(IWaterheater waterheater);
    Task<IEnumerable<IWaterheater>> GetWaterheatersAsync();
    Task<IWaterheater> GetWaterheaterByIdAsync(int waterheaterId);

    Task<IWaterheaterUsage> AddWaterheaterUsage(IWaterheaterUsage waterheaterUsage);
    Task<IWaterheaterUsage> UpdateWaterheaterUsageAsync(IWaterheaterUsage waterheaterUsage);
    Task<IEnumerable<IWaterheaterUsage>> GetWaterheaterUsagesAsync();
    Task<IWaterheaterUsage> GetWaterheaterUsageByIdAsync(int waterheaterUsageId);
    
}