# Project InDebt

## Project Description
The system is designed to optimize debt relations between users. 
Th system that helps the user to control their debts. 
The system also includes the ability to combine users into groups so that they can 
conveniently pay for group expenses. The system has the ability to optimize 
debts within the group in order to make it 
easier to pay off debts and receive debts. The system has the ability to 
store debt histories for analysis for individual purposes by the user. 
The system can also notify the user about new debts or show that the debt has been 
returned to him.

## Pre requirement

- Microsoft SQL Server
- .NET 7.0

## Setup
- Install EF Core with command ```dotnet tool install --global dotnet-ef```
- Apply migrations with command ```dotnet ef database update -p Persistence -s Presentation``` || just run app
- Run project with command ```dotnet run```
