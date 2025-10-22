Report  
1. Purpose 
The Contract Monthly Claim System (CMCS) aims to make it easier to handle 
Independent Contractor (IC) lecturer claims in the institution. This system lets lecturers 
send in monthly claims for their work hours, add supporting files, and see how their 
claims are moving along. Programme Coordinators and Academic Managers look over, 
okay, or turn down claims, which helps keep things open and people accountable. 
2. Users / Roles 
Lecturer 
• Submits new monthly claims (Hours × Rate). 
• Uploads supporting files. 
• Views claim history and approval status. 
Coordinator 
• Reviews all pending lecturer claims. 
• Approves or rejects claims. 
• Adds review comments explaining decisions. 
Manager 
• Performs the same actions as the Coordinator but acts as a senior approver for 
f
 inal verification. 
3. Core Workflow 
• Lecturer creates a new claim and uploads required documentation. 
• Claim is submitted and marked as Pending. 
• Coordinator or Manager logs in to their dashboard. 
• Reviews pending claims → approves or rejects with comments. 
• System updates claim status and stores the review record. 
• Lecturer can view updated status and review comments. 
4. Platform Change and High-Level Design Decisions 
Previous Platform: WPF (.NET Desktop Application) 
Originally, CMCS was built in WPF to prototype the user interface. WPF allowed the 
creation of a rich GUI using XAML, gradients, blurred ellipses, and local navigation via a 
Frame. This helped establish the layout, usability, and data structure in a visually 
consistent way. 
New Platform: ASP.NET Core MVC (Web Application) 
The system has been migrated from WPF to MVC to provide multi-user web access, 
database-backed persistence, and role-based authentication. 
Reasons for choosing MVC: 
• Enables multi-user, role-based web access (Lecturer, Coordinator, Manager). 
• Provides structured separation of concerns (Model–View–Controller pattern). 
• Integrates directly with Entity Framework Core for database operations. 
• Allows easy deployment on internal or public web servers. 
• Supports scalability, maintainability, and integration with authentication and 
logging. 
5. MVC Implementation Overview 
Models 
Created strong model classes with DataAnnotations for validation, including: 
• Claim.cs — stores claim details (ClaimID, ClaimMonth, HoursWorked, 
HourlyRate, Status, etc.). 
• Review.cs — stores review actions, reviewer details, comments, and 
timestamps. 
• User.cs — stores user registration and authentication details, including roles. 
Controllers 
• LecturerController — handles claim submission, viewing, and file uploads. 
• CoordinatorController — manages reviewing, approving, and rejecting claims. 
• ManagerController — similar to Coordinator but for senior approval. 
• AuthController — handles registration and login/logout workflows.  
Views 
Each role has its own dashboard view implemented with Razor (.cshtml): 
• LecturerDashboard.cshtml — displays submitted claims and statuses. 
• CoordinatorDashboard.cshtml — shows pending claims with Approve/Reject 
buttons. 
• ManagerDashboard.cshtml — displays all claims for senior review. 
• Shared Layout — consistent header, footer, and navigation. 
Views are styled with CSS for consistency, using: 
• Centered containers with clean white/gray backgrounds. 
• Rounded buttons and hover effects for affordance. 
• Styled tables (<table class="table table-striped table-hover">) for readability. 
6. Added Functionalities 
1. File Upload 
Lecturers can upload supporting documents (PDF, JPG, PNG, DOCX). 
Files are validated by type and size before saving to a secure directory. 
2. Status Tracking 
Claims are automatically updated when approved or rejected, and review comments 
are visible to the lecturer in their dashboard. 
3. Review System 
Coordinators and Managers must include a comment when approving or rejecting a 
claim. 
All review details (Reviewer, Decision, Date, Comment) are stored in the Reviews table. 
4. Session-Based Role Handling 
Each user’s role (Lecturer, Coordinator, or Manager) and UserID are stored in session 
variables for authorization and tracking of reviewer actions. 
5. Validation 
DataAnnotations were applied for validation: 
• Hours must be within a reasonable range. 
• ClaimMonth required. 
• Uploaded file must be in an approved format and under 10MB. 
7. Unit Testing 
To ensure functionality and reliability, unit tests were developed using xUnit and Entity 
Framework Core InMemory database. This test ensures that: 
• Claim status is set to Rejected. 
• A corresponding review record is created with correct details. 
• The reviewer ID matches the logged-in coordinator (from session). 
Other unit tests include: 
• Approving claims (ApproveClaim_AddsReviewAndSetsStatus) 
• Claim creation and validation 
• User registration and login verification 
8. Security and Privacy 
• Passwords are hashed and never stored in plain text. 
• Role-based authorization restricts access to certain controllers and views. 
• Files are stored securely and are only accessible to claim owners and authorized 
reviewers. 
• Audit logs (reviews) record who made each decision and when. 
9. Constraints and Validations 
• File types: PDF, JPG, PNG, DOCX 
• Max file size: 10MB 
• Claim fields (Month, Hours, Rate) required 
• HourlyRate ≥ 0, HoursWorked ≥ 0 
• Each lecturer can only submit one claim per month. 
10. Future Enhancements 
• Add search, sort, and filter features on dashboards. 
• Include email notifications upon approval or rejection. 
• Add report generation (CSV/PDF export). 
• Implement a managerial analytics dashboard for insights (total claims per 
month, approval rates). 
• Deploy system to a secure web host with SSL.
