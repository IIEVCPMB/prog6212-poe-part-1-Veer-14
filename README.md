Report 
Purpose: a simple admin system where Independent Contractor (IC) lecturers submit monthly claims (hours × rate), upload supporting documents, and Programme Coordinators / Academic Managers approve or reject claims.

Users / Roles

•	Lecturer — register, submit claims, upload files, view claim status/history.
•	Coordinator — approve/reject claims, add review comments.
•	Manager — same as Coordinator (senior approver).

Core flows
•	Register → Login → Lecturer Dashboard → New Claim → Upload docs → Submit.
•	Coordinator/Manager Login → Dashboard → Review pending claims → Approve/Reject (add comment) → status updates.
•	Lecturer views status and comments in dashboard.
2. High-level design decisions (why these choices)
Selected platform: WPF (.NET)

•	Rich desktop UI controls (DataGrid, styles, templates, animations).
•	Simple to prototype GUI and interactions locally without web server, routing, or controllers.
•	Enables advanced visuals (blurred ellipses, gradients) with minimal effort.

Navigation approach
•	Single MainWindow with Frame (MainFrame) hosting pages so navigation is simple and the Back button (page-level) can call NavigationService.GoBack() or MainFrame.Navigate(). This aligns with your prototype pages and maintains a consistent header/background between pages.


Data binding and tables
•	For prototype, use in-memory lists bound to DataGrid. For production, map to relational DB. Use normalized schema for most of the data, but for prototyping we flattened claim + review columns into the Claims table (Option 2) for simplicity.

UI design decisions

•	Minimalistic, modern appearance with large, futuristic-readable fonts for headings.
•	Consistent affordances: identical button style across pages (rounded corners, black border, transparent background, hover toggles to solid black with white text). Helps maintain consistency and reduces cognitive load.
•	Top-right utility actions: Register / Login buttons top row (left/right positioning as you requested) for discoverability.
•	Central action area: key actions (Lecturer / Coordinator / Manager dashboards) middle position as large buttons — salient primary affordances.
•	DataGrid for claims: brief overview with wrapping headers, readable columns, hover highlight and selectable rows.

3. GUI look & feel 
MainWindow
•	Background: soft multi-stop linear gradient and several large blurred ellipses (blue, cyan, teal/purple) for depth & "premium" look. Reasoning: modern, friendly and removes flatness. Ellipses are for appearance, not interactive.
•	Hosts MainFrame for pages.

HomePage
•	Top row: Register (left) and Login (right) buttons using TopButtonStyle. Reasoning: adhere to usual web placement (login right, register left in your screenshot).
•	Center: Title "CMCS Dashboard", three large buttons for each role dashboard — clean call-to-actions.



Register / Login
•	Form-focused, uniform button style.
•	TextBoxes with transparent background, bold black border (same look as Login page).
•	Back button top-aligned on pages to go back to HomePage (same appearance as other back buttons).

Lecturer Dashboard
•	Back button, Title, + Create New Claim button.
•	DataGrid showing claims: columns ClaimID, ClaimMonth, HoursWorked, HourlyRate, Amount, Status, ReviewedBy, ReviewDate, ReviewComments.
•	Row hover effect: dark → highlight for selection. DataGrid uses auto-sizing and weighted columns such that text fits.

NewClaim Page 
•	Fields:
o	Claim month/year (Automatically added)
o	Hours (int)
o	Hourly Rate (decimal)
o	Calculated Total (Hours × Rate)
o	Upload supporting file
o	Submit
•	Rationale: detailed recording of claim line items; facilitates future itemized viewing.

4. Affordance & consistency (usability)
•	Affordance: Buttons look clickable (round border + hover color switch). Large fonts and high contrast on action items.
•	Visual consistency: A single common button template (FuturisticButton / DashboardButtonStyle) across pages. All back/navigation controls have same style and position.
•	Feedback: Hover effects, enabled/disabled states, success/failure MessageBox for confirmation. DataGrid row hover highlights current selection.
•	Text wrapping & header height fixes truncation and labels are readable.
•	Icons for immediate recognition of login/register.

