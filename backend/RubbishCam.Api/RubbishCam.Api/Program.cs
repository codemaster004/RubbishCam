global using RubbishCam.Api.Exceptions;
using Microsoft.OpenApi.Models;
using RubbishCam.Api;
using RubbishCam.Api.Auth;
using RubbishCam.Api.Services;
using RubbishCam.Data;
using RubbishCam.Data.Repositories;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
	c.SwaggerDoc( "v1", new OpenApiInfo { Title = "HackathonE1.Api", Version = "v1" } );

	var securityScheme = new OpenApiSecurityScheme
	{
		Scheme = Constants.Auth.BearerScheme,
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Description = "JWT Authorization header. Example: 'Authorization: Bearer {token}'",

		Reference = new OpenApiReference
		{
			Id = Constants.Auth.BearerScheme,
			Type = ReferenceType.SecurityScheme
		}
	};

	c.AddSecurityDefinition( Constants.Auth.BearerScheme, securityScheme );
	c.AddSecurityRequirement( new OpenApiSecurityRequirement()
	{
		{ securityScheme, Array.Empty<string>() }
	} );
} );

builder.Services.AddAuthentication( options =>
	 {
	 	options.DefaultScheme = Constants.Auth.BearerScheme;
	 	options.DefaultAuthenticateScheme = Constants.Auth.BearerScheme;
	 	options.DefaultChallengeScheme = Constants.Auth.BearerScheme;
	 } )
	.AddScheme<TokenOptions, TokenAuthHandler>( Constants.Auth.BearerScheme, options => { } );

builder.Services.AddNpgsql<AppDbContext>(
	 builder.Configuration.GetConnectionString( "postgresConnection" ),
	 pgob => pgob.MigrationsAssembly( "RubbishCam.Migrations.Pg" ),
	 ob => ob.UseLoggerFactory( LoggerFactory.Create( factoryBuilder => factoryBuilder.AddConsole() ) )
	);

_ = builder.Services.AddScoped<IUsersService, UsersService>();
_ = builder.Services.AddScoped<IAuthService, AuthService>();
_ = builder.Services.AddScoped<IFriendsService, FriendsService>();

_ = builder.Services.AddScoped<IUserRepository, UserRepository>();
_ = builder.Services.AddScoped<ITokenRepository, TokenRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

_ = app.UseHttpsRedirection();

_ = app.UseAuthentication();
_ = app.UseAuthorization();

_ = app.MapControllers();

app.Run();
