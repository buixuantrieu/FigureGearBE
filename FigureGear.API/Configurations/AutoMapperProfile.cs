using AutoMapper;
using FigureGear.Data.Domain;
using FigureGear.Service.Models;

namespace FigureGear.API.Configurations
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile() {
            CreateMap<UserModel, User>();
        }
    }
}
