# Tasky--Task-Management-Website

# Tasky – ASP.NET Core MVC

A simple task manager with user profiles.
This submission includes the: 
**Profile Update** feature with secure encrypted password change, profile photo upload, TempData-based success/error messages, and session-based authentication checks.
 **Task Management** feature:
- **Create**: Users can add new tasks with the following fields:
  - **Name** (required, short title).
  - **Description** (optional details).
  - **Start Date** and **Due Date** (must be valid dates, with due date after start).
  - **Status** (*To Do*, *Doing*, *Done*).

- **Edit**: Users can update an existing task’s details or change its status.

- **Delete**: Users can remove tasks permanently.



## Tech Stack
- ASP.NET Core MVC (.NET 7+)
- Entity Framework Core
- SQL Server / LocalDB
- Bootstrap 5
- BCrypt.Net (for password hashing)

## Getting Started

### 1) Prerequisites
- .NET SDK 7.0+
- SQL Server or LocalDB
- (Optional) Node/NPM if you plan to build any front-end assets

### 2) Clone
```bash
git clone 'https://github.com/ToqaKhashashneh/Tasky--Task-Management-Website.git'
```

### 3) App Settings
make sure to change the server to your serve:
`

"ConnectionStrings": {
  "MyConnectionString": "Server=DESKTOP-5RUPPFV; Database= Tasky ;Trusted_Connection=True;TrustServerCertificate=True;"
}



```

### 4) Register Services
Make sure these exist in `Program.cs`:
```csharp
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YourConnectionString")));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); 

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // enable sessions
app.MapDefaultControllerRoute();
app.Run();
```

### 5) Database

Sql Execute this Query

Create Database TaskTracker;

Create Table Users (
	ID int primary key identity(1,1),
	UserName Nvarchar (255) Not Null,
	Email Nvarchar (255) Unique,
	Password Nvarchar (Max) Not Null,
	ProfilePicture Nvarchar (255), 
);


Create Table Tasks (
    ID int primary key identity(1,1),
    Name nvarchar(255),
    Status nvarchar(255),
   	UserID int Not Null,
    CreatedDate date,
    Description Nvarchar (Max),
    DueDate Date,
CONSTRAINT FK_Tasks_Users FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
);

Create Table Contact(
ID int Primary Key Identity(1,1),
FullName Nvarchar(255),
Email Nvarchar(255),
PhoneNumber Nvarchar(255),
Message Nvarchar (Max)
)

### 6) Run
```bash
dotnet run
```
Browse to `https://localhost:5001` (or the port shown in the console).

## Profile Update Endpoint
The profile update action supports:
- Updating `UserName`
- Optional profile photo upload (saved to `wwwroot/uploads`)
- Optional password change with validation (old password check, complexity rules, and re-hash via BCrypt)

## TempData Alerts
- `TempData["SuccessMessage"]` on success
- `TempData["ErrorMessage"]` for validation failures
(Using SweetAlert).


## How to Run the Demo Quickly
1. Register/login a test user.
2. Go to **Profile**, change the username, optionally upload an image.
3. Change password by entering **Old Password** and **New Password** (>= 8 chars, includes uppercase, lowercase, and a digit).
4. Observe success/error messages.

## Notes
- Passwords are hashed using BCrypt.Net-Next.
- Session is used to store `UserId` upon login.
- If you see `InvalidOperationException: No service for type 'IHttpContextAccessor'`, ensure `AddHttpContextAccessor()` is registered.
