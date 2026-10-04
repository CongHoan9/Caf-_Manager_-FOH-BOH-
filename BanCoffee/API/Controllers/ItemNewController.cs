using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemNewController(IConfiguration configuration) : Controller
    {
       private readonly string constring = configuration["ConnectionStrings:DefaultConnection"];

        [AllowAnonymous]
        [Route("get-by-id/{id}")]
        [HttpGet]
        public ItemModel GetDatabyID(string id)
        {
            SqlConnection con = new(constring);
            con.Open();
            SqlCommand cmd = new()
            {
                Connection = con,
                CommandType = CommandType.Text,
                CommandText = "Select * from item where item_id ='" + id + "'"
            };
            SqlDataAdapter da = new(cmd);
            DataTable tb = new();
            da.Fill(tb);
            if (tb.Rows.Count == 0)
            {
                return null; 
            }
            var row = tb.Rows[0];
            return new ItemModel
            {
                item_name = row["item_name"]?.ToString(),
                item_id = row["item_id"]?.ToString(),
                item_group_id = row["item_group_id"]?.ToString()
            };
        }
    }
}




