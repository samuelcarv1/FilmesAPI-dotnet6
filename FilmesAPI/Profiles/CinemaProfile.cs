using AutoMapper;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;

namespace FilmesAPI.Profiles;

public class CinemaProfile : Profile
{
    public CinemaProfile()
    {   
        CreateMap<CreateCinemaDto, Cinema>();
        CreateMap<UpdateCinemaDto, Cinema>();
        //Mostrando a minha aplicação de como converter um Cinema a um ReadCinemaDto
        CreateMap<Cinema, ReadCinemaDto>().ForMember(dto => dto.Endereco, opt => opt.MapFrom(cDto => cDto.Endereco))
                                          .ForMember(dto => dto.Sessoes, opt => opt.MapFrom(cDto => cDto.Sessoes));
    }
}
