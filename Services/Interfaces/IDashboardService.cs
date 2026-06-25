using SantoExpedito.DTOs;

namespace SantoExpedito.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync();
}
