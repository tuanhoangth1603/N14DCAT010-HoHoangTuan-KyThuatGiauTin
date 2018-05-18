using GoogleDriveRestAPI_v3.Models;
using SteganoWave;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace GoogleDriveRestAPI_v3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(UserAccount account)
        {

            using (MVCEntities db = new MVCEntities())
            {
                var obj = db.UserAccounts.Where(a => a.Username.Equals(account.Username) && a.Password.Equals(account.Password)).FirstOrDefault();
                if (obj != null)
                {
                    Session["UserID"] = obj.UserID.ToString();
                    Session["Username"] = obj.Username.ToString();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is wrong.");
                }
            }
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                using (MVCEntities db = new MVCEntities())
                {
                    var obj = db.UserAccounts.Where(a => a.Username.Equals(account.Username)).FirstOrDefault();
                    var objEmail = db.UserAccounts.Where(a => a.Email.Equals(account.Email)).FirstOrDefault();
                    if (obj != null)
                    {
                        ModelState.AddModelError("", "Username is taken. Try another.");
                    }
                    else if (objEmail != null)
                    {
                        ModelState.AddModelError("", "Email is taken. Try another.");
                    }
                    else
                    {
                        db.UserAccounts.Add(account);
                        db.SaveChanges();
                        ModelState.Clear();
                        ModelState.AddModelError("", "Successfully registered.");
                    }
                }
            }
            return View();
        }

        
        
        [HttpGet]
        public ActionResult Index()
        {
            
            return View(GoogleDriveFilesRepository.GetDriveFiles());
        }

        [HttpPost]
        public ActionResult DeleteFile(GoogleDriveFiles file)
        {
            GoogleDriveFilesRepository.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }
         
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            GoogleDriveFilesRepository.FileUpload(file);
            //Watermark wt = new Watermark();
            //wt.btnHide_Click();

            return RedirectToAction("GetGoogleDriveFiles");
        }
        

        public void DownloadFile(string id)
        {
            if (Session["Username"].Equals("admin"))
            {
                string FilePath = GoogleDriveFilesRepository.DownloadGoogleFile(id);

                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
                Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
                Response.End();
                Response.Flush();
            }
            else
            {
                RedirectToAction("Login");
            }
            
        }
        
    }
}