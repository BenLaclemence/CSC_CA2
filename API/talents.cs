using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using new_websub.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

using System.Net;
using System.Net.Mail;
using System.Configuration;
using Stripe;
using Microsoft.Extensions.Options;
using Stripe.BillingPortal;
using MySqlX.XDevAPI.Common;
using Amazon.S3;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;

using System.Net.Http;

using System.Xml;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace new_websub.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class talents : Controller
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = ""
        };
        IFirebaseClient client;
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("TalentsData")]
        public async Task<IActionResult> GetTalents()
        {
            var list = new List<talentsList>();

            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response =  client.Get("Talents");
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                if (data != null) {
                    foreach (var item in data)
                    {
                        list.Add(JsonConvert.DeserializeObject<talentsList>(((JProperty)item).Value.ToString()));
                    }
                }
               

            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                records = list
            });


        }//End of GetUsers web api
    }
}
