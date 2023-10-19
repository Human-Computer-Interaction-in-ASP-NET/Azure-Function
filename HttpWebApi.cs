using System.Net;
using FunctionSplash.Models.School;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace sahilrai.Function
{
    public class HttpWebApi
    {
        private readonly ILogger _logger;

        private readonly SchoolContext _context;

        public HttpWebApi(ILoggerFactory loggerFactory, SchoolContext context)
        {
            _logger = loggerFactory.CreateLogger<HttpWebApi>();
            _context = context;
        }

        [Function("HttpWebApi")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("GetStudents")]
        public HttpResponseData GetStudents([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP GET/posts trigger function processed a request inGetStudents().");
            var students = _context.Students.ToArray();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonConvert.SerializeObject(students));
            return response;
        }

        [Function("GetStudentById")]
        public HttpResponseData GetStudentById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("C# HTTP GET/posts trigger function processed a request inGetStudentById().");
            var student = _context.Students.Find(id);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonConvert.SerializeObject(student));
            return response;
        }

        [Function("AddStudent")]
        public HttpResponseData AddStudent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP POST/posts trigger function processed a request inAddStudent().");
            var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            _context.Students.Add(student);
            _context.SaveChanges();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonConvert.SerializeObject(student));
            return response;
        }

        [Function("UpdateStudent")]
        public HttpResponseData UpdateStudent([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("C# HTTP PUT/posts trigger function processed a request inUpdateStudent().");
            var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            student.StudentId = id;
            _context.Students.Update(student);
            _context.SaveChanges();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonConvert.SerializeObject(student));
            return response;
        }

        [Function("DeleteStudent")]
        public HttpResponseData DeleteStudent([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("C# HTTP DELETE/posts trigger function processed a request inDeleteStudent().");
            var student = _context.Students.Find(id);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            
            if (student != null) {
                _context.Students.Remove(student);
                _context.SaveChanges();
                response.WriteString(JsonConvert.SerializeObject(student));
            } else {
                response.WriteString(JsonConvert.SerializeObject("Student not found"));
            }
            return response;
        }
    }
}
