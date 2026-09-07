USE DVLD_Database;

--  ÃœÊ· «·œÊ·
CREATE TABLE Countries 
(
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName_En NVARCHAR(100) NOT NULL UNIQUE, -- «·«”„ »«·≈‰Ã·Ì“Ì (›—Ìœ „»Ì ﬂ——‘)
    CountryName_Ar NVARCHAR(100) NOT NULL UNIQUE  -- «·«”„ »«·⁄—»Ì (›—Ìœ „»Ì ﬂ——‘)
);

-- 1. ÃœÊ· «·√‘Œ«’ «·‰Ê«… «·√”«”Ì… ··”Ì” „
CREATE TABLE People 
(
    PersonID INT IDENTITY(1,1) PRIMARY KEY,
    NationalNo NVARCHAR(20) NOT NULL UNIQUE, -- «·—ﬁ„ «·Êÿ‰Ì ›—Ìœ Ê„»Ì ﬂ——‘
    FirstName NVARCHAR(50) NOT NULL,
    SecondName NVARCHAR(50) NOT NULL,
    ThirdName NVARCHAR(50) NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    Address NVARCHAR(max) NOT NULL,
    Phone1 NVARCHAR(20) NOT NULL,
	Phone2 nvarchar(20) null,
    Email NVARCHAR(100) NULL,
    NationalityCountryID INT NOT NULL, -- —ﬁ„ «·œÊ·… «··Ì »Ì„À· Ã‰”Ì… «·‘Œ’
    ImagePath NVARCHAR(250) NULL, -- „”«— «·’Ê—… ⁄·Ï «·ÃÂ«“
	Gender bit Not Null default 0, -- Male = 0, Female = 1
	CreationDate datetime default getdate(),

	CONSTRAINT FK_People_Countries FOREIGN KEY (NationalityCountryID) REFERENCES Countries(CountryID)
);

-- 2. ÃœÊ· «·„” Œœ„Ì‰ «·„ÊŸ›Ì‰
CREATE TABLE Users 
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    PersonID INT NOT NULL UNIQUE, -- ⁄·«ﬁ… One-to-One ·√‰ «·‘Œ’ ·ÌÂ Õ”«» Ê«Õœ
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1, -- 1 Ì⁄‰Ì ‘€«·° 0 Ì⁄‰Ì „Ã„œ
	CreatedDate datetime default getdate(),
    CONSTRAINT FK_Users_People FOREIGN KEY (PersonID) REFERENCES People(PersonID)
);

-- 3. ÃœÊ· √‰Ê«⁄ «·Œœ„« /«·ÿ·»«  «·À«» …
CREATE TABLE ApplicationTypes 
(
    ApplicationTypeID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationTypeName NVARCHAR(150) NOT NULL UNIQUE,
    Fees DECIMAL(18,2) NOT NULL DEFAULT 5.00 -- «·—”Ê„ «·«› —«÷Ì… 5 œÊ·«—
);

-- 4. ÃœÊ· √‰Ê«⁄ «·«Œ »«—«  «·À«» …
CREATE TABLE TestTypes 
(
    TestTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TestTypeName NVARCHAR(100) NOT NULL unique,
    TestTypeFees DECIMAL(18,2) NOT NULL
);

-- 5. ÃœÊ· ›∆«  «·—Œ’ «·”»⁄…
CREATE TABLE LicenseClasses
(
    LicenseClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(100) NOT NULL unique,
    ClassDescription NVARCHAR(500) NOT NULL,
    MinimumAllowedAge INT NOT NULL, -- «·”‰ «·ﬁ«‰Ê‰Ì
    ValidityLength INT NOT NULL, -- „œ… «·’·«ÕÌ… »«·”‰Ê« 
    ClassFees DECIMAL(18,2) NOT NULL
);

