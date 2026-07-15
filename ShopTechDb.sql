use ShopTechDb
Create Database ShopTechDb
drop database ShopTechDb
use ShopTechDb

USE master;
ALTER DATABASE ShopTechDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

-- 1. Tạo bảng không có khóa ngoại trước
-- Bảng Categories
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);

-- Bảng AccountTypes
CREATE TABLE AccountTypes (
    AccountTypeID INT PRIMARY KEY IDENTITY(1,1),
    AccountTypeName NVARCHAR(50) NOT NULL
);

-- Bảng AccountStatuses
CREATE TABLE AccountStatuses (
    AccountStatusID INT PRIMARY KEY IDENTITY(1,1),
    AccountStatusName NVARCHAR(50) NOT NULL
);

-- 2. Tạo bảng có khóa ngoại phụ thuộc vào bảng không có khóa ngoại
-- Bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(500) NOT NULL, -- Cần mã hóa mật khẩu trong thực tế
    Email NVARCHAR(100) NOT NULL,
    UserType NVARCHAR(50) NOT NULL
);

-- Bảng Admins
CREATE TABLE Admins (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID)
);

-- 3. Tạo bảng có khóa ngoại phụ thuộc vào các bảng đã tạo
-- Bảng Customers
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    FullName NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20) NOT NULL, 
    CustomerType NVARCHAR(50) NOT NULL, -- Loại khách hàng
    CustomerStatus NVARCHAR(50) NOT NULL, -- Trạng thái khách hàng
    TotalSpent DECIMAL(18, 2) DEFAULT 0, -- Tổng số tiền đã mua
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE()  -- Ngày đăng ký
);

-- Bảng Products
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL, -- Số tiền bán ra
    Manufacturer NVARCHAR(100), -- Hãng sản xuất
    Status NVARCHAR(50), -- Trạng thái sản phẩm
    Supplier NVARCHAR(100), -- Nhà cung cấp
    PurchasePrice DECIMAL(18, 2), -- Số tiền nhập hàng
    Quantity INT, -- Số lượng
    EnteredBy NVARCHAR(100), -- Người nhập
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedDate DATETIME
);

-- 4. Tạo các bảng phụ thuộc vào các bảng đã tạo ở bước trên
-- Bảng Orders
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL, -- Giá trị đơn hàng
    Status NVARCHAR(50), -- Trạng thái đơn hàng
    ApprovedBy NVARCHAR(100), -- Người duyệt đơn
    ShippingAddress NVARCHAR(200), -- Địa chỉ đặt hàng
    Notes NVARCHAR(MAX) -- Ghi chú,
	ConfirmedByUserId int not null,
	PaymentMethod nvarchar(max),
	ShippingDate datetime(27),
);

-- Bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);

-- Bảng Reviews
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Rating INT NOT NULL,
    Comment NVARCHAR(MAX)
);

-- 5. Tạo bảng khác nếu cần
-- Bảng News
CREATE TABLE News (
    NewsID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
    Author NVARCHAR(100), -- Thêm thông tin về tác giả nếu cần
    Category NVARCHAR(100), -- Thêm thông tin về danh mục nếu cần
    ImageURL NVARCHAR(500) -- Đường dẫn hình ảnh cho bài tin tức
);
-- 1. Thêm dữ liệu mẫu vào bảng Users
INSERT INTO Users (Username, Password, Email, UserType)
VALUES 
('admin1', 'password123', 'admin1@example.com', 'Admin'),
('customer1', 'password456', 'customer1@example.com', 'Customer');

-- 2. Thêm dữ liệu mẫu vào bảng Admins
INSERT INTO Admins (UserID)
VALUES 
((SELECT UserID FROM Users WHERE Username = 'admin1'));

-- 3. Thêm dữ liệu mẫu vào bảng Customers
INSERT INTO Customers (UserID, FullName, Address, Phone, CustomerType, CustomerStatus, TotalSpent, RegistrationDate)
VALUES 
(
    (SELECT UserID FROM Users WHERE Username = 'customer1'), 
    'John Doe', 
    '123 Main St, City, Country', 
    '123-456-7890', 
    'Regular', 
    'Active', 
    500.00, 
    GETDATE()
);
