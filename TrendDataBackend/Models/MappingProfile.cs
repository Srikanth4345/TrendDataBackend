using AutoMapper;
using TrendDataBackend.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeviceEntity, DeviceDTO>();
    }
}