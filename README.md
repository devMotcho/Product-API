# Product API Documentation

## Table of Contents

- [Overview](#Overview)
- [Tools](#Tools)
- [Authentication](#Authentication)
  - [JWT](#JWT)
  - [Hashing](#hashing)
- [Installation](#installation)
- [ProductDataInjectionService](#ProductDataInjectionService)
- [Configuration](#configuration-of-appsettingsjson)
- [Usage](#usage)
- [AuthController](#authcontroller)
- [ProductController](#productcontroller)
- [ProductEFController](#productefcontroller)
- [UserEFController](#userefcontroller)

## Overview

The Product API provides a set of endpoints for managing products, users, and authentication in an application. It is built using ASP.NET Core and it includes endpoints for product and user management, as well as authentication features such as registration and login.

## Tools

For this project i used:

- Dapper;
- Entity Framework;
- Mapper;
- Newtonsoft;
- AutoMapper;
- Azure Data Studio;
- Posteman (testing);

## Authentication

### Hashing

#### On Registration

- Password Salt is generated randomly;
- Password Hash is generated with the `GetPasswordHash` method;
- `GetPasswordHash` takes the user password and the salt generated as `args`;
- Inside the method `GetPasswordHash`:
  - It gets the `AppSetings:PasswordKey` from the `appsettings.json` file that is a random string and adds it to the Salt;
  - Implements then password-based key derivation functionality, `PBKDF2`, by using a pseudo-random number generator based on `HMACSHA256`;
  - returns the `passwordHash`;
- `PasswordHash` and `PasswordSalt` are stored in the database using a parameter SQL query with Dapper.

#### On Login

- The `Password Salt` and the `Password Hash` are extracted from the database using the `Email` sent in the request body;
- The `GetPasswordHash` method is called again taking the password sent on the request and the passwordSalt generating a `passwordHash`;
- The generated `passwordHash` is then compared with the `passwordHash` from the database `byte` by `byte`, otherwise it would only compare objects (memory addresses).

### JWT

- JWT is used for authentication
- Users register and login to receive a token
- The token must be included in the `Authorization` header for authenticated requests.
- Requests without a valid token will be rejected with a 401 status code.

## Installation

1. Clone the repository:

```
git clone https://github.com/devMotcho/Product-API.git
cd Product-API
```

2. Install Dependencies:

```
dotnet restore
```

3. Build the project

```
dotnet build
```

4. Enable your localhost server on **Azure Data Studio**

5. Copy and Past `CreateTables.sql` in **Azure Data Studio**

6. Start development server
```
dotnet watch run
```

## ProductDataInjectionService

This service will inject `Products.json`, dummy objects, into the database.

## Configuration of appsettings.json

```
{
  "ConnectionStrings": {
    "DefaultConnection":"Server=localhost; Database=MyProductsDatabase; Trusted_Connection=true; TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "PasswordKey": "you_random_password_key",
    "TokenKey": "you_random_token_key"
  }
}
```

## Usage

### AuthController

The `AuthController` manages authentication, including user registration and login.

#### Register

- URL: `/Auth/Register`
- Method: `POST`
- Request Body:

```
{
  "Email": "string",
  "Password": "string",
  "PasswordConfirm": "string"
}
```

- Responses:
  - **200 OK** : User registered successfully.
  - **400 Bad Request** : Passwords do not match or user aldready exists.
  - **500 Internal Server** : Failed to register user.

#### Login

- URL: `Auth/Login`
- Method: `POST`
- Request Body:

```
{
  "Email": "string",
  "Password": "string"
}
```

- Responses:
  - **200 OK** : Returns a JWT token.
  - **401 Unauthorized** : Incorrect password.
  - **404 Not Found** : User not found.
  - **500 Internal Server** : Failed to login user.

#### RefreshToken

- URL: `Auth/RefreshToken`
- Method: `GET`
- Responses:
  - **200 OK** : Returns a new JWT token.

### ProductController

The `ProductController` manages products using direct SQL queries with dapper.

#### TestConnection

- URL: `/Product/TestConnection`
- Method: `GET`
- Responses:
  - **200 OK** : Returns the current date and time from the database

#### GetProducts

- URL: `/Product/GetProducts`
- Method: `GET`
- Responses:
  - **200 OK** : Returns a list of products.

#### GetSingleProduct

- URL: `/Product/GetSingleProduct/{productId}`
- Method: `GET`
- Path Parameters: `productId` (int)
- Responses:
  - **200 OK** : Returns a single product.

#### EditProduct

- URL: `/Product/EditProduct`
- Method: `PUT`
- Request Body:

```
{
  "ProductId": "int",
  "Name": "string",
  "Description": "string",
  "Price": "decimal",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : Product updated successfully.
  - **400 Bad Request** : Invalid product data.
  - **404 Not Found** : Product not found.
  - **500 Internal Server Error** : Failed to update product.

#### AddProduct

- URL: `/Product/AddProduct`
- Method: `POST`
- Request Body:

```
{
  "Name": "string",
  "Description": "string",
  "Price": "decimal",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : Product added successfully.
  - **400 Bad Request** : Invalid product data.
  - **500 Internal Server Error** : Failed to add product.

#### DeleteProduct

- URL: `/Product/DeleteProduct/{productId}`
- Method: `DELETE`
- Path Parameters: `productId` (int)
- Responses:
  - **200 OK** : Product deleted successfully.
  - **404 Not Found** : Product not found.
  - **500 Internal Server Error** : Failed to delete product.

### ProductEFController

The `ProductEFController` manages products using Entity Framework.

#### GetProducts

- URL: `/ProductEF/GetProducts`
- Method: `GET`
- Responses:
  - **200 OK** : Returns a list of products.

#### GetSingleProduct

- URL: `/ProductEF/GetSingleProduct/{productId}`
- Method: `GET`
- Path Parameters: `productId` (int)
- Responses:
  - **200 OK** : Returns a single product.

#### EditProduct

- URL: `/ProductEF/EditProduct`
- Method: `PUT`
- Request Body:

```
{
  "ProductId": "int",
  "Name": "string",
  "Description": "string",
  "Price": "decimal",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : Product updated successfully.
  - **400 Bad Request** : Invalid product data.
  - **404 Not Found** : Product not found.
  - **500 Internal Server Error** : Failed to update product.

#### AddProduct

- URL: `/ProductEF/AddProduct`
- Method: `POST`
- Request Body:

```
{
  "Name": "string",
  "Description": "string",
  "Price": "decimal",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : Product added successfully.
  - **400 Bad Request** : Invalid product data.
  - **500 Internal Server Error** : Failed to add product.

#### DeleteProduct

- URL: `/ProductEF/DeleteProduct/{productId}`
- Method: `DELETE`
- Path Parameters: `productId` (int)
- Responses:
  - **200 OK** : Product deleted successfully.
  - **404 Not Found** : Product not found.
  - **500 Internal Server Error** : Failed to delete product.

### UserEFController

The `UserEFController` manages users using Entity Framework.

#### GetUsers

- URL: `/UserEF/GetUsers`
- Method: `GET`
- Responses:
  - **200 OK** : Returns a list of users.

#### GetSingleUser

- URL: `/UserEF/GetSingleUser/{userId}`
- Method: `GET`
- Path Parameters: `userId` (int)
- Responses:
  - **200 OK** : Returns a single user.

#### EditUser

- URL: `/UserEF/EditUser`
- Method: `PUT`
- Request Body:

```
{
  "UserId": "int",
  "FirstName": "string",
  "LastName": "string",
  "Email": "string",
  "Gender": "string",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : User updated successfully.
  - **400 Bad Request** : Invalid user data.
  - **404 Not Found** : User not found.
  - **500 Internal Server Error** : Failed to update user.

#### AddUser

- URL: `/UserEF/AddUser`
- Method: `POST`
- Request Body:

```
{
  "FirstName": "string",
  "LastName": "string",
  "Email": "string",
  "Gender": "string",
  "Active": "bool"
}
```

- Responses:
  - **200 OK** : User added successfully.
  - **400 Bad Request** : Invalid user data.
  - **500 Internal Server Error** : Failed to add user.

#### DeleteUser

- URL: `/UserEF/DeleteUser/{userId}`
- Method: `DELETE`
- Path Parameters: `userId` (int)
- Responses:
  - **200 OK** : User deleted successfully.
  - **404 Not Found** : User not found.
  - **500 Internal Server Error** : Failed to delete user.
