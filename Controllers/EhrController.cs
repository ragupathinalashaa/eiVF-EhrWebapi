using DTO.FHIR;
using eIVF.Utility;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Services.Manager;

namespace eIVF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EhrController : Controller
    {

        private readonly IDataSyncService dataSyncService;
        public EhrController(IDataSyncService _dataSyncService)
        {
            dataSyncService = _dataSyncService;
        }


        [HttpGet("GetAll")]
        public async Task<ApiResponse<List<Patient>>> GetAllPatients(string id)
        {

            if (id != null && id != "")
            {
                string message = "Data Found";

                List<Patient> patientsList = new List<Patient>();
                IEnumerable<DataSyncDTO> dataSyncDTOs = await dataSyncService.GetAllDataAsync(id);
                if (dataSyncDTOs == null)
                {
                    return new ApiResponse<List<Patient>>((int)StatusCodes.Status500InternalServerError, message, null);
                }
                else
                {
                    if (dataSyncDTOs != null)
                    {

                        foreach (DataSyncDTO dataSyncDTO in dataSyncDTOs.Where(cond => cond.Id != null))
                        {
                            if (dataSyncDTO.Id != "")
                            {
                                ApiResponse<Patient> aspResponse = await FhirDataAccess.GetPatinetById(dataSyncDTO.Id);
                                if (aspResponse.StatusCode == StatusCodes.Status200OK)
                                {
                                    if (aspResponse.Data != null)
                                    {
                                        patientsList.Add(aspResponse.Data);
                                    }
                                }
                            }
                        }
                    }
                }
                return new ApiResponse<List<Patient>>((int)StatusCodes.Status200OK, patientsList);
            }
            else
            {
                return new ApiResponse<List<Patient>>((int)StatusCodes.Status400BadRequest, "Tenant id not found", null);
            }
        }


        [HttpGet("GetUpdateSummaryById/{id}")]
        public async Task<ApiResponse<IEnumerable<UpdateSummaryDTO>>> GetUpdateSummary(string id)
        {
            if (id != "")
            {
                IEnumerable<UpdateSummaryDTO> updateSummaryDTO = await dataSyncService.GetUpdateSummaryByIdAsync(id);
                if (updateSummaryDTO == null)
                {
                    return new ApiResponse<IEnumerable<UpdateSummaryDTO>>((int)StatusCodes.Status404NotFound, "No such detail found transient database", null);
                }
                else
                {
                    return new ApiResponse<IEnumerable<UpdateSummaryDTO>>((int)StatusCodes.Status200OK, "Data Found", updateSummaryDTO);
                }
            }
            else
            {
                return new ApiResponse<IEnumerable<UpdateSummaryDTO>>((int)StatusCodes.Status400BadRequest, "Tenant Id not found", null);
            }
        }
    }
}
