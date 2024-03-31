using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Impl.Calendar;

namespace HomeAssistant.Service;

public static class ApiEndpoints
{
    public static void RegisterWaterHeaterAPIs(this WebApplication app)
    {
        app.MapGet("/waterheater/{id}/state",
            [Authorize] async (int id, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetStateByIdAsync(id));


        app.MapGet("/waterheater/{id}", 
            [Authorize] async (int id, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetByIdAsync(id));

        app.MapGet("/waterheater/cost/{date}", 
            [Authorize] async (DateTimeOffset date, IWaterHeaterService waterHeaterService) =>
            {
                return await waterHeaterService.GetWaterHeaterCostByDateAsync(date.ToUniversalTime().DateTime);
            });
        
        app.MapGet("/waterheater/consumption/{date}", 
            [Authorize] async (DateTimeOffset date, IWaterHeaterService waterHeaterService) =>
            {
                return await waterHeaterService.GetWaterHeaterConsumptionByDateAsync(date.ToUniversalTime().DateTime);
            });
        
        app.MapGet("/waterheater/saved/{date}", 
            [Authorize] async (DateTimeOffset date, IWaterHeaterService waterHeaterService) =>
            {
                return await waterHeaterService.GetSavedByDateAsync(date.ToUniversalTime().DateTime);
            });
        
        app.MapGet("/waterheater/saved/{year}/{month}", 
            [Authorize] async (int year, int month, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetSavedByMonthAsync(year, month));

    }
}