using AutoMapper;

namespace eIVF
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<DTO.FHIR.PatientsDTO, Entities.Models.Patients>();
            CreateMap<Entities.Models.Patients, DTO.FHIR.PatientsDTO>();

            CreateMap<DTO.FHIR.PatientAddressDTO, Entities.Models.PatientAddress>();
            CreateMap<Entities.Models.PatientAddress, DTO.FHIR.PatientAddressDTO>();


        }
    }
}
