using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Utility;


namespace RF_Technologies.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            if (ModelState.IsValid)
            {
                obj.Status = SD.StatusPending;
                _unitOfWork.RegistrationForm.Add(obj);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Home");
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
                if(obj.Status == SD.StatusApproved)
                {
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
                    obj.StartDate = currentDate.AddDays(2);

                    DateOnly starDate = DateOnly.FromDateTime(DateTime.Now);
                    obj.EndDate = starDate.AddMonths(1);
                }
                _unitOfWork.RegistrationForm.Update(obj);
                _unitOfWork.Save();
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


    }
}
