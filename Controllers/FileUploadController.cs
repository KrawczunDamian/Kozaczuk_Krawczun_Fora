using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace Fora2.Controllers
{

    public class FileUploadController : Controller
    {
        private readonly int maxSizeOfImaige = 100000;//100000B = 100kB
        private readonly byte maxWidthAndHeightOfImage = 128;

        private ApplicationUserManager _userManager;
        public FileUploadController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public FileUploadController()
        {
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult UploadAvatar()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadAvatar(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(file != null)
                    {
                        var userId = User.Identity.GetUserId();
                        var user = await UserManager.FindByIdAsync(userId);
                        var lastDot = file.FileName.ToString().LastIndexOf(@".");
                        string filePath = Path.Combine(Server.MapPath("~/UploadedFiles"),userId + file.FileName.ToString().Substring(lastDot)) ;
                        string userPath = Path.Combine("/UploadedFiles", userId + file.FileName.ToString().Substring(lastDot));
                        if (file.ContentLength>maxSizeOfImaige)
                        {
                            ViewBag.FileStatus = "File is too big, please upload an image up to " + maxSizeOfImaige / 1000 + " kB";
                            return View("UploadAvatar");
                        }
                        if (file.ContentType.Contains("image"))
                        {
                            var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
                            if(img.Width>maxWidthAndHeightOfImage || img.Height > maxWidthAndHeightOfImage)
                            {
                                ViewBag.FileStatus = "File resolution can at most be 128x128";
                                return View("UploadAvatar");
                            }
                            ViewBag.FileStatus = "File uploaded successfuly";
                            
                            user.AvatarURL = userPath;
                            var result = await UserManager.UpdateAsync(user);
                            if (result.Succeeded)
                            {
                                file.SaveAs(filePath);
                                return View();
                            }
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            
                            
                        }
                        else ViewBag.FileStatus = "Please upload an IMAGE file";
                        return View();

                    }
                   
                }
                catch (Exception)
                {
                    ViewBag.FileStatus = "Error while file uploading";
                }
            }
            return View("UploadAvatar");
        }
    }
}