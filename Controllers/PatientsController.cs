using DTO.FHIR;
using eIVF.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Manager;
using System.Collections.Generic;

namespace eIVF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService patientService;
        public PatientsController(IPatientService _patientService)
        {
            patientService = _patientService;
        }
     

        [HttpPost("Add")]
        public async Task<ApiResponse> AddPatientAsyn([FromBody] ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                return new ApiResponse((int)StatusCodes.Status400BadRequest);
            }
            else
            {
                int iSavedStatus = await patientService.AddPatientAsync((apiResponse.Data as PatientsDTO));
                if (iSavedStatus > 0)
                {
                    return new ApiResponse((int)StatusCodes.Status200OK);
                }
                else
                {
                    return new ApiResponse((int)StatusCodes.Status400BadRequest, "Unable to save");
                }
            }
        }

        [HttpPost("Update")]
        public async Task<ApiResponse> UpdatePatient([FromBody] ApiResponse apiResponse)
        {
            if (apiResponse == null)
            {
                return new ApiResponse((int)StatusCodes.Status400BadRequest);
            }
            else
            {
                PatientsDTO patientsDTO = await patientService.UpdatePatientAsync((apiResponse.Data as PatientsDTO));
                if (patientsDTO!=null)
                {
                    return new ApiResponse((int)StatusCodes.Status200OK, patientsDTO);
                }
                else
                {
                    return new ApiResponse((int)StatusCodes.Status400BadRequest, "Unable to save");
                }
            }
        }

    }
}
