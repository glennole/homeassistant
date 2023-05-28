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
        app.MapGet("/waterheaters/{id}/state",
            [Authorize] async (int id, IWaterHeaterService waterHeaterService) => await waterHeaterService.GetStateById(id));


        app.MapGet("/waterheater/{id}", 
            [Authorize] async (int id, IWaterHeaterService waterHeaterService) => await waterHeaterService.GetById(id));
       
    }
}