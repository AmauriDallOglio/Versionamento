using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using Versionamento.Api.Util;

namespace Versionamento.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // --- Chave secreta JWT ---
            var key = "minha_chave_secreta_super_segura";


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,   // N�o valida quem emitiu o token (Issuer). Aceita qualquer emissor.
                    ValidateAudience = false,   // N�o valida para quem o token foi emitido (Audience). Aceita qualquer p�blico.
                    ValidateLifetime = true,     // Verifica se o token n�o expirou. Tokens expirados ser�o rejeitados.
                    ValidateIssuerSigningKey = true,     // Verifica se o token foi assinado com a chave correta.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    //Tratamento customizado para 401 Unauthorized e 403 Forbidden com JSON v�lido;
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = JsonSerializer.Serialize(new
                        {
                            code = 401,
                            message = "Token inv�lido ou expirado. Fa�a login novamente."
                        });

                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = JsonSerializer.Serialize(new
                        {
                            code = 403,
                            message = "Acesso negado. Voc� n�o possui permiss�o para acessar este recurso."
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            });


            // --- Versionamento de API ---
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // --- Controllers ---
            builder.Services.AddControllers();

            // --- Authorization policies por vers�o ---
            //Policies de autoriza��o espec�ficas por vers�o (AcessoV2, AcessoV3);
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AcessoV2", policy => policy.RequireClaim("AcessoApi", "v2"));
                options.AddPolicy("AcessoV3", policy => policy.RequireClaim("AcessoApi", "v3"));
            });

            // --- Swagger ---
            builder.Services.AddEndpointsApiExplorer();

            //Swagger configurado para m�ltiplas vers�es e suporte a JWT.
            builder.Services.AddSwaggerGen(options =>
            {
                //Versionamento de API (v1, v2, v3 via segmento na URL);
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1" });
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "API v2", Version = "v2" });
                options.SwaggerDoc("v3", new OpenApiInfo { Title = "API v3", Version = "v3" });

                // JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // --- Swagger UI ---
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();


            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            //    c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
            //    c.SwaggerEndpoint("/swagger/v3/swagger.json", "API v3");
            //});


            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            });

            // --- Middlewares ---
            app.UseAuthentication();
            app.UseAuthorization();

            // --- Map Controllers ---
            app.MapControllers();


            app.Use(async (context, next) =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                // Ignora a rota de gera��o de token
                if (context.Request.Path.StartsWithSegments("/api/token"))
                {
                    await next();
                    return;
                }

                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var usuario = JwtHelper.DecodificarToken(token);

                    if (usuario == null)
                    {
                        context.Response.StatusCode = 401; // N�o autorizado
                        await context.Response.WriteAsync("{\"mensagem\":\"Token inv�lido ou expirado\"}");
                        return;
                    }

                    // Voc� pode salvar o usu�rio no HttpContext para usar nos controllers
                    context.Items["UsuarioToken"] = usuario;
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("{\"mensagem\":\"Token n�o informado\"}");
                    return;
                }

                await next();
            });


            app.Run();
        }
    }
}
