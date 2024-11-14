using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using RF_Technologies.Data_Access.Repository.IRepository;
using RF_Technologies.Model;
using RF_Technologies.Model.VM;
using RF_Technologies.Utility;
using MailKit.Net.Smtp;


namespace RF_Technologies.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl
            };

            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            RegisterVM registerVM = new()
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                RedirectUrl = returnUrl
            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use.");
                    registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Name
                    });
                    return View(registerVM);
                }

                ApplicationUser user = new()
                {
                    Name = registerVM.Name,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = false,
                    UserName = registerVM.Email,
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Student);
                    }

                    // Generate email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var confirmationLink = $"https://rftechnologies.cloud/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
                    var confirmationLink = $"https://rftechnologies.azurewebsites.net/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

                    // Send email
                    await SendEmailConfirmationAsync(user.Email, confirmationLink);

                    // Return to confirmation required view
                    return View("EmailConfirmationRequired");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            registerVM.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });

            return View(registerVM);
        }

        private async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RF Technologies", "internship@rftechnologies.cloud"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Confirm your email";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
    <html>
    <body>
        <h2>Welcome to RF Technologies!</h2>
        <p>Thank you for signing up with RF Technologies. We're thrilled to have you on board!</p>
        <p>RF Technologies specializes in innovative solutions to optimize and enhance RF (Radio Frequency) applications across various industries. We are committed to providing our clients with cutting-edge technology, superior customer support, and tailored solutions to meet your unique needs.</p>

        <h3>Account Confirmation</h3>
        <p>To activate your account and complete your registration, please confirm your email address by clicking on the link below:</p>
        <p><a href='{confirmationLink}'>Confirm My Account</a></p>
        <p>This link will take you directly to your account setup page, where you can finalize your profile and start exploring our services.</p>

        <h3>Contact Us</h3>
        <p>If you have any questions, feel free to reach out to us:</p>
        <ul>
            <li>Email: rftechnologiesltd@gmail.com</li>
            <li>Phone: +91-9905942199</li>
            <li>Address: Bari Khagual, Patna 801105</li>
        </ul>

        <p>We look forward to helping you harness the power of RF technology!</p>

        <p>Best regards,</p>
        <p><strong>The RF Technologies Team</strong></p>
    </body>
    </html>
"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Connect to the SMTP server
                await client.ConnectAsync("smtp.zoho.in", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync("internship@rftechnologies.cloud", "Internship@9304");

                // Send the message
                await client.SendAsync(message);

                // Disconnect and dispose of the client
                await client.DisconnectAsync(true);
            }
        }

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("EmailConfirmed");
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
                    {
                        model.LockoutMessage = "Your account is locked. Please contact the administrator.";
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RemeberMe, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.RedirectUrl) && Url.IsLocalUrl(model.RedirectUrl))
                        {
                            return Redirect(model.RedirectUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        model.LockoutMessage = "Your account is locked. Please contact the administrator.";
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token, email = user.Email },
                    protocol: "https",
                    host: "rftechnologies.cloud");

                await SendResetPasswordEmailAsync(user.Email, resetLink);

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private async Task SendResetPasswordEmailAsync(string email, string resetLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RF Technologies", "internship@rftechnologies.cloud"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Reset Password";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"Please reset your password by clicking this link: <a href='{resetLink}'>link</a>"
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.zoho.in", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("internship@rftechnologies.cloud", "Internship@9304");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;
            var userDetail = _unitOfWork.User.Get(u => u.Id == userId);
            return View(userDetail);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(ApplicationUser userModel)
        {
            var user = _unitOfWork.User.Get(u => u.Id == userModel.Id);
            if (user == null)
            {
                return NotFound();
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string previousImagePath = user.ProfilePicture;

            if (userModel.Image != null)
            {
                // Generate a unique filename for the new image
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(userModel.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\userPhoto");


                // Create the directory if it doesn't exist
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                // Delete the previous image if it exists
                if (!string.IsNullOrEmpty(previousImagePath) && !previousImagePath.StartsWith("https://"))
                {
                    string previousImageFullPath = Path.Combine(wwwRootPath, previousImagePath.Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(previousImageFullPath))
                    {
                        System.IO.File.Delete(previousImageFullPath);
                    }
                }

                // Save the new image
                string newImageFullPath = Path.Combine(imagePath, fileName);
                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);

                userModel.Image.CopyTo(fileStream);

                user.ProfilePicture = @"\images\userPhoto\" + fileName;
            }
            else
            {
                user.ProfilePicture = "https://placehold.co/600x400";
            }

            user.Name = userModel.Name;
            user.Bio = userModel.Bio;
            user.PhoneNumber = userModel.PhoneNumber;
            // Update other fields if needed
            _unitOfWork.User.Update(user);
            _unitOfWork.Save();
            TempData["success"] = "The Profile has been Updated successfully.";

            return RedirectToAction("UserProfile", new { userId = user.Id });
        }


    }
}
