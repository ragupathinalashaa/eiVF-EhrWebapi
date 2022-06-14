using AutoMapper;

namespace eIVF
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DTO.FHIR.DataSyncDTO, Entities.Models.DataSync>();
            CreateMap<Entities.Models.Patients, DTO.FHIR.PatientsDTO>();
            CreateMap<Entities.Models.DataSync, DTO.FHIR.DataSyncDTO>();
            CreateMap<Entities.Models.UpdateSummary, DTO.FHIR.UpdateSummaryDTO>();
            CreateMap<DTO.FHIR.UpdateSummaryDTO, Entities.Models.UpdateSummary>();
        }
    }
}
