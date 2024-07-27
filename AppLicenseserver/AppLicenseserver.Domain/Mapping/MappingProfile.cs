// <copyright file="MappingProfile.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
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
