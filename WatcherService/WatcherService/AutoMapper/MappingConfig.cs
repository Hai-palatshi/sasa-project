using AutoMapper;
using WatcherService.Models;
using WatcherService.Models.DTO;

namespace WatcherService.AutoMapper
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<UpdateWatcherSettingsRequestDto, UpdateWatcherSettingsRequest>().ReverseMap();
           // CreateMap<FileMetadata, FileMetadata>().ReverseMap();
        }
    }
}
