using DTO.FHIR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using eIVF.Utility;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace eIVF.Utility
{
    public class FhirDataAccess
    {
        public static string FhirToken = "";
        private const string FHIR_URL_PATIENT = @"https://cloudpatientportal-clinic1.fhir.azurehealthcareapis.com/Patient";
        private const string FHIR_URL_PATIENT_WITH_COUNT = @"https://cloudpatientportal-clinic1.fhir.azurehealthcareapis.com/Patient?_count=200";
        private const string FHIR_URL_PATIENT_TODAY = @"https://cloudpatientportal-clinic1.fhir.azurehealthcareapis.com/Patient?_lastUpdated=2022-05-25";
        private const string FHIR_URL_AUTH = @"https://login.microsoftonline.com/97bf3fbc-4081-425b-a1eb-e355047b6e5d/oauth2/token";
        private const string FHIR_PORT_AUITH_PARAMETERS = @"grant_type=Client_Credentials&client_id=1e5fd98a-90e9-4f87-96ec-57a5a08f8452&&client_secret=P3Q7Q~m4ljwzrgcvKJ2VyNoBarpfBT1Omq_46&resource=https://cloudpatientportal-clinic1.fhir.azurehealthcareapis.com";

        private static List<FhirPatientModel> FhirPatientUpdatedList = null;
        private static async Task<string> GetApiToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
             
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(FHIR_URL_AUTH));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                request.Content = new StringContent(FHIR_PORT_AUITH_PARAMETERS, Encoding.UTF8);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var bodyContent = await response.Content.ReadAsStringAsync();
                    if (bodyContent != "")
                    {
                        var resultObj = JObject.Parse(bodyContent);
                        if (resultObj != null)
                        {
                            if (resultObj.GetValue("access_token").Value<string>() != "")
                            {
                                string tokenValue = resultObj.GetValue("access_token").Value<string>();
                                // WriteToken(tokenValue);
                                return tokenValue;

                            }
                            else
                            {
                                return "NOTOKEN";
                            }
                        }
                    }
                }
            }

            return string.Empty;

        }
        public static async Task<ApiResponse<Bundle>> GetAllFromPatient()
        {

            ApiResponse<Bundle> apiResponse = new ApiResponse<Bundle>();
            FhirPatientUpdatedList = new List<FhirPatientModel>();

            FhirToken = await GetApiToken();
            if (FhirToken != "NOTOKEN")
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(FHIR_URL_PATIENT_WITH_COUNT));
                       
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", FhirToken);
                      //  request.Content = new StringContent("_lastUpdated=2022-05-25", Encoding.UTF8);
                        HttpResponseMessage response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            if (result != null)
                            {
                                FhirJsonParser fjp = new FhirJsonParser(); /* there is a FhirXmlParser as well */
                                /* You may need to Parse as something besides a Bundle depending on the return payload */
                                Bundle bund = fjp.Parse<Bundle>(result);
                                if (null != bund)
                                {
                                    //Bundle.EntryComponent ec = bund.Entry.FirstOrDefault();
                                    foreach (Bundle.EntryComponent entryComponent in bund.Entry)
                                    {
                                        
                                        if (null != entryComponent && null != entryComponent.Resource)
                                        {
                                        
                                            /* again, this may be a different kind of object based on which rest url you hit */
                                            Patient pat = entryComponent.Resource as Patient;
                                            //Identifier identifier = null;
                                            Hl7.Fhir.Model.Identifier identifier = pat.Identifier.FirstOrDefault();
                                            if (identifier != null)
                                            {
                                                string patientName = pat.Name.FirstOrDefault().Family;
                                                foreach(string sGiven in pat.Name.FirstOrDefault().Given)
                                                {
                                                    patientName = patientName + " " + sGiven; 
                                                }
                                                FhirPatientUpdatedList.Add(new FhirPatientModel { PatientMRN = pat.Identifier.FirstOrDefault().Value, Key = pat.Id, PatientName= patientName });
                                            }
                                        }
                                    }
                                }

                                apiResponse.StatusCode = StatusCodes.Status200OK;
                                apiResponse.Message = "Get all records successfuly Total Record:" + bund.Entry.Count();
                                var serData = JsonConvert.SerializeObject(result);
                                apiResponse.Data = bund; //JsonConvert.DeserializeObject<string>(serData);
                            }
                        }
                        else
                        {
                            var resultFailed = await response.Content.ReadAsStringAsync();
                            if (resultFailed != null)
                            {
                                apiResponse.StatusCode = StatusCodes.Status400BadRequest;
                                var serData = JsonConvert.SerializeObject(resultFailed);
                                apiResponse.Data = null; //JsonConvert.DeserializeObject<string>(serData);
                                apiResponse.Message = JsonConvert.DeserializeObject<string>(serData);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    apiResponse.Message = ex.Message;

                }

            }

            return apiResponse;
        }
   
        public static async Task<ApiResponse<Patient>> GetPatinetById(string patientKey)
        {

            ApiResponse<Patient> apiResponse = new ApiResponse<Patient>();


            FhirToken = await GetApiToken();
            if (FhirToken != "NOTOKEN")
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        var request = new HttpRequestMessage(HttpMethod.Get, new Uri(FHIR_URL_PATIENT+"/" +patientKey));
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", FhirToken);
                        //request.Content = new StringContent(patientKey, Encoding.UTF8);
                        HttpResponseMessage response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            if (result != null)
                            {
                                apiResponse.StatusCode = StatusCodes.Status200OK;
                                apiResponse.Message = "Get record successfuly";
                                var serData = JsonConvert.SerializeObject(result);

                                FhirJsonParser fjp = new FhirJsonParser();
                                Resource resource = fjp.Parse<Resource>(result);
                                if (null != resource)
                                {
                                    Patient pat = resource as Patient;
                                    apiResponse.Data = pat;
                                }
                                
                                //apiResponse.Data = JsonConvert.DeserializeObject<string>(serData);
                            }
                        }
                        else
                        {
                            var resultFailed = await response.Content.ReadAsStringAsync();
                            if (resultFailed != null)
                            {
                                apiResponse.StatusCode = StatusCodes.Status400BadRequest;
                                var serData = JsonConvert.SerializeObject(resultFailed);
                                apiResponse.Data = null;  //JsonConvert.DeserializeObject<string>(serData);
                                apiResponse.Message = JsonConvert.DeserializeObject<string>(serData);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    apiResponse.Message = ex.Message;

                }

            }

            return apiResponse;
        }

        public static async Task<ApiResponse<string>> UpdatePatient(string id)
        {
            ApiResponse<string> apiResponse = new ApiResponse<string>();
            const string TRANSIENT_SERVICE_URL = "https://eivf-datasynchronizationapi.azurewebsites.net/api/Patients/Update";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                string PUT_PATIENT_URL = $"{TRANSIENT_SERVICE_URL}?id={id}";

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(PUT_PATIENT_URL));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (result != null)
                    {
                        apiResponse.StatusCode = StatusCodes.Status200OK;
                        apiResponse.Message = "Record updated successfuly";
                        
                    }
                }
                else
                {
                    var resultFailed = await response.Content.ReadAsStringAsync();
                    if (resultFailed != null)
                    {
                        apiResponse.StatusCode = StatusCodes.Status400BadRequest;
                        apiResponse.Message = "";
                    }

                }
            }
            return apiResponse;
        }

    }
    
}
