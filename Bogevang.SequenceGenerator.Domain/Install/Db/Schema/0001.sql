create table app.SequenceCounter ( 
	[Name] varchar(100) not null,
	Counter int not null,
	
	constraint PK_SequenceCounter primary key ([Name])
)

