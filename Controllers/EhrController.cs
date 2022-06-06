using DTO.FHIR;
using eIVF.Utility;
using Microsoft.AspNetCore.Mvc;
using Services.Manager;

namespace eIVF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EhrController : Controller
    {

        private readonly IPatientService patientService;
        public EhrController(IPatientService _patientService)
        {
            patientService = _patientService;
        }


        [HttpGet("GetAll")]
        public async Task<ApiResponse> GetAllPatients()
        {
            string message = "Data Found";
            IEnumerable<PatientsDTO> patients = await patientService.GetAllPatientsAsync();
            if (patients == null)
            {
                message = "No data found";
            }

            return new ApiResponse((int)StatusCodes.Status200OK, message, patients);
        }


        // GET api/<PatientsController>/5
        [HttpGet("GetById/{id}")]
        public async Task<ApiResponse> GetPatientById(long id)
        {
            var patientDTO = await patientService.GetPatientByIdAsync(id);

            if (patientDTO == null)
            {
                return new ApiResponse((int)StatusCodes.Status404NotFound);
            }

            return new ApiResponse((int)StatusCodes.Status200OK, "DataFound", patientDTO);
        }
    }
}