-- 6. ÃœÊ· «·ÿ·»«  «·⁄«„ (√Ì Õ—ﬂ… √Ê Œœ„… » »œ√ „‰ Â‰«)
CREATE TABLE Applications
(
    ApplicationID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationPersonID INT NOT NULL, -- „Ì‰ «·„Ê«ÿ‰ «··Ì „ﬁœ„ «·ÿ·»
    ApplicationDate DATETIME NOT NULL DEFAULT GETDATE(),
    ApplicationTypeID INT NOT NULL, -- ‰Ê⁄ «·Œœ„… ( ÃœÌœ° »œ· ›«ﬁœ...)
    ApplicationStatus INT NOT NULL DEFAULT 1, -- 1=ÃœÌœ° 2=„·€Ì° 3=„ﬂ „·
    PaidFees DECIMAL(18,2) NOT NULL,
    CreatedByUserID INT NOT NULL, -- «·„ÊŸ› «··Ì ⁄„· «·Õ—ﬂ…
    CONSTRAINT FK_Applications_People FOREIGN KEY (ApplicationPersonID) REFERENCES People(PersonID),
    CONSTRAINT FK_Applications_ApplicationTypes FOREIGN KEY (ApplicationTypeID) REFERENCES ApplicationTypes(ApplicationTypeID),
    CONSTRAINT FK_Applications_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
);

-- 7. ÃœÊ· ÿ·»«  «·—Œ’ «·„Õ·Ì… („—»Êÿ »«·ÿ·» «·⁄«„ ⁄‘«‰ ‰Õœœ «·›∆…)
CREATE TABLE LocalDrivingLicenseApplications 
(
    LocalDrivingLicenseApplicationID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationID INT NOT NULL, -- „—»Êÿ »«·ÿ·» «·⁄«„
    LicenseClassID INT NOT NULL, -- „—»Êÿ »›∆… «·—Œ’… („·«ﬂÌ° „Ê Ê”Ìﬂ·...)
    CONSTRAINT FK_LocalDrivingLicenseApplications_Applications FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID),
    CONSTRAINT FK_LocalDrivingLicenseApplications_LicenseClasses FOREIGN KEY (LicenseClassID) REFERENCES LicenseClasses(LicenseClassID)
);

-- 8. ÃœÊ· ÕÃ“ „Ê«⁄Ìœ «·«Œ »«—« 
CREATE TABLE TestAppointments 
(
    TestAppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    TestTypeID INT NOT NULL, -- ‰Ÿ—° ‰Ÿ—Ì° ⁄„·Ì
    LocalDrivingLicenseApplicationID INT NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    PaidFees DECIMAL(18,2) NOT NULL,
    CreatedByUserID INT NOT NULL,
    IsLocked BIT NOT NULL DEFAULT 0, -- 1 Ì⁄‰Ì « „ Õ‰ Œ·«’ Ê„Ì‰›⁄‘ Ì⁄œ· «·„Ì⁄«œ
    RetestApplicationID INT NULL, -- ·Ê ≈⁄«œ… ›Õ’° »‰—»ÿÂ »—ﬁ„ ÿ·» ≈⁄«œ… «·›Õ’ «·›—⁄Ì
    CONSTRAINT FK_TestAppointments_TestTypes FOREIGN KEY (TestTypeID) REFERENCES TestTypes(TestTypeID),
    CONSTRAINT FK_TestAppointments_LocalDrivingLicenseApplications FOREIGN KEY (LocalDrivingLicenseApplicationID) REFERENCES LocalDrivingLicenseApplications(LocalDrivingLicenseApplicationID),
    CONSTRAINT FK_TestAppointments_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID),
    CONSTRAINT FK_TestAppointments_Applications FOREIGN KEY (RetestApplicationID) REFERENCES Applications(ApplicationID)
);

-- 9. ÃœÊ· ‰ «∆Ã «·«Œ »«—«  «·›⁄·Ì
CREATE TABLE Tests
(
    TestID INT IDENTITY(1,1) PRIMARY KEY,
    TestAppointmentID INT NOT NULL, -- „—»Êÿ »„Ì⁄«œ «·≈„ Õ«‰
    TestResult BIT NOT NULL, -- 1 ‰«ÃÕ° 0 —«”»
    Notes NVARCHAR(500) NULL,
    Marks INT NULL, -- «·⁄·«„… „‰ 100 (··‰Ÿ—Ì)
    CreatedByUserID INT NOT NULL,
    CONSTRAINT FK_Tests_TestAppointments FOREIGN KEY (TestAppointmentID) REFERENCES TestAppointments(TestAppointmentID),
    CONSTRAINT FK_Tests_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
);

-- 10. ÃœÊ· «·”«∆ﬁÌ‰ («·„Ê«ÿ‰ »Ì ÕÊ· ·”«∆ﬁ ·„« Ì‰ÃÕ)
CREATE TABLE Drivers (
    DriverID INT IDENTITY(1,1) PRIMARY KEY,
    PersonID INT NOT NULL UNIQUE, -- «·‘Œ’ »Ì ÷«› Â‰« „—… Ê«Õœ… »” ›Ì ÕÌ« Â
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByUserID INT NOT NULL,
    CONSTRAINT FK_Drivers_People FOREIGN KEY (PersonID) REFERENCES People(PersonID),
    CONSTRAINT FK_Drivers_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
);

