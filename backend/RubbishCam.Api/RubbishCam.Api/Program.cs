global using RubbishCam.Api.Exceptions;
using Microsoft.OpenApi.Models;
using RubbishCam.Api.Auth;
using RubbishCam.Api.Services;
using RubbishCam.Data;

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
		Scheme = "Bearer",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Description = "JWT Authorization header. Example: 'Authorization: Bearer {token}'",

		Reference = new OpenApiReference
		{
			Id = "Bearer",
			Type = ReferenceType.SecurityScheme
		}
	};

	c.AddSecurityDefinition( "Bearer", securityScheme );
	c.AddSecurityRequirement( new OpenApiSecurityRequirement()
	{
		{ securityScheme, Array.Empty<string>() }
	} );
} );

builder.Services.AddAuthentication( options =>
	 {
	 	options.DefaultScheme = "Bearer";
	 	options.DefaultAuthenticateScheme = "Bearer";
	 	options.DefaultChallengeScheme = "Bearer";
	 } )
	.AddScheme<TokenOptions, TokenAuthHandler>( "Bearer", options => { } );

builder.Services.AddNpgsql<AppDbContext>(
	 builder.Configuration.GetConnectionString( "postgresConnection" ),
	 pgob => pgob.MigrationsAssembly( "RubbishCam.Migrations.Pg" ),
	 ob => ob.UseLoggerFactory( LoggerFactory.Create( factoryBuilder => factoryBuilder.AddConsole() ) )
	);

_ = builder.Services.AddScoped<IUsersService, UsersService>();
_ = builder.Services.AddScoped<IAuthService, AuthService>();
_ = builder.Services.AddScoped<IFriendsService, FriendsService>();

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
