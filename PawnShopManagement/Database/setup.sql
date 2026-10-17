CREATE TABLE "AppUsers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AppUsers" PRIMARY KEY AUTOINCREMENT,
    "UserName" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "FailedLoginCount" INTEGER NOT NULL,
    "LockoutUntil" TEXT NULL,
    "Role" TEXT NOT NULL,
    CONSTRAINT "CK_AppUsers_Role" CHECK (Role IN ('Admin','Staff'))
);


CREATE TABLE "Customers" (
    "CustomerID" INTEGER NOT NULL CONSTRAINT "PK_Customers" PRIMARY KEY AUTOINCREMENT,
    "FullName" TEXT NOT NULL,
    "IdentityCard" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "Address" TEXT NOT NULL
);


CREATE TABLE "PawnContracts" (
    "ContractID" INTEGER NOT NULL CONSTRAINT "PK_PawnContracts" PRIMARY KEY AUTOINCREMENT,
    "CustomerID" INTEGER NOT NULL,
    "AssetName" TEXT NOT NULL,
    "AssetDetails" TEXT NULL,
    "LoanAmount" TEXT NOT NULL,
    "InterestRate" REAL NOT NULL,
    "StartDate" TEXT NOT NULL,
    "TermDays" INTEGER NULL,
    "Status" TEXT NOT NULL,
    CONSTRAINT "CK_PawnContracts_Loan" CHECK (CAST(LoanAmount AS REAL) BETWEEN 500000 AND 1000000000),
    CONSTRAINT "CK_PawnContracts_Rate" CHECK (InterestRate BETWEEN 0 AND 100),
    CONSTRAINT "CK_PawnContracts_Status" CHECK (Status IN ('DangCam','DaChuoc','DaThanhLy')),
    CONSTRAINT "CK_PawnContracts_TermDays" CHECK (TermDays IS NULL OR TermDays BETWEEN 1 AND 3650),
    CONSTRAINT "FK_PawnContracts_Customers_CustomerID" FOREIGN KEY ("CustomerID") REFERENCES "Customers" ("CustomerID") ON DELETE RESTRICT
);


CREATE UNIQUE INDEX "IX_AppUsers_UserName" ON "AppUsers" ("UserName");


CREATE UNIQUE INDEX "IX_Customers_IdentityCard" ON "Customers" ("IdentityCard");


CREATE INDEX "IX_PawnContracts_CustomerID" ON "PawnContracts" ("CustomerID");


