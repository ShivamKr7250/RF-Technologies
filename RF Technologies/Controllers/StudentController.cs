using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Utility;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Security.Claims;
using RF_Technologies.Model.VM;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using System.Net;
//using System.Net.Mail;
using MailKit.Net.Smtp;


namespace RF_Technologies.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public RegistrationVM RegistrationVM { get; set; }
        public StudentController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var registration = _unitOfWork.RegistrationForm.GetAll();
            return View(registration);
        }

        public IActionResult StudentRegistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StudentRegistration(RegistrationForm obj)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.User.Get(u => u.Id == userId);
            obj.UserId = userId;
            obj.RegistrationDate = DateTime.Now;
            obj.Status = SD.StatusPending;

            _unitOfWork.RegistrationForm.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "The Registration has been done successfully.";
            return RedirectToAction("RedirectPage", "Student", new { registrationId = obj.ID });

        }

        public IActionResult RedirectPage(int registrationId)
        {
            var applicationId = "RFIN" + DateTime.Now.Year + registrationId.ToString();
            return View((object)applicationId);
        }

        public IActionResult RegistrationUpdate(int registrationId)
        {
            RegistrationForm? obj = _unitOfWork.RegistrationForm.Get(u => u.ID == registrationId);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }


        [HttpPost]
        public IActionResult RegistrationUpdate(RegistrationForm obj)
        {
            if (ModelState.IsValid && obj.ID > 0)
            {
                if (obj.Status == SD.StatusApproved)
                {
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
                    obj.StartDate = currentDate.AddDays(2);

                    DateOnly starDate = DateOnly.FromDateTime(DateTime.Now);
                    obj.EndDate = starDate.AddMonths(1);
                }
                _unitOfWork.RegistrationForm.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "The Registration has been updated successfully.";
                return RedirectToAction("Index", "Home");
            }
            return View(obj);
        }


        //public IActionResult Delete(int registrationId)
        //{
        //    RegistrationForm? obj = _unitOfWork.RegistrationForm.Get(u => u.ID == registrationId);

        //    //Villa? obj = _db.Villas.Find(villaId);
        //    //var VillaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
        //    if (obj is null)
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    return View(obj);
        //}

        //[HttpPost]
        //public IActionResult Delete(RegistrationForm obj)
        //{
        //    RegistrationForm? objFromDb = _unitOfWork.RegistrationForm.Get(u => u.ID == obj.ID);
        //    if (objFromDb is not null)
        //    {
        //        _unitOfWork.RegistrationForm.Remove(objFromDb);
        //        _unitOfWork.Save();
        //        TempData["error"] = "The Registration has been deleted successfully.";
        //        return RedirectToAction("Index", "Student");
        //    }
        //    TempData["error"] = "The Registration has could not be deleted.";
        //    return View();
        //}

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Approve(RegistrationForm obj)
        {
            _unitOfWork.RegistrationForm.UpdateStatus(obj.ID, SD.StatusApproved);
            _unitOfWork.Save();
            // Send email notification
            SendEmailNotificationForApprove(obj.Email,obj.Name, obj.ID);
            TempData["success"] = "Registration Updated Successfully";
            return RedirectToAction(nameof(RegistrationUpdate), new { registrationId = obj.ID });
        }

        private void SendEmailNotificationForApprove(string toEmail, string name, int registrationId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RF Technologies", "internship@rftechnologies.cloud"));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = "Your Internship Request has been Approved";

            var body = $@"
    <p>Dear {name},</p>

    <p>Your internship request has been approved. Congratulations!</p>

    <p><strong>Welcome to the RF Technologies Internship Program!</strong></p>

    <p>We are delighted to offer you the Internship Position at RF Technologies.</p>

    <p><strong>Intern ID: RFIN2024{registrationId}</strong></p>

    <p><strong>Stay Connected:</strong></p>
    <ul>
        <li>Join Our Telegram Group: Get the latest updates by joining our Telegram group</li>
        <li>Follow Us on LinkedIn: Stay updated with our LinkedIn page</li>
    </ul>

    <p>Please join the above channels to stay informed on daily updates.</p>

    <p><strong>Key Points for Your Internship:</strong></p>
    <ul>
        <li>Enhance Your LinkedIn Profile: Keep your LinkedIn profile updated. Share your achievements, such as the Offer Letter and Internship Completion Certificate, tagging @RFTechnologies and using #RFTechnologies.</li>
        <li>Original Work Requirement: All submitted projects or code must be your own work. Plagiarism will result in termination of the internship and a ban from future opportunities.</li>
        <li>Showcase Your Work: Share a video presentation of your completed tasks on LinkedIn. Tag @RFTechnologies and use #RFTechnologies.</li>
        <li>GitHub Repository: Create and maintain a GitHub repository named ""RFTechnologies"" for all your internship tasks. The repository link will be requested via email later.</li>
    </ul>

    <p><strong>Important Instructions:</strong></p>
    <ul>
        <li>Use the provided links for guidance and develop your unique solutions.</li>
        <li>There are no restrictions on programming languages.</li>
    </ul>

    <p><strong>Additional Guidelines:</strong></p>
    <ul>
        <li>Carefully read all instructions and submission guidelines provided in the task PDFs.</li>
        <li>Completing all assigned tasks may increase your chances of an extended internship with stipends, although this is not guaranteed. Ensure your code is original.</li>
        <li>Consistently update your LinkedIn profile with your achievements, tagging @RFTechnologies and using #RFTechnologies.</li>
        <li>Maintain originality in your work. Copied projects/code will result in internship termination and disqualification from future opportunities.</li>
        <li>Share a video of your completed tasks on LinkedIn, tagging @RFTechnologies and using #RFTechnologies.</li>
        <li>Keep a separate GitHub repository named ""RFTechnologies"" for all tasks. You will be asked to share the repository link via email.</li>
    </ul>

    <p>Once again, congratulations on your selection!</p>

    <p><strong>Contact Us:</strong></p>
    <ul>
        <li>Email: <a href='mailto:internship@rftechnologies.cloud'>internship@rftechnologies.cloud</a></li>
        <li>Telegram: <a href='https://t.me/rf_technologies'>https://t.me/rf_technologies</a></li>
        <li>LinkedIn: <a href='https://www.linkedin.com/company/rf-technology-ltd/'>https://www.linkedin.com/company/rf-technology-ltd/</a></li>
        <li>Phone No: +91-9905942199</li>
    </ul>

    <p>Best Regards,<br/>
    Team RF Technologies</p>

    <p><strong>Offer Letter Download Link:</strong> <a href='http://rftechnologies.cloud/Student?status=Approved'>Download Offer Letter</a></p>"; // Replace with actual offer letter download link

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.zoho.in", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("internship@rftechnologies.cloud", "Internship@9304"); // Use your actual email and password

                client.Send(message);
                client.Disconnect(true);
            }
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult InternshipComplete(RegistrationForm obj, int internshipId)
        {
            var registration = _unitOfWork.RegistrationForm.Get(u => u.ID == internshipId);
            if (registration is not null)
            {
                registration.Status = SD.StatusCompleted;
            }
            _unitOfWork.RegistrationForm.Update(registration);
            _unitOfWork.Save();
            SendEmailNotificationFoComplete(registration.Email, registration.Name, registration.ID);
            TempData["success"] = "Registration Updated Successfully";
            //return View("Index", "Student");
            return RedirectToAction(nameof(RegistrationUpdate), new { registrationId = obj.ID });
        }

        private void SendEmailNotificationFoComplete(string toEmail, string name, int registrationId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RF Technologies", "internship@rftechnologies.cloud"));
            message.To.Add(new MailboxAddress(name, toEmail));
            message.Subject = "Your Internship has been Completed";

            var body = $@"
<p>Dear {name},</p>

<p>Congratulations on successfully completing your internship at RF Technologies!</p>

<p><strong>Welcome to the RF Technologies Internship Program!</strong></p>

<p>We are delighted to offer you the Internship Position at RF Technologies.</p>

<p><strong>Intern ID: RFIN2024{registrationId}</strong></p>

<p><strong>Stay Connected:</strong></p>
<ul>
    <li>Join Our Telegram Group: Get the latest updates by joining our <a href='https://t.me/rf_technologies'>Telegram group</a>.</li>
    <li>Follow Us on LinkedIn: Stay updated with our <a href='https://www.linkedin.com/company/rf-technology-ltd/'>LinkedIn page</a>.</li>
</ul>

<p>Please join the above channels to stay informed on daily updates.</p>

<p><strong>Key Points for Your Internship:</strong></p>
<ul>
    <li>Enhance Your LinkedIn Profile: Keep your LinkedIn profile updated. Share your achievements, such as the Offer Letter and Internship Completion Certificate, tagging @RFTechnologies and using #RFTechnologies.</li>
    <li>Original Work Requirement: All submitted projects or code must be your own work. Plagiarism will result in termination of the internship and a ban from future opportunities.</li>
    <li>Showcase Your Work: Share a video presentation of your completed tasks on LinkedIn. Tag @RFTechnologies and use #RFTechnologies.</li>
    <li>GitHub Repository: Create and maintain a GitHub repository named ""RFTechnologies"" for all your internship tasks. The repository link will be requested via email later.</li>
</ul>

<p><strong>Certificate Download:</strong></p>
<p>You can download your Internship Completion Certificate from the portal. Please follow these steps:</p>
<ul>
    <li>Log in to the portal: <a href='http://rftechnologies.cloud/'>RF Technologies Portal</a></li>
    <li>Navigate to the 'Manage Internship' tab</li>
    <li>Go to the 'Completed' section</li>
    <li>Check your details and download the certificate</li>
</ul>

<p>Once again, congratulations on your selection and successful completion!</p>

<p><strong>Contact Us:</strong></p>
<ul>
    <li>Email: <a href='mailto:internship@rftechnologies.cloud'>internship@rftechnologies.cloud</a></li>
    <li>Telegram: <a href='https://t.me/rf_technologies'>https://t.me/rf_technologies</a></li>
    <li>LinkedIn: <a href='https://www.linkedin.com/company/rf-technology-ltd/'>https://www.linkedin.com/company/rf-technology-ltd/</a></li>
    <li>Phone No: +91-9905942199</li>
</ul>

<p>Best Regards,<br/>
Team RF Technologies</p>";

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.zoho.in", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("internship@rftechnologies.cloud", "Internship@9304"); // Use your actual email and password

                client.Send(message);
                client.Disconnect(true);
            }
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Student)]
        public IActionResult CheckIn(RegistrationForm obj)
        {
            _unitOfWork.RegistrationForm.UpdateStatus(obj.ID, SD.StatusCheckedIn);
            _unitOfWork.Save();
            TempData["success"] = "Registration Updated Successfully";
            return RedirectToAction(nameof(RegistrationUpdate), new { registrationId = obj.ID });
        }

        public IActionResult SubmitInternshipPage(int internshipId)
        {
            var registrationForm = _unitOfWork.RegistrationForm.Get(u => u.ID == internshipId);
            var internshipComplete = _unitOfWork.InternshipSubmit.Get(u => u.IntenshipId == internshipId) ?? new InternshipSubmit();

            RegistrationVM = new RegistrationVM
            {
                RegistrationList = registrationForm,
                InternshipCompelet = internshipComplete
            };

            return View(RegistrationVM);
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Student)]
        public IActionResult SubmitInternship(RegistrationVM obj, int internshipId)
        {
            if (obj.InternshipCompelet.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.InternshipCompelet.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\payment");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.InternshipCompelet.Image.CopyTo(fileStream);

                obj.InternshipCompelet.PaymentScreenShot = @"\images\payment\" + fileName;
            }

            obj.InternshipCompelet.IntenshipId = internshipId;

            // Fetch the existing InternshipComplete record if it exists
            var existingInternshipComplete = _unitOfWork.InternshipSubmit.Get(u => u.IntenshipId == internshipId);

            if (existingInternshipComplete != null)
            {
                // Update the existing record
                existingInternshipComplete.PaymentScreenShot = obj.InternshipCompelet.PaymentScreenShot;
                // Update other fields as necessary
                _unitOfWork.InternshipSubmit.Update(existingInternshipComplete);
            }
            else
            {
                // Add a new record
                _unitOfWork.InternshipSubmit.Add(obj.InternshipCompelet);
            }

            var registration = _unitOfWork.RegistrationForm.Get(u => u.ID == internshipId);
            registration.Status = SD.StatusInternshipSubmited;

            _unitOfWork.RegistrationForm.Update(registration);
            _unitOfWork.Save();

            TempData["success"] = "The internship has been submitted successfully.";
            return RedirectToAction("Index", "Student");
        }



        [HttpPost]
        [Authorize]
        public IActionResult GenerateOfferLetter(int id, string downloadType)
        {
            string basePath = _webHostEnvironment.WebRootPath;

            WordDocument document = new WordDocument();

            //Load the Template
            string dataPath = basePath + @"/exports/OfferLetter.docx";
            using FileStream fileStream = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            document.Open(fileStream, FormatType.Automatic);

            //Update Template
            RegistrationForm registrationFromDb = _unitOfWork.RegistrationForm.Get(u => u.ID == id, includeProperties: "User");

            TextSelection textSelection = document.Find("xx_student_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.Name;

            textSelection = document.Find("xx_domain_name", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.Domain;

            textSelection = document.Find("xx_domain_name_intern", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.Domain;

            textSelection = document.Find("xx_start_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.StartDate.ToString();

            textSelection = document.Find("xx_end_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.EndDate.ToString();

            textSelection = document.Find("xx_intern_intern_id", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "Intern ID: RFIN2024" + registrationFromDb.ID.ToString();

            textSelection = document.Find("xx_internship_approved_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = registrationFromDb.StartDate.ToString();


            using DocIORenderer renderer = new();
            MemoryStream stream = new();

            if (downloadType == "word")
            {

                document.Save(stream, FormatType.Docx);
                stream.Position = 0;

                return File(stream, "application/docx", "Offer Letter.docx");
            }
            else
            {
                PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                pdfDocument.Save(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", "Offer Letter.pdf");
            }


        }

        [HttpPost]
        [Authorize]
        public IActionResult GenerateCertificate(int id, string downloadType)
        {
            string basePath = _webHostEnvironment.WebRootPath;

            WordDocument document = new WordDocument();

            //Load the Template
            string dataPath = basePath + @"/exports/InternshipCompletionLetter.docx";
            using FileStream fileStream = new(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            document.Open(fileStream, FormatType.Automatic);

            //Update Template
            RegistrationForm registrationFromDb = _unitOfWork.RegistrationForm.Get(u => u.ID == id, includeProperties: "User");

            TextSelection textSelection;
            WTextRange textRange;

            // Replace all occurrences of "xx_intern_name"
            while ((textSelection = document.Find("xx_intern_name", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = registrationFromDb.Name;
            }

            // Replace all occurrences of "xx_domain_name"
            while ((textSelection = document.Find("xx_domain_name", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = registrationFromDb.Domain;
            }

            // Replace all occurrences of "xx_start_date"
            while ((textSelection = document.Find("xx_start_date", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = registrationFromDb.StartDate.ToString();
            }

            // Replace all occurrences of "xx_end_date"
            while ((textSelection = document.Find("xx_end_date", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = registrationFromDb.EndDate.ToString();
            }

            // Replace all occurrences of "xx_issue_date"
            while ((textSelection = document.Find("xx_issue_date", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = registrationFromDb.EndDate.AddDays(5).ToString();
            }

            // Replace all occurrences of "xx_intern_intern_id"
            while ((textSelection = document.Find("xx_intern_intern_id", false, true)) != null)
            {
                textRange = textSelection.GetAsOneRange();
                textRange.Text = "Intern ID: RFIN2024" + registrationFromDb.ID.ToString();
            }

            //textSelection = document.Find("xx_internship_approved_date", false, true);
            //textRange = textSelection.GetAsOneRange();
            //textRange.Text = registrationFromDb.StartDate.ToString();


            using DocIORenderer renderer = new();
            MemoryStream stream = new();

                PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                pdfDocument.Save(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", "Internship Completion Letter.pdf");


        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<RegistrationForm> objRegistration;

            if (User.IsInRole(SD.Role_Admin))
            {
                objRegistration = _unitOfWork.RegistrationForm.GetAll();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objRegistration = _unitOfWork.RegistrationForm.GetAll(u => u.UserId == userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                objRegistration = objRegistration.Where(u => u.Status.ToLower().Equals(status.ToLower()));
            }
            return Json(new { data = objRegistration });
        }

        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Delete(int? id)
        {
            var RegistrationToBeDeleted = _unitOfWork.RegistrationForm.Get(u => u.ID == id);
            if (RegistrationToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.RegistrationForm.Remove(RegistrationToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successful" });
        }

        #endregion


    }
}
