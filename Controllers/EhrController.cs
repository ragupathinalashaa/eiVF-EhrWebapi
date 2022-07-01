using DTO.FHIR;
using eIVF.Filters;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Manager;
using System.Text;
using System.Text.Json;

namespace eIVF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EhrController : Controller
    {

        private readonly IDataSyncService dataSyncService;
        private readonly IFhirService fhirService;
        public EhrController(IDataSyncService _dataSyncService, IFhirService _fhirService)
        {
            dataSyncService = _dataSyncService;
            fhirService = _fhirService;
        }

        /// <summary>
        /// Get All Patient Records where are not synced to EHR on prim database 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Returns no of patient which are not synced</returns>
        [HttpGet("GetUpdatedPatientRecords")]
        public async Task<ApiResponse<List<Patient>>> GetAllPatients(string tenantId)
        {

            if (tenantId != null && tenantId != "")
            {
                string message = "Data Found";

                List<Patient> patientsList = new List<Patient>();
                IEnumerable<DataSyncDTO> dataSyncDTOs = await dataSyncService.GetAllDataAsync(tenantId);
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
                                ApiResponse<Patient> aspResponse = await fhirService.GetPatinetById(dataSyncDTO.Id);
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
       
        /// <summary>
        /// Get resource count which are not synced with on prim
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <returns>Returns count of each resource type with not synced</returns>
        [HttpGet("GetUpdateSummaryByTenantId")]
        public async Task<ApiResponse<IEnumerable<UpdateSummaryDTO>>> GetUpdateSummary(string tenantId)
        {
            if (tenantId != "")
            {
                IEnumerable<UpdateSummaryDTO> updateSummaryDTO = await dataSyncService.GetUpdateSummaryByIdAsync(tenantId);
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
       
        
        /// <summary>
        /// Update the sync status of on prim into transient database 
        /// </summary>
        /// <param name="fhirId">FhirId</param>
        /// <returns>Updated status will return</returns>
        [HttpPost("UpdateSyncStatus")]
        public async Task<ApiResponse<string>> UpdateStatus(string fhirId)
        {
            if (fhirId != "")
            {
                int status = await dataSyncService.UpdateSyncStatus(fhirId);
                if (status > 0)
                {
                    return new ApiResponse<string>((int)StatusCodes.Status200OK, "Successfully Update");
                }
                else
                {
                    return new ApiResponse<string>((int)StatusCodes.Status404NotFound, "Unable to update");
                }
            }
            else
            {
                return new ApiResponse<string>((int)StatusCodes.Status400BadRequest, "Id not found");
            }
        }


        //[HttpGet("EncryptContent")]
        //public async Task<ApiResponse<dynamic>> EncryptContent(string id)
        //{
        //    ApiResponse<dynamic> apiResponse = new ApiResponse<dynamic>();
        
        //    ApiResponse<string> apiResponseRet = await fhirService.GetPatinetByIdAsString(id);
        //    apiResponse.Data = EncryptionHelper.Encrypt(apiResponseRet.Data, "eivf@*12345678912345678912345678");
        //   // string content = "1tWH/MWR3wnKpkKaqDwtOHxrwrFO7I21fv93SkBcC/WLDK1MP3F1uVraBHGAwXmhiHhPBQw9UX6uBfexvKGPeb9wXedLZvN6ahlBOQKiLdoIGT3Z91rHtp7O+bA+P4ngCvqnbJuDxYoC/SvD05ykc/ov1+s2mlUfFsFVFe7BjCQQ4a6V+U0p4gWd5zHAvc1FLagZ6gx2pwIkDy3H/8zar9X3g86OEuRcdX7xjOvw8zNOw1irz3pz8Y+x73nYx/kJOiW8D+MWUPdaTJUAV7+wnLRoKlZvacpwIEa4X5v1yiD5rDrxee9wSTtnPmarBMIL7Kik11KUHajqH2ZzaouG68ENHIfWB5EJBj5z/V7zSVo77qKvcwsbwSXwodmwNKaAxHu+NehMCW5XqBU8iiUa0QuoAqqLwakmLi43giDGZv3UKitdUomUPJYr4zdv+0NOfMntvldMEZH01rPGlcC+Y+yfxM2qDPZ7yD5ChdYdAprYmD2ubss+25tLyp9lrL8VoVTBtT9+IiB6eTV8fI6EH38O3bw/zKSEJS346Ziy9Bq9CQpwlj5GbNFeBCyU4QAe6rBH+FKS69WZMVnvAl+jAFZVz6LRi3rltdUCAfIEOhUDX0vDsvxSqsy91jPBrltpMj9IFLPjkT7i7zQ3I2wh936dRgb0iYvJb2HKjZ1vB/OkElM1EIcaWvAiE4fTidqzgMIsRUsRmzLRtzK+BUqGfB/ZL5d5mQUEd0UOtOFHG8FUK1xH0Qy8rBE4JeiJSbo4qbKYYtxxsvv0yv+sXy0Tk1IxKJlUg61dGQTLWv3vL+vU8KzvFl7na8EjGK6HP8jmGQEbbrIfvyIB4I5qCQLOPlq6rMagrgw9uHHqzezHP1JWpEXwBsmqJhtV/OjNSx/twmX0FqGn3dzYHuRqKBzhCBiR8ftLW0S4dHY7Gr5CJtwtfWjUb5pFKJBAwkJ5kdecLiIdPf9tfL/hutbGaG4xLgm/QIpsU7fD51VPAuxsDVACyNiod8HPlFBwrIsmSjcpCR+wdGqOg3EsXjjHvAQbtCDNsdB7ebsdNvO2DeKL1zgol0YlJwOtqJZLCaZBF6w/ok4mccza6kC/Wt5JGgquFcEP+K0+4tD+mlI2m2pUbh1/bJSsu9cwCaC2m50olnfcrEOMXApOAwfmlyO+/fJOTtMU6vyqOuOPqqtcDN2XwzLSNl5Dche1xFToF47BvLk1YzzXmJqLWSFT1XL4U+1XOk3OxfPJnT2y5ToorBKmu3NeTahlxay1pyD5rt5XAa9ebuOFw7IIAvaVLfBB5FmHrAvCI6t/Ggf5p13tTEfM/7ScL7LK50o1TjQ0akAmV7PlV/xZM4eVJWMcj4zmAHCiZvdWnFI8nCbAUiWoZTFyc9pe/72r1N5AAQQG5cd7aVVyiKjY+7zZ3563xozNE9tJ6o6YWPsJ85lJJwXcT43DvXJZq7a2uaMC0SQhPqYxjm8EQWEAljspCd3IApKqPEqWNU6GJ1PuRsET0S/bYRPYttqyZ/FbRTMI2hBSNZsUUhqE2dMeFc9ujyQHCpwv4sbrfjACByXKNwk1cx60tcHu5xi0V0GYZ8/+rWvWaqV9u0zv";
        //    apiResponse.Data = EncryptionHelper.Decrypt(apiResponse.Data, "eivf@*12345678912345678912345678");
        //    var ser = JsonConvert.SerializeObject(apiResponseRet.Data);
        //    apiResponse.Data = ser;
        //   // apiResponse.Data = JsonConvert.DeserializeObject<string>(ser);

        //    // string val = ((JsonElement)ser).ToString();

        //    apiResponse.StatusCode = StatusCodes.Status200OK;
        //    return apiResponse;
        //}

        //[HttpGet("DecryptContent")]
        //public async Task<ApiResponse<string>> DecryptContent()
        //{
        //    string privateKey = "1234567890000000";
        //    string bodyContent = "U2FsdGVkX1/wGGCZw47nxvyIxYwxM+E1rv5Zd/LT3CaQP0x7CKUbeHnMxCSmSsnTPHXdtf5O01TLx14Nj0k3gUlqjCajJUUVekYL7Dv4/4mPo5DwyZQ/VKcuE8l8yGKCGgYviUHuOMGL23dlS1nUBlI1c/Ov/gjjNt/R6M2G5eTlKXs+4T39rACyGEASYUtwKJZZ3XrXf0ictJZHOXun9f7aJX1N2d8zluLkcfy4Ph+Zl8H4rzrizakNO4ZN/MZGmMPW5cRLa6q/iVJPWcStPcIy7xNCODijCQpEZ22aZNFWeZZnx4ZYjU/ALKVffabSknaqPNiWgFGQj9K0Eo9Z4lhInluasLrrbHqmW1eN5fZ92gGGsq+MRI3QrPHBHFXhwhSRIfQGSbIYPG2rXJ4V+BRYyfpI/aDyPz4Y+fbr8V8qWcUuwdU0JToKJUlSxUgyOz5d4RuR8/MZRRnKtd+AxR7l59zlVj5bBX+O9OohsEfwm8BGUZQquNpgdqlVErCUByQLmpz+6N4TpG/y97CqSwhqK9zmbp6sev0hV7qPBJMos7AgxbXOgreFe7kvXh66i4KNnS4csEPwCUeXGDjwh751pu7+3fDicvK9+VRGEZ5mPRyXqmM4nu5+dv08caOUdeSW/OLuxx1uTbY34pkP1jTRT+VyGIebbcmg+CMn9/XY54mUAGMSjtXxUtr6riq+L96YliClQOOHPCDl8MgY2o1IJfpe0x27lHw+zMMXefWHYD7pod/82mUqYuBSX9tKV80Us329BIsgmSKZG2fzqmx/kU3dH3MXWFRs4PHWceiesNnrAlzOIhvTFI0toJoI3AtHZNmX+CEvnkJOO6bmM7NqbyoOKZjDp+46+WT8AzGqiGGE55R+XE1H87QnEQwIFaT9MVS9fa1n9wAT0c9Op90t7bvPwVzgxO+2SCKJ8pXh+4hk/pnOwrlJRa8EodmoKZXR19dJPR153YBk3c7NhtDK89yL5BfLL2j/lQ2F4vEhu0yGjvD4C4LpZ98r1glOKsiMjIu0CKkfFs/qyF+tylRJLrJ381T3prWgm2kQ13CUkKKGm7ezlFECOPFjTZ4drTmsPVVf009VEd+WOItmmlLrGTd1PpwmgoNBeW0M97jABBxiPVAUoAXvE9ZqyqFV6XNvuU4ginDlOBgoqjk7KehLfrAuMhM8lJ58Nh480XFOEzD9qr2mN0WiaCcEKfiNobVE53GvKt0m6hmhVX+fRtaDXisd/C11NoOu0/LlZXkNVzgqZo4Pgked0SNT/x5Axy4pvLsw2t4JLp+SkHBHcz4ucj5nPKxu48HdpfpcmcDyTgC7IPs8Wd8UNllawMzFeGpgf0K7NbvTnZEOGN9LrjK2kivAq/1yzoGG6L0YTadiHvfsp9MVqMIq34wZ8AlWKuy20yVPwBw4QcpKz/ZFn0+vGFoD6Rw8gEI0AwJPsb6VpqjPUw47cPMIFidsWlW6sku48TZE/CgqII+pZN8Iyss8P5ePhpmFXWAkc/W6KRYIIVWZsM6pNgvFhVIy2hSjm6zBNavHaEOyJGpJ8nQ1U/nFG/L4KdYtmKYdh48eAz0=";
        //    bodyContent = "U2FsdGVkX19hoOWKR68A5FNl8WIIjSxf7pMVC5s9fw5WNLfDeE8m2klP989AiyQljIJe3TEcc3oT9Wv5xix6avsQfQiksp7g7SPZomQ+ODzn/Ytq4+M+p7Idjijtq0h1FvBRhbDZkGgtkSJ/AOFNSAEVPsooeOdiFSdFLcjY+ml2WoMa9PUnfJy801Yv8hmImkohX54RLSTamlj0q5N3LPtFMLD//mIT651Y6vRjoAQBk4ijpRU2Up804uyY7eNxW6yAlVwtu8yLf0LioB7BRGM+9k4WSkQoGXG/dGzyY/cUmoZPCfibHHp+TTqrJHcNfZNDIGoTXCNha5IJP8EnnF3O8i/7apRTrnqzEUSL5RS0TL33y1h1I6idc0tGO9t9puLONwq7etevyq2WoxIAeTvnmg9GZOn25+DWQquxqSaFcx3Vrm2ApD5euaNP/Cs//uBmpAK7yAp0tRdRPQkyiORNTggekp1782BQdZhDiI1FHhxSCUkxduxtujyCnfcf5MIzIjWC6mzQKwNPSegCcELWLvkO42fMer9VRxWRZSa+1neHMFxKSsNBfuf9LAzGWu/KCVe4k7ag18kLkUHH/1s78SVco3WRP17L4bSFhmL+Yjm+t7HC4Fm8GnaIY689f8Da0R0Ior2lxLkhFtXbAoDGd3N9zz+SrwUEppm6HBTnL5YyWYryMf1dPfspVy80Xx8T6qx+DSqtDXSiCUNFFoDW0UJYjrQ9DWViCvfcfn6XIFGgQ677XSrFohbIWRdF9fTt+x+OYK+TTJNCwUcquCZUH+XOrf8LIglYmYIFzk6F6u08aFS4SOpInE4IXpfOT2n5FMMGyCBpjfnRyCqAPlDmGNYAnTGSxPAC/c2D5Fwb+H62lKjfSjk1K9ItyKs5XEHJw7wCwxgG6IpIIEOCmpbR1hCJLn2b3qNVZo/oPPA4xJxo2BWGl5Ra7YPt+DNG068/fE5az1r6ZYDmCO/w0zRPix/wm+P3ReNiH/wg/ZHDF0zh9bupBUyejP9kdfOI+wiYYHsPAIFjsAKgSyf1rsT1mMQCWmLYyE6UcCaKcDZKnHRUYZcRenQ9uaj57FKcEe8G0T61kdVcSZOWvEtq9DKR90pnDNVWAs+wAigJPg1fj4Gg6W/K7aiPLekPSQf+xkxRuKeNnMl2gVmNMIGRg5wFqj7gtHw++hOts5M+0dBgnIMzNkDe1BITwOS3gwLvtKb0Bo50bNLAww2gN9lejdAT0sdL7Ytbh3IGOuPP2fvynmtfSEUDoaIZHeD7Xqsm8Q0QaLNF3IPaxx+e1f/HSMH5k2TybLWDaZd8RUJqM50QyEKXRZZ/TqDKn36D75oSN/6+joIbS+rjD2cWKoL6ytWdWkcqP/+igAoXfkemh8PJZph+1/ljzqUqXJleJDJpisLOrSQgReKUiH/wAksHrILDcThLjHjA2rtMtW6bPPlEXWwWC0k2jcIrkMGUTfIlr+dwZrOaeuoUX7UIB2smbkejEyHoyBJ1Vv3/jHUKhLViMb36BMhGRdrWK5C7zzCLki0PeTdukUJMUd9hRUfScqwzeOeCivlHY6rVAeTVd+NhOdRtafYtYNL8ixr3Jvc9";

        //    string decryptedInput = EncryptionHelper.Decrypt(bodyContent, privateKey);
        //    if (decryptedInput != "")
        //    {
        //        var deserilizedFromJson = JsonConvert.SerializeObject(decryptedInput);
        //        byte[] requestData = Encoding.ASCII.GetBytes(deserilizedFromJson);
        //    }
        //    string id = "";
        //    if (!string.IsNullOrWhiteSpace(id))
        //    {
        //        int status = await dataSyncService.UpdateSyncStatus(id);
        //    }
        //    return new ApiResponse<string>((int)StatusCodes.Status400BadRequest, "Id not found");
        //}
    }
}
