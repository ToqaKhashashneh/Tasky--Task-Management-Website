using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Task_Tracking.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Task_Tracking.Controllers
{
    public class UserController : Controller
    {

        private readonly MyDbContext _context;

        private readonly IWebHostEnvironment _env;

        public UserController(MyDbContext context, IWebHostEnvironment env)
        {

            _context = context;
            _env = env;
        }

        public IActionResult SignUp()
        {
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitSignUp(User user, string confirmPassword)
        {
            if (user.Password != confirmPassword)
            {
                ModelState.AddModelError("PasswordMismatch", "Passwords do not match");
                return View("SignUp", user);
            }

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View("SignUp", user);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (ModelState.IsValid)
            {
                user.Password = hashedPassword;
                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] = "Welcome to Tasky! Your account was created successfully.";
                return RedirectToAction("SignIn");
            }

            return View("SignUp", user);
        }



        public IActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitSignIn(string Email , string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                TempData["Error"] = "Invalid email or password.";
                return View("SignIn");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Tasks", "User");

        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("SignIn");
        }

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("SignIn");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("SignIn");
            }
            return View(user);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(User updatedUser, IFormFile? ProfilePhoto, string? OldPassword, string? NewPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = updatedUser.UserName;

            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{ProfilePhoto.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePhoto.CopyTo(stream);
                }

                user.ProfilePicture = "/uploads/" + uniqueFileName;
            }

            if (!string.IsNullOrWhiteSpace(OldPassword))
            {
                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    TempData["ErrorMessage"] = "Please enter a new password.";
                    return RedirectToAction("Profile");
                }

                if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.Password))
                {
                    TempData["ErrorMessage"] = "Old password is incorrect.";
                    return RedirectToAction("Profile");
                }

                if (OldPassword == NewPassword)
                {
                    TempData["ErrorMessage"] = "New password cannot be the same as the old password.";
                    return RedirectToAction("Profile");
                }

                if (NewPassword.Length < 8 || !NewPassword.Any(char.IsUpper) || !NewPassword.Any(char.IsLower) || !NewPassword.Any(char.IsDigit))
                {
                    TempData["ErrorMessage"] = "New password must be at least 8 characters long and include uppercase, lowercase, and a number.";
                    return RedirectToAction("Profile");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            _context.Users.Update(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile"); 
        }



        public IActionResult Tasks(string query)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("SignIn");
            }

            var tasks = _context.Tasks.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                tasks = tasks.Where(t =>
                    (t.Name != null && t.Name.ToLower().Contains(query)) ||
                    (t.Description != null && t.Description.ToLower().Contains(query))
                );
            }

            return View(tasks.ToList());
        }



        public IActionResult AddTask()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("SignIn");
            }

            var task = new Task_Tracking.Models.Task(); 
            return View(task);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitTask(Task_Tracking.Models.Task task)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn");

            
                task.UserId = userId.Value;
                task.Status = "To Do";
                task.Name = task.Name?.Trim();
                task.Description = task.Description;
                task.DueDate = task.DueDate;
                task.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

                _context.Tasks.Add(task);
                _context.SaveChanges();

                TempData["Success"] = "Task added successfully!";
                return RedirectToAction("Tasks");
            
        }



        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var task = await _context.Tasks.FindAsync(id);

            task.Status = status;

            _context.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Tasks));
        }


        public IActionResult EditTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitEditTask(Task_Tracking.Models.Task task, int id)
        {
            var EditedTask = await _context.Tasks.FindAsync(id);
            if (EditedTask == null)
            {
                return NotFound();
            }

            EditedTask.Name = task.Name;
            EditedTask.Description = task.Description;
            EditedTask.DueDate = task.DueDate;
            EditedTask.Status = task.Status;
            EditedTask.CreatedDate = task.CreatedDate;

            _context.Update(EditedTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasks));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tasks));
        }

    }


}
