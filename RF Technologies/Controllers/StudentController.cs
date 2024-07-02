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
                TempData["success"] = "The Registration has been Updated done successfully.";
                return RedirectToAction("Index", "Home");
            }
            return View(obj);
        }

        public IActionResult Delete(int registrationId)
        {
            RegistrationForm? obj = _unitOfWork.RegistrationForm.Get(u => u.ID == registrationId);

            //Villa? obj = _db.Villas.Find(villaId);
            //var VillaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(RegistrationForm obj)
        {
            RegistrationForm? objFromDb = _unitOfWork.RegistrationForm.Get(u => u.ID == obj.ID);
            if (objFromDb is not null)
            {
                _unitOfWork.RegistrationForm.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["error"] = "The Registration has been deleted successfully.";
                return RedirectToAction("Index", "Student");
            }
            TempData["error"] = "The Registration has could not be deleted.";
            return View();
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Approve(RegistrationForm obj)
        {
            _unitOfWork.RegistrationForm.UpdateStatus(obj.ID, SD.StatusApproved);
            _unitOfWork.Save();
            TempData["success"] = "Registration Updated Successfully";
            return RedirectToAction(nameof(RegistrationUpdate), new { registrationId = obj.ID });
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
            TempData["success"] = "Registration Updated Successfully";
            //return View("Index", "Student");
            return RedirectToAction(nameof(RegistrationUpdate), new { registrationId = obj.ID });
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

        #endregion


    }
}
