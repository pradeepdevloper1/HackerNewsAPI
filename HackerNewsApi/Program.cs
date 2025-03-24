
using HackerNewsApi.Services;

namespace HackerNewsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache(); 
            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowHackerNewsApp", builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowHackerNewsApp");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
