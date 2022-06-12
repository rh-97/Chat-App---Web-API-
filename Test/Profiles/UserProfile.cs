using Test.Models;
using AutoMapper;

namespace Test.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserRegister, User>();
    }
    
}
