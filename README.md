# Hotel Booking Management System

A role-based hotel reservation platform built with ASP.NET Core MVC, Entity Framework Core, SQL Server, and ASP.NET Identity.

The system streamlines hotel operations through centralized management of rooms, guests, and reservations while providing secure, role-specific experiences for both administrators and guests.

---

## Key Features

### Authentication & Authorization
- Secure user registration and login using ASP.NET Identity
- Role-based access control (Admin / Guest)
- Ownership-based authorization for reservation data

### Reservation Management
- Create, update, and cancel bookings
- Room availability tracking
- Automated reservation validation
- Prevention of overlapping bookings

### Guest Experience
- Personalized guest dashboard
- View and manage personal reservations
- Download booking receipts as PDF documents

### Administrative Management
- Manage rooms, guests, and bookings
- Full visibility across all reservations
- Centralized hotel operations dashboard

---

## Engineering Highlights

- Implemented ASP.NET Identity authentication and role management
- Linked authenticated users to guest records through persistent UserId relationships
- Enforced ownership validation to ensure guests can only access their own reservations
- Implemented reservation conflict detection to prevent double-booking scenarios
- Automated booking documentation through PDF receipt generation
- Structured the application using the ASP.NET Core MVC architecture for maintainability and scalability

---

## Technology Stack

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- Bootstrap
- Rotativa (PDF Generation)

---

## Domain Model

```text
Identity User
      ↓
    Guest
      ↓
   Booking
      ↓
     Room
```

This relationship structure enables secure ownership validation, role-specific workflows, and consistent data integrity throughout the reservation lifecycle.

---

## Security & Data Integrity

- Role-based authorization using `[Authorize(Roles = "...")]`
- Ownership-based access control using authenticated user claims
- Identity User ↔ Guest mapping through UserId relationships
- Server-side validation for reservation and account operations
- Booking visibility restricted to the authenticated guest owner

---

## Project Outcomes

- Multi-role hotel management platform
- Secure reservation ownership enforcement
- Automated PDF receipt generation
- Reservation conflict prevention
- Centralized management of rooms, guests, and bookings
- Improved maintainability through layered MVC architecture

---

## Getting Started

1. Clone the repository

```bash
git clone https://github.com/Menna06/Hotel-Booking-Management-System.git
```

2. Configure the SQL Server connection string in `appsettings.json`

3. Apply migrations

```bash
dotnet ef database update
```

4. Run the application

```bash
dotnet run
```

---

Developed as a full-stack software engineering project focused on secure access control, reservation workflow management, and practical business process modeling.
