using ASP.NET_Core.API.Data;
using ASP.NET_Core.API.Models.Domain;
using ASP.NET_Core.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP.NET_Core.API.Controllers
{
    //https:localhost:7189/api/Regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly APIDbContext dbContext;
        //inject configuration for jwt authentication
        private readonly IConfiguration configuration;
        public RegionsController(APIDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        //GET ALL REGIONS 
        //GET: https:localhost:7189/api/Regions
        [HttpGet]
        public IActionResult GetAll()
        {
            //get data from database - domain models
            var regionsDomain = dbContext.Regions.ToList();

            //map domain models to DTOs
            var regionsDto = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Code = regionDomain.Code,
                    Name = regionDomain.Name,
                    RegionImageUrl = regionDomain.RegionImageUrl
                });
            }

            //return DTO
            return Ok(regionsDomain);
        }

        //GET SINGLE REGION BY ID
        //GET: https:localhost:7189/api/Regions
        [Authorize]
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetbyId([FromRoute] Guid id)
        {
            //var region= dbContext.Regions.Find(id);  //find method only for id
            //get region domain model from database
            var regionDomain = dbContext.Regions.FirstOrDefault(x => x.Id == id);   //LINQ method for all

            if (regionDomain == null)
            {
                return NotFound();
            }

            //map/convert region domain model to region DTO
            var regionDto = new RegionDto
            {

                Id = regionDomain.Id,
                Code = regionDomain.Code,
                Name = regionDomain.Name,
                RegionImageUrl = regionDomain.RegionImageUrl
            };


            return Ok(regionDto);
        }

        //POST TO CREATE NEW REGION
        //POST: https:localhost:7189/api/Regions
        [HttpPost]
        public IActionResult Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //map or convert DTO to Domain Model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            //Use Domain model to create Region
            dbContext.Regions.Add(regionDomainModel);
            dbContext.SaveChanges();

            //map domain model to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl,
            };

            //build claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", regionDomainModel.Id.ToString()),
                new Claim("Code", regionDomainModel.Code),
                new Claim("Name", regionDomainModel.Name)
            };
            //create signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signingcreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //generate jwt token
            var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signingcreds);

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            //return response
            return CreatedAtAction(nameof(GetbyId), new { id = regionDomainModel.Id }, new
            {
                Region = regionDomainModel,
                Token = jwtToken
            });
        }
    }
}
