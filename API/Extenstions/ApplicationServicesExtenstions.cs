using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Intrfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extenstions
{
    public static class ApplicationServicesExtenstions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokkenService, TokkenService>();
            services.AddDbContext<DataContext>(options =>
          {
              options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
          });

            return services;

        }
    }
}