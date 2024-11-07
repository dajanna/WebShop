using AutoMapper;
using BillApplication.Dto;
using BillApplication.Models;

namespace BillApplication.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Racun, RacunDto>();
            CreateMap<RacunDto, Racun>();
         
            CreateMap<Proizvod, ProizvodDto>();
            CreateMap<ProizvodDto, Proizvod>();

            CreateMap<Status, StatusDto>();
            CreateMap<StatusDto, Status>();

            CreateMap<Stavke, StavkeDto1>();
            CreateMap<StavkeDto1, Stavke>();

        }
    }
}
