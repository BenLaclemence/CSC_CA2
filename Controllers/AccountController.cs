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

using Microsoft.Extensions.Options;

using MySqlX.XDevAPI.Common;
using Amazon.S3;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;

using System.Net.Http;

using System.Xml;

using Microsoft.Extensions.Logging;



namespace new_websub.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

       
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "",
            BasePath = ""
        };
        IFirebaseClient client;
     

        private Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

     
        public string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for(i = 0; i<arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        // my account page
     
      
      
   

     



       


     
     

      

     


    

      
      
        public IActionResult TalentsList() {
            HttpContext.Session.SetInt32("isLoggedIn", 1);
            HttpContext.Session.SetString("isPaid", "Paid");
            HttpContext.Session.SetString("User", "benlaclemence2013@gmail.com");
            ViewBag.isPaid = HttpContext.Session.GetString("isPaid");
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("talentsID");
            
            HttpContext.Session.SetString("TalentsID",response.Body.ToString());
            return View();
        }
        public ActionResult CreateTalent()
        {
            ViewBag.isPaid = HttpContext.Session.GetString("isPaid");
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }
        public ActionResult Comment() {
            ViewBag.isPaid = HttpContext.Session.GetString("isPaid");
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            return View();
        }
   

        string AWS_accessKey = "";
        string AWS_secretKey = "";
        string AWS_bucketName = "";
        string AWS_defaultFolder = "";

        [HttpPost]
        public async Task<IActionResult> UploadNewFileAsync(IFormFile myfile, string subFolder)
        {
            var result = await UploadFileToAWSAsync(myfile, subFolder);
            HttpContext.Session.SetString("UploadedLink", result);

            return RedirectToAction(nameof(UploadToFirebase));

        }
        [HttpPost]
        public async Task<IActionResult> UpdateFileAsync(IFormFile myfile, string subFolder)
        {
            var result = await UploadFileToAWSAsync(myfile, subFolder);
            HttpContext.Session.SetString("UploadedLink", result);
            
           
            return RedirectToAction(nameof(UploadUpdatedTalent));

        }

        protected async Task<string> UploadFileToAWSAsync(IFormFile myfile, string subFolder = "")
        {
            var result = "";
            try
            {
                var s3Client = new AmazonS3Client(AWS_accessKey, AWS_secretKey, Amazon.RegionEndpoint.APSoutheast1);
                var bucketName = AWS_bucketName;
                var keyName = AWS_defaultFolder;
                if (!string.IsNullOrEmpty(subFolder))
                    keyName = keyName + "/" + subFolder.Trim();
                keyName = keyName + "/" + myfile.FileName;

                var fs = myfile.OpenReadStream();
                var request = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = fs,
                    ContentType = myfile.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };
                await s3Client.PutObjectAsync(request);

                result = string.Format("http://{0}.s3.amazonaws.com/{1}", bucketName, keyName);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public IActionResult UpdateTalent(string TalentCode)
        {
            ViewBag.isPaid = HttpContext.Session.GetString("isPaid");
            ViewBag.isLoggedIn = HttpContext.Session.GetInt32("isLoggedIn");
            HttpContext.Session.SetString("UpdateCode", TalentCode);
            return View();
        }
        public ActionResult DeleteTalent(string TalentCode) {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Talents/" + TalentCode);
            return Redirect("http://18.139.166.95/CSC_Ben");
        }

        public ActionResult UploadToFirebase()
        {
            
            client = new FireSharp.FirebaseClient(config);
            string link = HttpContext.Session.GetString("UploadedLink");
            string id = HttpContext.Session.GetString("TalentsID");
            
           
            
            talentsList T = new talentsList();
            T.imagelink = link;
            T.TalentId = int.Parse(id) + 1;
            var data = T;
            
            PushResponse response = client.Push("Talents/",data);
            data.TalentCode = response.Result.name;
            SetResponse response3 = client.Set("Talents/"+data.TalentCode,data);
            SetResponse response1 = client.Set("talentsID", int.Parse(id) + 1);

            return Redirect("http://18.139.166.95/CSC_Ben");
        }
        public ActionResult UploadUpdatedTalent()
        {

            client = new FireSharp.FirebaseClient(config);
            string link = HttpContext.Session.GetString("UploadedLink");
            string id = HttpContext.Session.GetString("TalentsID");
            string code = HttpContext.Session.GetString("UpdateCode");


            talentsList T = new talentsList();
            T.imagelink = link;
            T.TalentId = int.Parse(id);
            T.TalentCode = code;
            var data = T;

            SetResponse response = client.Set("Talents/"+code, data);
            
            
        

            return Redirect("http://18.139.166.95/CSC_Ben");
        }

    }
}