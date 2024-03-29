﻿using AutoMapper;
using CourseLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

            CreateMap<Models.AuthorForCreationDto, Entities.Author>();

            // 03/08/2022 02:40 pm - SSN - [20220307-2140] - [004] - M06-08 - Demo - Working with vendor-specific media types on input
            CreateMap<Models.AuthorForCreationWithDateOfDeathDTO, Entities.Author>();


            // 03/07/2022 11:09 am - SSN - [20220307-1105] - [002] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types
            CreateMap<Entities.Author, Models.AuthorFullDTO>();

        }
    }
}
