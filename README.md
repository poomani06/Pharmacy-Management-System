# Pharmacy Inventory Management System

## Project Overview

The **Pharmacy Inventory Management System** is a C# console application used to manage medicines, suppliers, sales, and payments in a pharmacy.

The application helps users maintain medicine stock, track expiry dates, manage supplier information, record sales, and keep track of customer payments. It uses Microsoft SQL Server to store data.

## Technologies Used

- **Programming Language:** C#
- **Framework:** .NET
- **Database:** Microsoft SQL Server Express
- **Database Connectivity:** Microsoft.Data.SqlClient
- **Architecture:** Repository Pattern and Service Layer
- **Development Environment:** Visual Studio

## Features

### 1. Medicine Management
- Add new medicines.
- View all medicines.
- Search medicines by name or medicine code.
- View medicine details by ID.
- Update medicine information.
- Delete medicines by marking them inactive.
- View low-stock medicines.
- View expired medicines.
- View medicines expiring within a specified number of days.

### 2. Supplier Management
- Add new suppliers.
- View all suppliers.
- Search suppliers.
- View supplier details by ID.
- Update supplier information.
- Delete suppliers by marking them inactive.

### 3. Sales Management
- Create a sale for a medicine.
- View all sales.
- Search sales by medicine name.
- View sale details by ID.
- View sales within a date range.
- View total sales amount.
- View today's sales amount.
- View total quantity sold.
- View the number of completed sales.
- Cancel a sale and restore the medicine stock.

### 4. Payment Management
- Add a payment for a completed sale.
- View all payments.
- View payment details by ID.
- Search payments by payment method.
- View payments associated with a sale.
- Calculate the total paid amount.
- Update payment status.
- Mark a payment as failed.
- Prevent payments from exceeding the remaining sale amount.

## Project Structure

```text
PharmacyInventoryManagement/
│
├── Models/
│   ├── Medicine.cs
│   ├── Supplier.cs
│   ├── Sale.cs
│   └── Payment.cs
│
├── Repositories/
│   ├── MedicineRepository.cs
│   ├── SupplierRepository.cs
│   ├── SaleRepository.cs
│   └── PaymentRepository.cs
│
├── Services/
│   ├── MedicineService.cs
│   ├── SupplierService.cs
│   ├── SaleService.cs
│   └── PaymentService.cs
│
├── Helpers/
│   └── DatabaseHelper.cs
│
├── Program.cs
├── PharmacyInventoryManagement.csproj
├── README.md
└── docs.md
```

## Architecture

The application uses a simple layered structure.

- **Models:** Define the data properties for medicines, suppliers, sales, and payments.
- **Repositories:** Perform database operations such as adding, retrieving, updating, and deleting records.
- **Services:** Apply business rules and validate user input before performing operations.
- **DatabaseHelper:** Creates SQL Server database connections.
- **Program.cs:** Displays the console menu and calls the appropriate service methods.

## Database Configuration

The application uses the following SQL Server connection string in `Helpers/DatabaseHelper.cs`:

```csharp
@"Server=.\SQLEXPRESS;Database=PharmacyInventoryDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

Make sure that:

1. SQL Server Express is installed and running.
2. The database `PharmacyInventoryDB` exists.
3. The required tables and columns are created in the database.
4. The connection string matches your SQL Server instance.
5. Your Windows account has permission to access the database.

## Prerequisites

Install the following before running the application:

- .NET SDK compatible with the project's target framework.
- SQL Server Express.
- SQL Server Management Studio (SSMS).
- Visual Studio or Visual Studio Code.

## How to Run the Project

### Step 1: Open the project

Open the `PharmacyInventoryManagement` project folder in Visual Studio or open a terminal in the project directory.

### Step 2: Configure the database

Create the `PharmacyInventoryDB` database and the required tables. Check `Helpers/DatabaseHelper.cs` and update the connection string if necessary.

### Step 3: Restore dependencies

Run:

```powershell
dotnet restore
```

### Step 4: Build the project

Run:

```powershell
dotnet build
```

Resolve any build errors before proceeding.

### Step 5: Run the application

Run:

```powershell
dotnet run
```

### Step 6: Use the menu

Follow the displayed menu and enter the number corresponding to the operation you want to perform. Enter the requested details when prompted.

## Important Notes

- Medicine stock is reduced when a sale is successfully created.
- A sale should only be created when sufficient stock is available and the medicine is valid for sale.
- Cancelling a sale restores the associated medicine stock.
- Payments are associated with sales using `SaleId`.
- Payment amounts must not exceed the remaining amount due for a completed sale.
- The database schema must match the SQL queries in the repository classes.
- This project is a console application and does not include a web frontend.

## Future Enhancements

- Add a graphical user interface or web interface.
- Generate sales and inventory reports.
- Add user login and role-based access.
- Export reports to Excel or PDF.
- Add automated database backup functionality.
- Add unit tests for services and repositories.

## Author

Developed as a C# and SQL Server pharmacy inventory management project.
