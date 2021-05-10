USE [DbIntegrationTests]
GO
 

CREATE TABLE Customers(
	id int IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	name varchar(255) NOT NULL,
	amount numeric(12,6) null, 
	notation float null,
	traderid int null,
	creationdate datetime NULL,
	enabled bit default 1
)
GO 

CREATE PROCEDURE sp_insert_customers(@name varchar(255), @amount numeric(12,6), @notation float, @traderid int, @creationdate datetime )
as
begin 

Insert into Customers( name , amount , notation , traderid , creationdate )
		values ( @name, @amount, @notation, @traderid , @creationdate )
end 

select @@IDENTITY 'customerId'
go 


-- exec sp_insert_Customers 'yan', 150.2, 10,13974,'20210427'

-- select * from Customers 

-- delete from Customers