5. Colour scheme & typography
•	Main neutral colours: black / white for controls and content (simplest design).
•	Accent background: soft gradient + softly blurred colour orbs (blue / cyan / teal / purple) in order to create depth. Reasoning: makes UI appear modern without taking away from content readability.
•	Accent action: previously neon green (#39FF14) on hover highlights for some of the prototype versions — high-contrast for selection. For our modern black/white version we keep hover to black and text to white for readability.
•	Typography: massive, bold headings (36–40px). Body type 14–16px for readability. For "futuristic" select a display font (e.g., Orbitron/Phantom Sans) for headings only.

6. Menu & structure 
•	Top utility row: Register / Login — consistent with most web apps where auth is top-right.
•	Middle action area: instant access to role dashboards (Lecturer, Coordinator, Manager) for quick navigation — handy for demo/prototype when real login workflow may be bypassed.
•	Inside dashboards: actions close to content (Create New Claim at top centre), DataGrid underneath for history summary; Review actions (Approve/Reject) within coordinator/manager dashboards close to DataGrid.
7. Assumptions 
User assumptions
•	Users will sign up with name, email and password. Role (Lecturer/Coordinator/Manager) fixed at sign up or as assigned by admin.
•	Lecturers only submit claims for months they did work.
•	Hourly rates are provided per lecturer (can be editable on claim or retrieved from HR table).
•	Only Coordinators & Managers can change claim Status.

Technical assumptions
•	App runs on Windows with .NET/WPF supported. For persistence, SQL Server or SQLite available.
•	Attachments stored on file system; path saved by DB. Alternatively, cache blobs in DB if necessary.



Business rules (claims)
•	Single claim per month per lecturer (or multiple claim items within a claim permitted).
•	Claim amount = total(hours × rate) and must not exceed configured max.
•	Coordinator needs to add comments in rejection.

8. Constraints & validations
File upload
•	Permitted formats: PDF, JPG, PNG, DOCX. (Why: typical documents and photos)
•	Max file size: 10 MB per file optimal for university attachments (storage vs. quality trade-off). You can reduce to (5MB) if storage/network is constrained.
•	Max attachments per claim: e.g., 5 for abuse prevention.
•	Storage: save to secure folder per claim (e.g., /data/claims/{ClaimID}/{filename}), store relative path in Attachments table.

Data
•	HoursWorked >= 0, HourlyRate >= 0.
•	Email unique, verified format.
•	Passwords hashed stored, never unhashed.

UI
•	DataGrid columns wrap text and provide scrollbars if content overflows column limits.
•	Column widths responsive (* weights) across window sizes.
•	Accessibility: keyboard focus, tooltips, AutomationProperties for screen readers (optional enhancement).

9. Security & privacy notes
•	Passwords: should be hashed (never unhashed); store PasswordHash.
•	Authorization: role-based access to pages (Lecturer cannot approve, Coordinator cannot edit lecturer personal details).
•	File privacy: attachments visible to claim owner and reviewers only.
•	Logging/auditing: record who reviewed (ReviewedBy) and when (ReviewDate).


10. Future enhancements 
•	Normalize DB: separate ClaimReview table for audit/history so re-open workflows and multiple reviews allowed.
•	Add search/filter/sort to DataGrid (month, status, module).
•	Add yes/no confirmation prompts to approve/reject and show reviewer comments through popups.
•	Add export to CSV/PDF for reporting.
•	Add role-management admin view.
•	Use server-side API and switch to web app (MVC/Razor or Blazor) if you need multi-user remote access.

11. Mapping deliverables
•	MainWindow.xaml — gradient background + misty orbs, Frame MainFrame.
•	HomePage.xaml — top-left Register / top-right Login, middle buttons.
•	Register.xaml — register form (FirstName, Surname, Email, Password).
•	Login.xaml — login form with Back button.
•	LecturerDashboard.xaml — Back, Create New Claim, DataGrid (Claim list) with wrapped headers and review columns.
•	CoordinatorDashboard.xaml, ManagerDashboard.xaml — same theme (yet to be reviewed, approve/reject operations).
•	NewClaim.xaml — form to type out claim (fields + file upload) — yet to be hooked into database.
