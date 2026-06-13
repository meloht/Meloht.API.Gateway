using Meloht.API.Gateway.BackendAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Meloht.API.Gateway.BackendAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        /// <summary>
        /// QueryString
        /// </summary>
        /// <returns></returns>
        [HttpGet("query")]
        public IActionResult Test()
        {
            string? id = Request.Query["id"];
            string? name = Request.Query["name"];

            return Ok(new { id, name });
        }

        /// <summary>
        /// x-www-form-urlencoded
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginData request)
        {
            return Ok(new
            {
                request.Username,
                request.Password
            });
        }

        [HttpPost("loginData")]
        public IActionResult LoginData([FromBody] LoginData request)
        {
            return Ok(new
            {
                request.Username,
                request.Password
            });
        }

        [HttpPost("test")]
        public IEnumerable<WeatherForecast> Test([FromBody] LoginData request)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// form data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public IActionResult Save([FromForm] LoginData request)
        {
            return Ok(request);
        }

        [HttpPost("uploadOne")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("file is empty");
            }
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, file.FileName);

            await using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            return Ok(new
            {
                file.FileName,
                file.Length
            });
        }

        [HttpPost("uploadOneBinary")]
        public async Task<IActionResult> UploadBinary()
        {
            string filePath = Path.Combine("uploads", Guid.NewGuid().ToString());

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            await using var fileStream = System.IO.File.Create(filePath);

            await Request.Body.CopyToAsync(fileStream);

            return Ok(new
            {
                FilePath = filePath,
                Size = fileStream.Length
            });
        }



        [HttpPost("uploads")]
        public async Task<IActionResult> Uploads(List<IFormFile> files)
        {
            var result = new List<string>();

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var file in files)
            {
                var path = Path.Combine("uploads", file.FileName);

                await using var stream = new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);

                result.Add(file.FileName);
            }

            return Ok(result);
        }

        [HttpPost("uploadForm")]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            return Ok(new
            {
                request.UserId,
                request.Remark,
                request.File.FileName
            });
        }


        [HttpGet("preview/pdf")]
        public IActionResult PreviewPdf()
        {
            var filePath = @"D:\code\TestFiles\test.pdf";

            return PhysicalFile(filePath, "application/pdf");

        }

        [HttpGet("preview/image")]
        public IActionResult PreviewImage()
        {
            return PhysicalFile(@"D:\code\TestFiles\logo.png", "image/png");
        }

        [HttpGet("download")]
        public IActionResult Download()
        {
            var stream = new FileStream(
                @"D:\code\TestFiles\bigfile.zip",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return File(stream, "application/zip", "bigfile.zip", enableRangeProcessing: true);




        }
    }
}
