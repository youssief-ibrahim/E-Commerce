using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.Services.MappingProfile
{
    public class PictureURLResolver<TSource, TDestination> : IMemberValueResolver<TSource, TDestination, string, string>
    {
        private readonly IConfiguration cofig;

        public PictureURLResolver(IConfiguration cofig)
        {
            this.cofig = cofig;
        }
        public string Resolve(TSource source, TDestination destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
                return string.Empty;

            var BaseUrl = cofig["URL:BaseURL"];

            return $"{BaseUrl}/{sourceMember.TrimStart('/')}";
        }
    }
}