-- 11. ÃœÊ· «·—Œ’ «·„Õ·Ì… «·’«œ—… («·ﬂ«—  «·›⁄·Ì)
CREATE TABLE Licenses (
    LicenseID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationID INT NOT NULL, -- «·ÿ·» «·√’·Ì «··Ì »”»»Â ÿ·⁄  «·—Œ’…
    DriverID INT NOT NULL,
    LicenseClassID INT NOT NULL,
    IssueDate DATETIME NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    Notes NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1, -- 1 ‰‘ÿ…° 0 €Ì— ‰‘ÿ… (·Ê « Ãœœ  √Ê ÷«⁄ )
    IssueReason INT NOT NULL, -- 1=ÃœÌœ° 2= ÃœÌœ° 3=»œ·  «·›° 4=»œ· ›«ﬁœ
    CreatedByUserID INT NOT NULL,
    CONSTRAINT FK_Licenses_Applications FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID),
    CONSTRAINT FK_Licenses_Drivers FOREIGN KEY (DriverID) REFERENCES Drivers(DriverID),
    CONSTRAINT FK_Licenses_LicenseClasses FOREIGN KEY (LicenseClassID) REFERENCES LicenseClasses(LicenseClassID),
    CONSTRAINT FK_Licenses_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
);

-- 12. ÃœÊ· «·—Œ’ «·œÊ·Ì…
CREATE TABLE InternationalLicenses (
    InternationalLicenseID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationID INT NOT NULL,
    DriverID INT NOT NULL,
    IssuedUsingLocalLicenseID INT NOT NULL, -- „—»Êÿ… »—Œ’ Â «·„Õ·Ì… «·›∆… «· «· …
    IssueDate DATETIME NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserID INT NOT NULL,
    CONSTRAINT FK_InternationalLicenses_Applications FOREIGN KEY (ApplicationID) REFERENCES Applications(ApplicationID),
    CONSTRAINT FK_InternationalLicenses_Drivers FOREIGN KEY (DriverID) REFERENCES Drivers(DriverID),
    CONSTRAINT FK_InternationalLicenses_Licenses FOREIGN KEY (IssuedUsingLocalLicenseID) REFERENCES Licenses(LicenseID),
    CONSTRAINT FK_InternationalLicenses_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
);

-- 13. ÃœÊ· «·—Œ’ «·„ÕÃÊ“… Ê«·€—«„«  («·ÃœÊ· «·√ŒÌ— «··Ì ‰«ﬁ‘‰«Â)
CREATE TABLE DetainedLicenses (
    DetainID INT IDENTITY(1,1) PRIMARY KEY,
    LicenseID INT NOT NULL, -- «·—Œ’… «·„ÕÃÊ“…
    DetainDate DATETIME NOT NULL,
    FineAmount DECIMAL(18,2) NOT NULL, -- ﬁÌ„… «·€—«„…
    Reason NVARCHAR(500) NOT NULL, -- ”»» «·ÕÃ“
    CreatedByUserID INT NOT NULL, -- «·„ÊŸ› «··Ì ÕÃ“Â«
    IsReleased BIT NOT NULL DEFAULT 0, -- Â· « ›ﬂ ø 0=·√° 1=¬Â
    ReleaseDate DATETIME NULL, --  «—ÌŒ «·›ﬂ
    ReleasedByUserID INT NULL, -- «·„ÊŸ› «··Ì ›ﬂ «·ÕÃ“
    ReleaseApplicationID INT NULL, -- —ﬁ„ ÿ·» ›ﬂ «·ÕÃ“ «·„—»Êÿ »ÌÂ
    CONSTRAINT FK_DetainedLicenses_Licenses FOREIGN KEY (LicenseID) REFERENCES Licenses(LicenseID),
    CONSTRAINT FK_DetainedLicenses_Users FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID),
    CONSTRAINT FK_DetainedLicenses_Users1 FOREIGN KEY (ReleasedByUserID) REFERENCES Users(UserID),
    CONSTRAINT FK_DetainedLicenses_Applications FOREIGN KEY (ReleaseApplicationID) REFERENCES Applications(ApplicationID)
);
