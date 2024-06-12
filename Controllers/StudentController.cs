using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;

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


    }
}
