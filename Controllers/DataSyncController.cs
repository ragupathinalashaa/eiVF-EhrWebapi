using DTO.FHIR;
using eIVF.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Manager;
using System.Collections.Generic;
using System.Text.Json;

namespace eIVF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSyncController : ControllerBase
    {
        private readonly IDataSyncService datasyncService;
        public DataSyncController(IDataSyncService _datasyncService)
        {
            datasyncService = _datasyncService;
        }
     
      
        [HttpPost("Update")]
        public async Task<ApiResponse<string>> UpdateDataSync(string id)
        {
            ApiResponse<string> apiResponse = null;

            if (string.IsNullOrWhiteSpace(id))
            {
                apiResponse = new ApiResponse<string>((int)StatusCodes.Status400BadRequest, "Id not found to update");
                return apiResponse;
            }
            int iReturn = 0;
            DataSyncDTO dataSyncDTO = await datasyncService.GetDataAsyncById(id);
            if (dataSyncDTO != null)
            {
                if (dataSyncDTO.IsSynced)
                {
                    dataSyncDTO = new DataSyncDTO();
                    dataSyncDTO.TenantId = "TN00001";
                    dataSyncDTO.ResourceType = "Patient";
                    dataSyncDTO.Id = id;
                    dataSyncDTO.CreatedOn = DateTime.UtcNow;
                    dataSyncDTO.UpdatedOn = null;
                    dataSyncDTO.IsSynced = false;
                    iReturn = await datasyncService.AddDataSyncAsync(dataSyncDTO);
                }
                else
                {
                    dataSyncDTO.TenantId = "TN00001";
                    dataSyncDTO.ResourceType = "Patient";
                    dataSyncDTO.Id = id;
                    dataSyncDTO.UpdatedOn = DateTime.UtcNow;
                    dataSyncDTO.IsSynced = false;
                    DataSyncDTO dataSyncDTOReturn = await datasyncService.UpdateDataSyncAsync(dataSyncDTO);
                    if (dataSyncDTOReturn != null)
                    {
                        iReturn = 1;
                    }
                }
            }
            else
            {
                dataSyncDTO = new DataSyncDTO();
                dataSyncDTO.TenantId = "TN00001";
                dataSyncDTO.ResourceType = "Patient";
                dataSyncDTO.Id = id;
                dataSyncDTO.CreatedOn = DateTime.UtcNow;
                dataSyncDTO.UpdatedOn = null;
                dataSyncDTO.IsSynced = false;
                iReturn = await datasyncService.AddDataSyncAsync(dataSyncDTO);
            }
            if (iReturn > 0)
            {
                apiResponse = new ApiResponse<string>((int)StatusCodes.Status200OK, "added successfully");
            }
            else
            {
                apiResponse = new ApiResponse<string>((int)StatusCodes.Status500InternalServerError, "Unable to add");
            }
           
            return apiResponse;

        }
    }
}
