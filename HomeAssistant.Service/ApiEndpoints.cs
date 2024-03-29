using HomeAssistant.Contracts.DTOs;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            [Authorize] async (DateTime date, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetWaterHeaterCostByDateAsync(date));
        
        app.MapGet("/waterheater/consumption/{date}", 
            [Authorize] async (DateTime date, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetWaterHeaterConsumptionByDateAsync(date));
        
        app.MapGet("/waterheater/saved/{date}", 
            [Authorize] async (DateTime date, IWaterHeaterService waterHeaterService) => 
                await waterHeaterService.GetSavedByDateAsync(date));

    }
}