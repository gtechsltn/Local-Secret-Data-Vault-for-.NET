using Dapper;
using LocalVaultWebApi.Data;
using LocalVaultWebApi.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace LocalVaultWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        private readonly DapperContext _dapperContext;

        public SecretController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        [HttpGet]
        public IActionResult CreateDatabase()
        {
            var result = 0;

           using var connection = _dapperContext.CreateConnection();
           try
            {
                string sql2 = "CREATE TABLE IF NOT EXISTS Secret (Id INTEGER PRIMARY KEY, key TEXT,value TEXT)";

                connection.Execute(sql2);
            }
            catch (Exception ex) 
            { 
            
            }
            return Ok(true);
        }

        [HttpGet]
        public IActionResult GetSecret(string key)
        {
            var result =new Secret();


            using (var connection = _dapperContext.CreateConnection())
            {              

                string sql = "SELECT * From Secret Where key=@key";

                result = connection.QueryFirstOrDefault<Secret>(sql, new { key });
            }

            return Ok(result);
        }

        [HttpPost]
        public IActionResult StoreSecret(SecretRequest request_data)
        {
            var result = 0;

            using (var connection = _dapperContext.CreateConnection())
            {
                
                string sql = "INSERT INTO Secret(Key,Value) VALUES (@Key,@Value)";

                result =  connection.Execute(sql, new { request_data.Key, request_data.Value });
            }

            return Ok(result);
        }

    }
}
