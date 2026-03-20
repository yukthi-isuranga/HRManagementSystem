select * from EmployeeDepartments

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(100),
    Password NVARCHAR(200),
    Role NVARCHAR(50)
);

CREATE TABLE Departments (
    Id INT PRIMARY KEY IDENTITY,
    DepartmentCode NVARCHAR(50) UNIQUE,
    DepartmentName NVARCHAR(100)
);


CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Email NVARCHAR(150),
    DateOfBirth DATE,
    Salary DECIMAL(18,2)
);

CREATE TABLE EmployeeDepartments (
    EmployeeId INT,
    DepartmentId INT,
    PRIMARY KEY(EmployeeId, DepartmentId)
);

CREATE INDEX IX_DepartmentCode ON Departments(DepartmentCode);

CREATE UNIQUE INDEX IX_Departments_DepartmentCode
ON Departments (DepartmentCode);


<img width="328" height="516" alt="Screenshot 2026-03-20 230411" src="https://github.com/user-attachments/assets/b91510af-bcd6-4487-b67e-71a41027dad4" />
<img width="657" height="659" alt="Screenshot 2026-03-20 230316" src="https://github.com/user-attachments/assets/00a9e811-ea10-4fca-ba63-5aa194ec229f" />
<img width="845" height="1025" alt="Screenshot 2026-03-20 230056" src="https://github.com/user-attachments/assets/a9315a44-98ee-4549-b753-cbc1104a4398" />
<img width="1907" height="936" alt="Screenshot 2026-03-20 225910" src="https://github.com/user-attachments/assets/8bee00d9-3c69-421f-b659-9a62ddcc135b" />
<img width="1885" height="983" alt="Screenshot 2026-03-20 230452" src="https://github.com/user-attachments/assets/d704d9ac-20e0-4191-b445-04a1d67cdcee" />
