create table app.BookingDocument ( 
	Id int identity (1, 1) not null,
	Title nvarchar(200) not null,
	MimeType nvarchar(200) not null,
	Body varbinary(max) not null,
	CreatedDate datetime not null,
	
	constraint PK_BookingDocument primary key (Id)
)

