
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace TESTWEBAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            //builder.Services.AddControllers(options =>
            //{
            //    // Register known types
            //    var xmlInputFormatter = new XmlDataContractSerializerInputFormatter(options);
            //    xmlInputFormatter.SerializerSettings.KnownTypes = new[] { typeof(float[]) };
            //    options.InputFormatters.Insert(0, xmlInputFormatter);


            //    // Check for the index position of a known type
            //    var knownType = typeof(float[]);
            //    var index = Array.IndexOf(xmlInputFormatter.SerializerSettings.KnownTypes.ToArray(), knownType);
            //    if (index >= 0)
            //    {
            //        Console.WriteLine($"Known type {knownType} found at index {index} in XmlDataContractSerializerInputFormatter.");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Known type {knownType} not found in XmlDataContractSerializerInputFormatter.");
            //    }

            //}).AddXmlDataContractSerializerFormatters() ;

            builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();

         //   MODELBINDERPROVIDER
            builder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new XmlToDictionaryModelBinderProvider());
            });


            // builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();


            // Add custom XML request middleware
            // app.UseXmlRequestMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            // app.UseMiddleware<XmlToDictionaryMiddleware>();

            app.Run();
        }
    }
}
