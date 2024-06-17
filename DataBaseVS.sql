CREATE DATABASE tttn_viettel_store

CREATE TABLE Admin
(
	Id			int			IDENTITY(1,1)		primary key,
	Name		nvarchar(500),
	Photo		nvarchar(500),
	Email		nvarchar(500),
	Password	nvarchar(500),
)

CREATE TABLE Customers
(
	Id			int			IDENTITY(1,1)		primary key,
	Name		nvarchar(500),
	Photo		nvarchar(500),
	Email		nvarchar(500),
	Address		nvarchar(500),
	Phone		nvarchar(500),
	Password	nvarchar(500),
)

CREATE TABLE Adv
(
	Id			int			IDENTITY(1,1)		primary key,
	Name		nvarchar(500),
	Photo		nvarchar(500),
	Position	int
)

CREATE TABLE News
(
	Id			int			IDENTITY(1,1)		primary key,
	Name		nvarchar(500),
	Photo		nvarchar(500),
	Content		nvarchar(500),
	Timestamp	date,
	Hot			int
)

CREATE TABLE Slides
(
	Id			int			IDENTITY(1,1)		primary key,
	Name		nvarchar(500),
	Photo		nvarchar(500),
	Title		nvarchar(500),
	SubTitle	nvarchar(500),
	Info		nvarchar(500),
	Link		nvarchar(500),
)

CREATE TABLE Categories
(
	Id					int			IDENTITY(1,1)		primary key,
	ParentId			int,
	Name				nvarchar(500),
	DisplayHomePage		int
)

CREATE TABLE Products
(
	Id					int			IDENTITY(1,1)		primary key,
	Name				nvarchar(max),
	Description			nvarchar(max),	
	Content				nvarchar(max),
	Photo				nvarchar(500),
	Price				float,
	Discount			float,
	Hot					int	
)	

CREATE TABLE Categories_Product
(
	Id					int			IDENTITY(1,1)		primary key,
	CategoryId			int,
	ProductId			int
)	

CREATE TABLE Rating
(
	Id					int			IDENTITY(1,1)		primary key,
	ProductId			int,
	Star				int
)

CREATE TABLE Orders
(
	Id					int			IDENTITY(1,1)		primary key,
	CustomerId			int,
	CreateTime			date,
	Price				float,
	Status				int
)

CREATE TABLE OrdersDetail
(
	Id					int			IDENTITY(1,1)		primary key,
	OrderId				int,
	ProductId			int,
	Quantity			int,
	Price				float
)

CREATE TABLE Tags
(	
	Id					int			IDENTITY(1,1)		primary key,
	Name				nvarchar(500)
)

CREATE TABLE TagsProducts
(
	Id					int			IDENTITY(1,1)		primary key,
	TagId				int,
	ProductId			int
)