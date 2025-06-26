// <copyright file="MappingProfile.cs" company="Sascha Manns">
// Copyright (c) 2025 Sascha Manns.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the “Software”), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis

using System;

using AppLicenseserver.Entity;

using AutoMapper;

namespace AppLicenseserver.Domain.Mapping
{
    /// <summary>
    /// Profile for Automapping.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public partial class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<AccountViewModel, Account>();
            CreateMap<Account, AccountViewModel>();
            CreateMap<UserViewModel, User>()
                .ForMember(dest => dest.DecryptedPassword, opts => opts.MapFrom(src => src.Password))
                .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => string.Join(";", src.Roles)));
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Password, opts => opts.MapFrom(src => src.DecryptedPassword))
                .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => src.Roles.Split(";", StringSplitOptions.RemoveEmptyEntries)));

            // call code in partial scaffolded function
            SetAddedMappingProfile();
        }

        // to call scaffolded method
        partial void SetAddedMappingProfile();
    }
}
